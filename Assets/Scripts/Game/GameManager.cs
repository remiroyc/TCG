using DG.Tweening;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class GameManager : MonoBehaviour
{

    public int CurrentWave = 0;
    public bool WaveStarted = false;
    public bool PauseEnabled = false;
    public int CountDown = 0;
    public Transform Joystick;
    public Transform WavePanel, ButtonsPanel, ScrollPanel;
    public Text TimerText;
    public Transform PlayerSpawn;
    private bool _introductionLaunched = false;

    //private bool _timerEnable = false;
    private bool _inCoroutine = false;
    public RectTransform UserPanel;

    public AudioClip Bip1, Bip2;

    [Serializable]
    public class Wave
    {
        public EnemyType[] Enemies;
    }

    public Wave[] Waves;
    private Transform[] _instantiatedEnemies = null;

    #region SINGLETON

    private static GameManager _instance;
    [HideInInspector]
    public static GameManager Instance
    {
        get { return _instance ?? (_instance = Object.FindObjectOfType<GameManager>()); }
    }

    #endregion

    #region MONO BEHAVIOUR

    private void Awake()
    {
        _instance = this;
        MyCharacterController.Instance.OrbitCameraScript.enabled = false;
        MyCharacterController.Instance.AutoFocus.enabled = false;
        MyCharacterController.Instance.GetComponent<FollowCamera>().enabled = false;
    }

    private void Start()
    {
        ActivateNextWave();
    }

    private void Update()
    {
        if (MyCharacterController.Instance.Win || _instantiatedEnemies.All(e => e == null || e.GetComponent<Enemy>().IsDied))
        {
            // var enemy = _instantiatedEnemies.FirstOrDefault(e => e == null || e.GetComponent<Enemy>().IsDied);
            if (!_inCoroutine)
            {
                StartCoroutine(WaitAndActivateWin(5f));
            }
        }
    }

    #endregion

    private void DisplayNextWaveMenu()
    {
        WavePanel.gameObject.SetActive(true);
        ButtonsPanel.gameObject.SetActive(false);
        Joystick.gameObject.SetActive(false);

        var lifeText = GameObject.Find("LifeText");
        if (lifeText != null)
        {
            lifeText.GetComponent<Text>().text = string.Format("{0} / {1} pv", MyCharacterController.Instance.Life,
                MyCharacterController.Instance.MaxLife);
        }

		// ScrollPanel.transform.DetachChildren();
/*
		foreach (var enemy in wave.Enemies)
		{
			var enemyPanel = Instantiate(Resources.Load<GameObject>("Prefabs/UI/EnemyPanel"),
		}
*/
    }

    public void ActivateNextWave()
    {
        MyCharacterController.Instance.Win = false;
        MyCharacterController.Instance.CanAttack = false;
        MyCharacterController.Instance.CanMove = false;

        DisplayNextWaveMenu();

        if (CurrentWave + 1 <= Waves.Length)
        {
            LaunchGameIntroduction();
        }
    }

    private IEnumerator WaitAndActivateWin(float waitTime)
    {
        _inCoroutine = true;
        yield return new WaitForSeconds(waitTime);
        if (_instantiatedEnemies != null)
        {
            foreach (var enemy in _instantiatedEnemies.Where(enemy => enemy != null))
            {
                Destroy(enemy.gameObject);
            }
        }
        ++CurrentWave;
        WaveStarted = false;
        ActivateNextWave();
        _inCoroutine = false;
    }

    private void ActivateFight()
    {
        Debug.Log("ActivateFight()");

        WaveStarted = true;
        MyCharacterController.Instance.GetComponent<FollowCamera>().enabled = true;

        foreach (var enemyScript in _instantiatedEnemies.Select(enemies => enemies.GetComponent<Enemy>()))
        {
            enemyScript.Target = MyCharacterController.Instance.transform;
            enemyScript.StartAttack();
        }

        if (TimerText != null)
        {
            TimerText.transform.parent.gameObject.SetActive(false);
        }

        ButtonsPanel.gameObject.SetActive(true);
        Joystick.gameObject.SetActive(true);

        MyCharacterController.Instance.CanAttack = true;
        MyCharacterController.Instance.CanMove = true;

        MyCharacterController.Instance.OrbitCameraScript.enabled = true;
        MyCharacterController.Instance.AutoFocus.enabled = true;

    }

    private void LaunchGameIntroduction()
    {
        _introductionLaunched = true;
        InstantiateEnemies();
        _introductionLaunched = false;
    }

    private void InstantiateEnemies()
    {
        var wave = Waves[CurrentWave];
        _instantiatedEnemies = new Transform[wave.Enemies.Length];

        for (int i = 0; i < wave.Enemies.Length; i++)
        {
            var enemyType = wave.Enemies[i];
            GameObject obj = null;
            switch (enemyType)
            {
                case EnemyType.Cell:
                    obj = Resources.Load<GameObject>("Prefabs/CellPrefab");
                    obj.name = string.Concat("Cell", i.ToString(CultureInfo.InvariantCulture));
                    break;
                case EnemyType.Saibaiman:
                    obj = Resources.Load<GameObject>("Prefabs/SaibaimanPrefab");
                    obj.name = string.Concat("Saibaiman", i.ToString(CultureInfo.InvariantCulture));
                    break;
                case EnemyType.SBuu:
                    obj = Resources.Load<GameObject>("Prefabs/SBuuPrefab");
                    obj.name = string.Concat("SBuu", i.ToString(CultureInfo.InvariantCulture));
                    break;
			case EnemyType.Freezer1:
				obj = Resources.Load<GameObject>("Prefabs/FreezerPrefab");
				obj.name = string.Concat("Freezer1", i.ToString(CultureInfo.InvariantCulture));
				break;
            }
            if (obj != null)
            {
                var position = new Vector3(90 + i * 5, 24, 79);
                var go = (GameObject)Instantiate(obj, position, Quaternion.identity);
                _instantiatedEnemies[i] = go.transform;
            }
        }



        MyCharacterController.Instance.transform.position = PlayerSpawn.transform.position;
        Camera.main.transform.position = _instantiatedEnemies[0].position + _instantiatedEnemies[0].forward + (Vector3.up * 0.5f);
        MyCharacterController.Instance.transform.LookAt(_instantiatedEnemies[0]);

    }

    public void LaunchGame()
    {
        UserPanel.gameObject.SetActive(true);

        var endPos = MyCharacterController.Instance.transform.position - MyCharacterController.Instance.transform.forward + (Vector3.up * 0.5f);

        Camera.main.transform.DOMove(endPos, CountDown);

        // PlayerSpawn.LookAt(_instantiatedEnemies[0]);
        // MyCharacterController.Instance.transform.LookAt(_instantiatedEnemies[0]);
        // MyCharacterController.Instance.transform.position = PlayerSpawn.transform.position;
        //  Camera.main.transform.position = PlayerSpawn.transform.position - PlayerSpawn.transform.forward;


        WavePanel.gameObject.SetActive(false);
        StartCoroutine(CloakTimer());
    }

    private IEnumerator CloakTimer()
    {



        if (TimerText != null)
        {
            TimerText.transform.parent.gameObject.SetActive(true);
        }

        if (!WaveStarted)
        {
            GetComponent<AudioSource>().volume = 1;
            for (int i = 1; i <= CountDown; i++)
            {
                if (TimerText != null)
                {
                    TimerText.text = i.ToString(CultureInfo.InvariantCulture);
                }
                GetComponent<AudioSource>().PlayOneShot(Bip1);
                yield return new WaitForSeconds(1);
            }
            if (TimerText != null)
            {
                TimerText.text = "FIGHT !";
            }
            GetComponent<AudioSource>().PlayOneShot(Bip2);
            yield return new WaitForSeconds(1);
            ActivateFight();
        }
    }

}
