using System.Collections;
using UnityEngine;

/// <summary>
/// Script qui permet de gérer la capacité Genkidama de Sangoku
/// </summary>
public class GenkidamaScript : SkillBase
{

    public AudioClip GenkidamaCharge;
    public ParticleSystem GenkidamaEnergy;
    public OrbitCamera CameraController;
    private bool _heightFixed;
    private bool _moveCameraForGenki;

    public GameObject mapToDestroy;
    public GameObject mapToReveal;

    public Camera CameraTp;
    private bool _isLookingAt;
    public Transform wayPoint;
    private GameObject _genki;
    private bool _isLookingAtGenki;

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        CurrentSkillType = SkillType.Genkidama;

    }

    // ReSharper disable once UnusedMember.Local
    private void FixedUpdate()
    {
        GenkidamaReadyCheck();

        if (_isLookingAt)
            Camera.main.transform.LookAt(this.transform);

        if (_isLookingAtGenki)
            Camera.main.transform.LookAt(_genki.transform);
    }

    // ReSharper disable once UnusedMember.Local
    private void LateUpdate()
    {
        if (_moveCameraForGenki)
        {
            var cam = CameraController.transform;
            cam.position = Vector3.Slerp(cam.position, cam.localPosition + Vector3.back, Time.deltaTime * 0.7f);

            //CameraTp.transform.LookAt(this.transform);
            //CameraTp.transform.Translate(Vector3.left * Time.deltaTime);
        }

    }

    /// <summary>
    /// Méthode pour commencer à lancer un genkidama. Dans un premier temps le personnage va s'élever du sol...
    /// </summary>
    public void StartGenkidama()
    {
        if (!MyCharacterController.Instance.TransformationManager.IsSuperSaiyan && CanActivateSkill)
        {
            MyCharacterController.Instance.CurrentSkill = CurrentSkillType;
            SubstractKi();

            //Effet de camera :
            Camera.main.GetComponent<OrbitCamera>().enabled = false;
            _isLookingAt = true;


            GetComponent<Rigidbody>().AddForce(Vector3.up * 1000);
            _heightFixed = true;
            MyCharacterController.Instance.CanMove = false;
            MyCharacterController.Instance.CanAttack = false;
            MyCharacterController.Instance.CharAnimator.Play("Genkidama Charge");
        }
    }

    /// <summary>
    /// Méthode qui permet de vérifier que le personnage est à une bonne hauteur du sol. Si c'est le cas il lance son genkidama. 
    /// </summary>
    private void GenkidamaReadyCheck()
    {
        if (_heightFixed && transform.position.y >= 15f)
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;

            if (!CameraController.enabled)
            {
                CameraController.enabled = false;
            }

            if (MyCharacterController.Instance.FocusEnemy != null)
            {
                //Camera.main.transform.LookAt(CharController.FocusEnemy);
            }

            iTween.MoveTo(Camera.main.gameObject, iTween.Hash(
                    "position", wayPoint.position + Vector3.back * 3f,
                    "time", 2f,
                    "islocal", false,
                    "oncompletetarget", this.gameObject,
                    "transition", "easeInOutSine",
                    "oncomplete", "OnTweenWaypointEnterComplete"));



            //_moveCameraForGenki = true;
            GetComponent<AudioSource>().PlayOneShot(GenkidamaCharge);

            // On calcule le vecteur entre la position de notre personnage et l'ennemi

            //if (FocusEnemy != null)

            var genkidamaObj = Resources.Load<GameObject>("Prefabs/GenkidamaPrefab");
            var genkidamaPosition = transform.position + Vector3.up * 8f;
            var genkidamaRotation = Quaternion.LookRotation((GameObject.Find("EnnemyPrefab").transform.position - transform.position) + Vector3.down * 10);

            _genki = Instantiate(genkidamaObj, genkidamaPosition, genkidamaRotation) as GameObject;
            //else
            //	Instantiate (Resources.Load("Prefabs/GenkidamaPrefab"), transform.position + Vector3.up * 10f, this.transform.rotation);

            GenkidamaEnergy.Play();
            _heightFixed = false;
            MyCharacterController.Instance.CurrentSkill = null;
        }
    }

    private void OnTweenWaypointEnterComplete()
    {
        _isLookingAt = false;
        iTween.LookTo(Camera.main.gameObject, transform.position + Vector3.up * 1.5f, 5f);
        StartCoroutine(ChangeView());
    }

    public IEnumerator ChangeView()
    {
        yield return new WaitForSeconds(8f);
        iTween.LookTo(Camera.main.gameObject, GameObject.Find("EnnemyPrefab").transform.position, 5f);
    }

}
