using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Script qui permet de gérer la capacité Kamehameha de notre personnage
/// </summary>
public class KamehamehaScript : SkillBase
{

    public ParticleSystem[] Effects;
    public AudioSource KameChargeAudioSource;
    public AudioClip KameControl, BusterFire;
    public GameObject KamehamehaPrefab;
    public GameObject CameraKameha;
    public GameObject WayPointKameha;
    public GameObject KameLight;

    private Vector3 _oldPosCamera;
    private GameObject _currentKamehameha = null;
    private bool _charging = false, _control = false;

    /// <summary>
    /// Temps de chargement requis pour lancer un kamehameha
    /// </summary>
    public float ChargeTime = 2f;

    /// <summary>
    /// Temps maximum de controle du kamehameha
    /// </summary>
    public float MaxControlTime = 3f;

    /// <summary>
    /// Référence du temps au moment de la charge du kamehameha
    /// </summary>
    private float _kamehamehaTimer;

    /// <summary>
    /// Référence du temps au moment du lancement du kamehameha
    /// </summary>
    private float _kamehamehaControlTimer;

    private const string ControlAnimName = "Kame Control", ChargeAnimName = "kame_charging";


    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        CurrentSkillType = SkillType.Kamehameha;
        _oldPosCamera = CameraKameha.transform.position;
    }
    void Start()
    {
        DOTween.Init(true, true, LogBehaviour.Default);
    }

    void Update()
    {
        if (_currentKamehameha != null && _control && (Time.time - _kamehamehaControlTimer >= MaxControlTime))
        {
            StopCoroutine(WaitDeathOfKamehameha());
            _currentKamehameha.GetComponent<EnergyBeam>().DestroyKamehameha();
            _currentKamehameha = null;
            CancelKame();
        }
    }

    public void ChargeKame()
    {
        if (CanActivateSkill)
        {
            DesactivateOtherSkill();
            MyCharacterController.Instance.CurrentSkill = CurrentSkillType;

            _charging = true;
            _kamehamehaTimer = Time.time;

            MyCharacterController.Instance.CharAnimator.SetBool(ChargeAnimName, true);
            MyCharacterController.Instance.CanMove = false;
            MyCharacterController.Instance.StopVelocity(withYAxe: true);

            // On play les animations de particules
            foreach (var particle in Effects)
            {
                particle.Play();
            }

            if (!KameChargeAudioSource.isPlaying)
            {
                KameChargeAudioSource.Play();
            }

            KameLight.SetActive(true);

            DOTween.To(() => MyCharacterController.Instance.OrbitCameraScript.Distance,
                x => MyCharacterController.Instance.OrbitCameraScript.Distance = x, 0.5f, 0.5f);

        }
    }

    /// <summary>
    /// Méthode qui permet de lancer un kamehameha suite à une charge
    /// </summary>
    /// <param name="inComboAttack">Booléen qui signifie que le kamehameha est lancé à la suite d'un combo (tp + attaque).</param>
    public void LaunchKame(bool inComboAttack = false)
    {

        if (!_charging || _control)
        {
            return;
        }

        MyCharacterController.Instance.CanMove = false;

        // On stop toutes les animations de particules
        foreach (var particle in Effects.Where(p => p.isPlaying))
        {
            particle.Stop();
            particle.Clear();
        }

        if (KameChargeAudioSource.isPlaying)
        {
            KameChargeAudioSource.Stop();
        }

        float chargingTime = Time.time - _kamehamehaTimer;
        // Si le temps de charge du kamehameha est inférieur à 2 secondes on ne fait rien.
        if ((MyCharacterController.Instance.LoadingSkill
            && MyCharacterController.Instance.CurrentSkill == SkillType.Kamehameha
            && chargingTime >= ChargeTime)
            || inComboAttack
            || CanActivateSkill)
        {

            _charging = false;

            SubstractKi();
            //CharController.CharAnimator.Play(ControlAnimName);
            MyCharacterController.Instance.CharAnimator.SetBool("kame_charging", false);
            MyCharacterController.Instance.CharAnimator.SetBool("kame_controlling", true);
            // On détache le perso de la camera

            MyCharacterController.Instance.OrbitCameraScript.enabled = false;
            // MyCharacterController.Instance.GetComponent<FollowCamera>().enabled = false;

            if (!inComboAttack)
            {
                KameChargeAudioSource.Stop();
                GetComponent<AudioSource>().PlayOneShot(KameControl);
            }
            else
            {
                GetComponent<AudioSource>().PlayOneShot(BusterFire);
            }

            DOTween.To(() => Camera.main.GetComponent<Camera>().fieldOfView, x => Camera.main.GetComponent<Camera>().fieldOfView = x, 90, 0.2f);

            if (KamehamehaPrefab != null)
            {
                if (!inComboAttack)
                {
                    var direction = (MyCharacterController.Instance.FocusEnemy != null
                        ? transform.position - MyCharacterController.Instance.FocusEnemy.position
                        : transform.position - CharacterSight.Instance.GetImpactPoint()).normalized;

                    var kamePosition = transform.position + (-direction * 0.2f) + Vector3.up / 7;
                    _currentKamehameha = (GameObject)Instantiate(KamehamehaPrefab, kamePosition, Quaternion.LookRotation(direction));

                    var energyBeam = _currentKamehameha.GetComponent<EnergyBeam>();

                    energyBeam.Mask = MyCharacterController.Instance.EnemyLayerMask;
                    energyBeam.KamehamehaDirection = -direction;
                    energyBeam.PlayerEmitter = MyCharacterController.Instance;
                    _kamehamehaControlTimer = Time.time;

                    _control = true;

                    StartCoroutine(WaitDeathOfKamehameha());

                }
                else
                {
                    var kameObj = Instantiate(KamehamehaPrefab) as GameObject;
                    if (kameObj != null)
                    {
                        kameObj.transform.Rotate(new Vector3(90f, 0));
                        kameObj.transform.position = new Vector3(transform.position.x, transform.position.y,
                            transform.position.z);
                    }
                }
            }
        }
        else
        {
            CancelKame();
        }
    }

    /// <summary>
    /// Méthode qui attend que le kamehameha soit terminé avant de réinitialiser les parametres du joueur.
    /// </summary>
    IEnumerator WaitDeathOfKamehameha()
    {
        do
        {
            yield return new WaitForEndOfFrame();
        } while (_currentKamehameha != null);
        ReinitializeMyCharacter();
    }

    private void ReinitializeMyCharacter()
    {
        _kamehamehaTimer = 0;
        KameLight.SetActive(false);

        MyCharacterController.Instance.CanMove = true;
        MyCharacterController.Instance.CanAttack = true;
        MyCharacterController.Instance.CurrentSkill = null;
        MyCharacterController.Instance.CharAnimator.SetBool(ChargeAnimName, false);

        if (!MyCharacterController.Instance.OrbitCameraScript.enabled && !MyCharacterController.Instance.AutoFocus.AutoFocus)
        {
            MyCharacterController.Instance.OrbitCameraScript.enabled = true;
        }

        if (MyCharacterController.Instance.OrbitCameraScript.Distance < 1.3f)
        {
            DOTween.To(() => MyCharacterController.Instance.OrbitCameraScript.Distance,
                x => MyCharacterController.Instance.OrbitCameraScript.Distance = x, 1.3f, 0.5f);

            DOTween.To(() => Camera.main.GetComponent<Camera>().fieldOfView, x => Camera.main.GetComponent<Camera>().fieldOfView = x, 60, 0.5f);
        }
    }

    /// <summary>
    /// Méthode qui permet de désactiver le kaméhaméha quand on se prend des dégats par exemple
    /// </summary>
    public void CancelKame()
    {
        _charging = false;
        _control = false;

        MyCharacterController.Instance.CharAnimator.SetBool("kame_charging", false);
        MyCharacterController.Instance.CharAnimator.SetBool("kame_controlling", false);

        if (!MyCharacterController.Instance.CanMove)
        {
            MyCharacterController.Instance.CanMove = true;
        }

        if (_currentKamehameha != null)
        {
            Destroy(_currentKamehameha);
        }

        _currentKamehameha = null;

        foreach (var particle in Effects)
        {
            particle.Stop();
            particle.Clear();
        }

        if (KameChargeAudioSource.isPlaying)
        {
            KameChargeAudioSource.Stop();
        }


        ReinitializeMyCharacter();
    }

}
