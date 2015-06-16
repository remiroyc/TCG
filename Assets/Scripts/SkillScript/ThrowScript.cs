using System.Collections;
using UnityEngine;

public class ThrowScript : MonoBehaviour
{
    public MyCharacterController CharController;
    public AudioSource SwingAudioSource;
    public Camera PlayerCamera;
    private GameObject _characterLeftHand;
    private bool _isLookingAt;
    private bool _askForQTE;

    private void Start()
    {
        _characterLeftHand = GameObject.FindGameObjectWithTag("Hand");
    }

    void LateUpdate()
    {
        if (_isLookingAt)
        {
            Camera.main.transform.LookAt(this.transform);
            Camera.main.transform.Translate(Vector3.left * Time.deltaTime);
        }
    }

    void Update()
    {
        // Si l'ennemi est projeté on propose de se tp derrière lui
        if (_askForQTE)
        {
           
        }
    }

    /// <summary>
    /// Lance le combo d'attaque où notre personnage attrape l'ennemi pour le faire tournoyer
    /// </summary>
    public void StartSwing()
    {
        if (!CharController.LoadingSkill && CharController.FocusEnemy != false && CharController.CloseCombat)
        {
            CharController.Attacking = true;
            CharController.CanMove = false;
            CharController.CanAttack = false;

            PlayerCamera.gameObject.SetActive(true);

            var enemyScript = CharController.FocusEnemy.GetComponent<Enemy>();
            enemyScript.CanMove = false;
            enemyScript.CanAttack = false;

            CharController.CharAnimator.SetBool("releaseThrow", false);

            CharController.FocusEnemy.GetComponent<Enemy>().TakeDamage("throw");
            //FocusEnemy.transform.Rotate(new Vector3(0f, 180f, 0f));
            _characterLeftHand = CharController.DamageManager.LeftHand.gameObject;
            CharController.FocusEnemy.transform.position = _characterLeftHand.transform.position;
            CharController.FocusEnemy.transform.parent = _characterLeftHand.transform;
            CharController.FocusEnemy.transform.localPosition = new Vector3(0.04166669f, -0.07807709f, 0.1318349f);
            CharController.FocusEnemy.transform.localRotation = Quaternion.Euler(new Vector3(325.6549f, 203.7312f, 156.9356f));
            //CharController.FocusEnemy.GetComponent<CapsuleCollider>().enabled = false;
            //CharController.FocusEnemy.GetComponent<Enemy>().  enabled = false;

            CharController.FocusEnemy.GetComponent<Rigidbody>().isKinematic = true;
            transform.GetComponent<Rigidbody>().isKinematic = true;
            //transform.position += Vector3.up;
            //CharController.FocusEnemy.transform.position += Vector3.up;

            CharController.CharAnimator.Play("Swinger Fast");

            var spin = CharController.AudioController.AudioClips["spin"];
            GetComponent<AudioSource>().clip = spin;
            GetComponent<AudioSource>().Play();

            StartCoroutine(ThrowFoe());

        }
    }

    private IEnumerator ThrowFoe()
    {
        // On attend 3 secondes avant de lancer
        yield return new WaitForSeconds(3);

        transform.GetComponent<Rigidbody>().isKinematic = false;

        if (GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Stop();
        }
        GetComponent<AudioSource>().clip = CharController.AudioController.AudioClips["projection"];
        GetComponent<AudioSource>().Play();

        //GameObject.Find ("Main Camera").GetComponent<CameraController> ().enabled = true;
        CharController.CharAnimator.SetBool("startThrow", false);
        CharController.CharAnimator.SetBool("releaseThrow", true);


        CharController.FocusEnemy.transform.parent = null;
        CharController.FocusEnemy.GetComponent<Rigidbody>().isKinematic = false;
        //FocusEnemy.rigidbody.AddForce(Vector3.up * 8f);
        CharController.FocusEnemy.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 10f, 25f), ForceMode.Impulse);
        CharController.FocusEnemy.transform.rotation = Quaternion.Euler(Vector3.zero);
        //FocusEnemy.rigidbody.MovePosition(Vector3.forward * 0.1f  * Time.deltaTime);

        var cell = GameObject.Find("Cell");
        if (cell != null)
        {
            var cellAnimator = cell.GetComponent<Animator>();
            cellAnimator.SetBool("thrown", true);
            cellAnimator.SetBool("swung", false);
            _askForQTE = true;
        }

        CharController.CanMove = true;
        CharController.CanAttack = true;

        
        var enemy = CharController.FocusEnemy.GetComponent<Enemy>();

        enemy.CanMove = true;
        enemy.CanAttack = true;

        PlayerCamera.gameObject.SetActive(false);
    }

}
