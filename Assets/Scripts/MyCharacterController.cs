using Assets.Scripts.SkillScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource), typeof(TeleportationComboScript), typeof(AutoFocusScript))]
public class MyCharacterController : Saiyan
{

    #region SINGLETON

    private static MyCharacterController _instance;
    public static MyCharacterController Instance
    {
        get { return _instance ?? (_instance = Object.FindObjectOfType<MyCharacterController>()); }
    }

    #endregion

    #region PUBLIC PROPERTIES

    public bool TutorialMode = false;

    [HideInInspector]
    public CharacterMoveState CurrentCharState;
    [HideInInspector]
    public bool Win = false;
    [HideInInspector]
    public bool Loose = false;
    [HideInInspector]
    public bool CloseCombat = false;
    [HideInInspector]
    public Vector2 InputMovement = Vector2.zero;
    [HideInInspector]
    public bool ChargeActivated = false;
    public GameObject FootTrail;

    public TeleportationComboScript TeleportationManager;
    public TransformationScript TransformationManager;
    public GenkidamaScript GenkidamaManager;
    public KamehamehaScript KamehamehaManager;
    public KaiokenScript KaiokenManager;
    public DashScript DashManager;
    public KiBlastScript KiBlastManager;
    public ThrowScript ThrowManager;
    public KiaiScript KiaiManager;

    public Image EnergyBarImage;
    public Image LifeBarImage;
    public MyAudioController AudioController;
    public float AccelerationSpeed = 20f;
    public float AirSpeed = 30f;
    public OrbitCamera OrbitCameraScript;
    public ParticleSystem FX_power_up_dust;
    public float MaxMagnitudeSpeed = 2f;
    public float RotationSpeed = 100f;
    public List<GameObject> l_kame_effects = new List<GameObject>();
    public AudioSource KiChargeAudioSource, RunAudioSource, FlyAudioSource, DisableClickAudioSource;
    public GameObject Cursor;
    public float JumpHeight = 500f;
    public Slider AButtonLoader;
    public AutoFocusScript AutoFocus;
    public Transform FocusEnemy;
    public float MaxChargeTime = 3;
    public DamageManager DamageManager;
    public Text ComboText, ComboHistory;

    #endregion

    #region PRIVATES PROPERTIES

    private Dictionary<int, string> AnimHashes = new Dictionary<int, string>();
    private string _currentAnimationTrigger;

    /// <summary>
    /// Trail Renderer utilisé lors d'une attaque pied/poing
    /// </summary>
    private GameObject _currentAttackTrail;

    private bool _moveBack;
    /// <summary>
    /// Booléen qui permet de lancer la vérification "touche t'on touche le sol ?" uniquement aprés le jump.
    /// </summary>
    private bool _hasJumped;
    private float _kiMultiplicator = 1;
    private float _lastKeyPressed;
    private float _comboTimer;
    /// <summary>
    /// Vitesse de déplacement du personnage
    /// </summary>
    private float _currentSpeed;
    private float _chargeButtonTime;
    private int _currentAttackState;
    private bool _checkEndAnim;
    /// <summary>
    /// File d'attente des attaques a traiter (animations). Elle se réinitialise toute les secondes.
    /// </summary>
    private Queue<char> _comboQueue = new Queue<char>();
    /// <summary>
    /// Historique des attaques passées. Elle se réinitialise toute les 4 secondes (ce qui permet de réaliser des combos).
    /// </summary>
    private string _comboHistory = string.Empty;
    private string _skillCharged;
    private bool _btnClicked;
    private float _chargeKiTimer;

    #endregion

    #region MONO BEHAVIOUR METHODS

	protected override void Awake()
    {
        _instance = this;
        IsMyPlayer = true;

        base.Awake();

        // Debug.Log(.GetLayer(count).stateMachine.GetState(0).name);

        if (TeleportationManager == null)
        {
            TeleportationManager = GetComponent<TeleportationComboScript>();
        }
        if (TransformationManager == null)
        {
            TransformationManager = GetComponent<TransformationScript>();
        }
        if (GenkidamaManager == null)
        {
            GenkidamaManager = GetComponent<GenkidamaScript>();
        }
        if (KiBlastManager == null)
        {
            KiBlastManager = GetComponent<KiBlastScript>();
        }
        if (ThrowManager == null)
        {
            ThrowManager = GetComponent<ThrowScript>();
        }
        DashManager = GetComponent<DashScript>();
    }

    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        AnimHashes.Add(Animator.StringToHash("Base.Idle"), "Idle");
        AnimHashes.Add(Animator.StringToHash("Base.WalkOrRun"), "WalkOrRun");

        AnimHashes.Add(Animator.StringToHash("WeakAttack.RightWeakAttack2"), "RightWeakAttack2");
        AnimHashes.Add(Animator.StringToHash("WeakAttack.LeftWeakAttack2"), "LeftWeakAttack2");
        AnimHashes.Add(Animator.StringToHash("WeakAttack.RightWeakAttack1"), "RightWeakAttack1");
        AnimHashes.Add(Animator.StringToHash("WeakAttack.LeftWeakAttack1"), "LeftWeakAttack1");

        AnimHashes.Add(Animator.StringToHash("MediumAttack.LeftMediumAttack2"), "LeftMediumAttack2");
        AnimHashes.Add(Animator.StringToHash("MediumAttack.LeftMediumAttack1"), "LeftMediumAttack1");
        AnimHashes.Add(Animator.StringToHash("MediumAttack.RightMediumAttack2"), "RightMediumAttack2");
        AnimHashes.Add(Animator.StringToHash("MediumAttack.RightMediumAttack1"), "RightMediumAttack1");

        Life = MaxLife;
        Time.timeScale = 1;

        if (CharAnimator == null)
        {
            CharAnimator = GetComponentInChildren<Animator>();
        }
        CurrentCharState = CharacterMoveState.Classic;

        var joystickMargin = 0.05f * Screen.width;
        var joystickSize = new Rect(joystickMargin, joystickMargin, Screen.height * 0.3f, Screen.height * 0.3f);

        GameObject.Find("JoystickBackground").GetComponent<GUITexture>().pixelInset = joystickSize;

        var controllerJoystickSize = new Rect
        {
            x = joystickSize.x + (0.1f * joystickSize.width),
            y = joystickSize.y + (0.1f * joystickSize.height),
            width = joystickSize.width * 0.8f,
            height = joystickSize.height * 0.8f
        };

        var joystickControllerObj = GameObject.Find("JoystickController");

        joystickControllerObj.GetComponent<GUITexture>().pixelInset = controllerJoystickSize;
        var joystickScript = joystickControllerObj.GetComponent<Joystick>();
        joystickScript.enabled = true;

    }

    // ReSharper disable once UnusedMember.Local
    private void FixedUpdate()
    {
        if (!KoManager.Ko && !GettingHit && !Attacking && CanMove)
        {
            //SetPlayerPosition();
            switch (CurrentCharState)
            {
                case CharacterMoveState.Jumping:
                    UpdateJumpMovement();
                    break;

                case CharacterMoveState.Flying:

                    var impactPoint = CharacterSight.Instance.GetImpactPoint();
                    var dir = impactPoint != Vector3.zero ? -(transform.position - impactPoint).normalized : transform.forward;

                    SetPlayerVelocity(dir, false);
                    UpdateFlyMovement();
                    break;

                case CharacterMoveState.Falling:
                    GetComponent<Rigidbody>().useGravity = true;
                    if (Grounded)
                    {
                        CurrentCharState = CharacterMoveState.Classic;
                        GetComponent<AudioSource>().PlayOneShot(AudioController.AudioClips["grounded"]);
                    }
                    break;

                case CharacterMoveState.Classic:

                    SetPlayerVelocity(transform.forward);
                    break;
            }
        }

        RunAudioSourceActivation(); // En dehors du test car il faut pouvoir désactiver le son.
        FlyAudioSourceActivation();
    }

    protected override void Update()
    {
        base.Update();

        if (Loose || Win)
        {
            return;
        }

        CheckChargeState();
        CheckEndOfAnimation();
        GetInputMovement();
        SetAnimatorParams();

        if (EnergyBarImage != null)
        {
            EnergyBarImage.fillAmount = Ki / MaxKi;
        }

        if (LifeBarImage != null)
        {
            LifeBarImage.fillAmount = Life / MaxLife;
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void LateUpdate()
    {
        LoadKi();
        ManageAttackQueue();
    }

    private void CancelMyAttack()
    {
        Attacking = false;

        if (LoadingSkill)
        {
            switch (CurrentSkill)
            {
                case SkillType.Kamehameha:
                    KamehamehaManager.CancelKame();
                    break;
                default:
                    Debug.LogError("Je connais pas cette technique : " + CurrentSkill.ToString());
                    CancelCharge();
                    break;
            }
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void OnCollisionEnter(Collision collision)
    {
        CancelMyAttack();
        switch (collision.transform.tag)
        {
            case "Blast":
                {
                    // Pas besoin d'une autre explosion ici car cela est déjà implémenté dans les scripts de blast

                    //var explosion = Resources.Load<GameObject>("CFXPrefabs_Mobile/Explosions/CFXM_SmokeExplosion");
                    //Instantiate(explosion, collision.transform.position, Quaternion.identity);
                    Destroy(collision.transform.gameObject);
                    if (!KoManager.Ko)
                    {
                        CharAnimator.SetTrigger("blast_hit");
						GetComponent<Rigidbody>().AddForce(-collision.transform.forward * 500);
                    }
                    Life -= 25f;
                }
                break;
        }

    }

    //// ReSharper disable once UnusedMember.Local
    //private void OnParticleCollision(GameObject other)
    //{
    //    CancelMyAttack();
    //    switch (other.tag)
    //    {
    //        case "Fireball":
    //            {
    //                var dir = (transform.position - other.transform.position).normalized;
    //                dir.y = 2f;
    //                rigidbody.AddForce(dir * 200);
    //                StartCoroutine(ReceiveKo());
    //                Life -= 150f;
    //            }
    //            break;
    //    }
    //}

    #endregion

    #region UPDATE MOVEMENTS

    /// <summary>
    /// Permet d'activer ou de désactiver le son des bruits de pas en fonction de la vitesse et de la détection du sol
    /// </summary>
    private void RunAudioSourceActivation()
    {
        if (_currentSpeed > 0.5f && Grounded && CanMove && !DashManager.IsDashing && RunAudioSource != null)
        {

            RunAudioSource.pitch = _currentSpeed;

            if (!RunAudioSource.isPlaying)
            {
                RunAudioSource.Play();
            }
        }
        else
        {
            if (RunAudioSource != null)
                if (RunAudioSource.isPlaying)
                {
                    RunAudioSource.Stop();
                }
        }
    }

    private void FlyAudioSourceActivation()
    {
        if (CurrentCharState == CharacterMoveState.Flying && _currentSpeed > 0 && !Grounded && CanMove && !DashManager.IsDashing)
        {
            if (FlyAudioSource != null && !FlyAudioSource.isPlaying)
            {
                AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/pre_fly"), transform.position);
                FlyAudioSource.Play();
            }
        }
        else
        {
            if (FlyAudioSource != null && FlyAudioSource.isPlaying)
            {
                FlyAudioSource.Stop();
            }
        }
    }

    private void UpdateJumpMovement()
    {
        if (!Grounded)
        {
            _hasJumped = true;
        }
        else if (_hasJumped)
        {
            _hasJumped = false;
            GetComponent<AudioSource>().PlayOneShot(AudioController.AudioClips["grounded"]);
            CurrentCharState = CharacterMoveState.Classic;
        }
    }

    private void UpdateFlyMovement()
    {
        if (Grounded || !CanFly)
        {
            GetComponent<Rigidbody>().useGravity = true;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            CurrentCharState = CharacterMoveState.Classic;
        }
    }

    private void SetPlayerVelocity(Vector3 playerDirection, bool withGravity = true)
    {
        if (Math.Abs(InputMovement.x) < float.Epsilon && Math.Abs(InputMovement.y) < float.Epsilon)
        {
            if (withGravity)
            {
                if (_moveBack)
                {
                    GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
                }
                else
                {
                    // (OrbitCameraScript != null && OrbitCameraScript.IsRotating)
                    if (GetComponent<Rigidbody>() != null && GetComponent<Rigidbody>().velocity != Vector3.zero)
                    {
                        GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
                    }
                }
            }
            else
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        else
        {
            if (CurrentCharState != CharacterMoveState.Classic || Grounded)
            {
                if (Math.Abs(InputMovement.y) > float.Epsilon)
                {
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    if (InputMovement.y > 0)
                    {
                        if (!AutoFocus.AutoFocus || (AutoFocus.AutoFocus && !CloseCombat))
                        {
                            var accelerationForce = playerDirection * AccelerationSpeed * 1000 * Time.deltaTime;
                            GetComponent<Rigidbody>().AddForce(accelerationForce);
                            _moveBack = false;
                        }
                    }
                    else
                    {
                        _moveBack = true;
                        var accelerationForce = -playerDirection * AccelerationSpeed * 500 * Time.deltaTime;
                        GetComponent<Rigidbody>().AddForce(accelerationForce);
                    }
                }

                // Pas de côté (déplacement latéral)

                //if (!AutoFocus.AutoFocus)
                //{
                //    if (Math.Abs(InputMovement.x) > float.Epsilon)
                //    {
                //        if (InputMovement.x > 0)
                //        {
                //            rigidbody.AddForce(transform.right * AccelerationSpeed);
                //        }
                //        else
                //        {
                //            rigidbody.AddForce(-transform.right * AccelerationSpeed);
                //        }
                //    }
                //}

            }

            var clamp = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, MaxMagnitudeSpeed);

            GetComponent<Rigidbody>().velocity = new Vector3(clamp.x, withGravity ? GetComponent<Rigidbody>().velocity.y : 0, clamp.z);
        }
    }

    #endregion

    #region ANIMATION METHODS

    /// <summary>
    /// Cette méthode permet de passer les valeurs à l'animator (utiles pour déclencher les animations).
    /// </summary>
    private void SetAnimatorParams()
    {
        if (!CanMove)
        {
            InputMovement = Vector2.zero;
            _currentSpeed = 0;
            CharAnimator.SetBool("move_back", false);
        }
        else
        {
            _currentSpeed = (float)Math.Round((GetComponent<Rigidbody>().velocity.magnitude / MaxMagnitudeSpeed), 1);
            CharAnimator.SetBool("move_back", _moveBack);
        }

        CharAnimator.SetFloat("speed", _currentSpeed);
        CharAnimator.SetBool("move_back", InputMovement.y < 0);
        CharAnimator.SetFloat("rotation", InputMovement.x);
        CharAnimator.SetBool("flying", CurrentCharState == CharacterMoveState.Flying);
    }

    /// <summary>
    /// Permet de retourner le nom d'une animation grâce à son hash id
    /// </summary>
    private string GetAnimName(int hash)
    {
        return AnimHashes.ContainsKey(hash) ? AnimHashes[hash] : hash.ToString();
    }

    /// <summary>
    /// Cette méthode permet de détecter le changement et la fin d'une animation (lorsque l'on est en mode combat). Ainsi on peut repasser le booléen Attacking à 'false'.
    /// </summary>
    private void CheckEndOfAnimation()
    {
        var state = CharAnimator.GetCurrentAnimatorStateInfo(0);

        //Debug.Log(GetAnimName(state.nameHash));

        if (Attacking)
        {
            if (!_checkEndAnim)
            {
                if (_currentAttackState != state.nameHash)
                {
                    Debug.Log(string.Format("On passe de {0} a {1}. RightAttack = {2}", GetAnimName(_currentAttackState), GetAnimName(state.nameHash), RightAttack));
                    _currentAttackState = state.nameHash;
                    _checkEndAnim = true;
                }
            }
            else
            {
                if (_currentAttackState == state.nameHash)
                {
                    if (state.normalizedTime >= 1f && !CharAnimator.IsInTransition(0)
                        && (_currentAnimationTrigger != string.Empty && CharAnimator.GetBool(_currentAnimationTrigger)))
                    {
                        if (_currentAttackTrail != null)
                        {
                            _currentAttackTrail.transform.parent = null;
                        }
                        Attacking = false;
                        _checkEndAnim = false;
                        _currentAnimationTrigger = string.Empty;
                        RightAttack = !RightAttack;
                    }
                }
                else if (!ChargeActivated)
                {
                    Debug.Log(string.Format("L'animation a changé avant la fin ({0}). On passe de {1} a {2}. RightAttack = {3}",
                        state.normalizedTime.ToString(), GetAnimName(_currentAttackState), GetAnimName(state.nameHash), RightAttack));

                    if (_currentAttackTrail != null)
                    {
                        _currentAttackTrail.transform.parent = null;
                    }
                    _currentAttackState = state.nameHash;
                    Attacking = false;
                    _checkEndAnim = false;
                    _currentAnimationTrigger = string.Empty;
                    RightAttack = !RightAttack;
                }
            }

            //var state = CharAnimator.GetCurrentAnimatorStateInfo(0);
            //if (state.normalizedTime > 1)
            //{
            //    // Debug.Log("On change l'état \"Attacking\" automatiquement suite à l'arret de l'animation.");
            //    Attacking = false;
            //}
        }
        else
        {
            _currentAttackState = state.nameHash;
        }
    }

    #endregion

    //IEnumerator WaitAndDestroyAttackTrail(GameObject currentAttackTrail)
    //{
    //    if (currentAttackTrail != null)
    //    {
    //        currentAttackTrail.transform.parent = null;
    //        yield return new WaitForSeconds(5f);
    //        Destroy(currentAttackTrail);
    //    }
    //}

    private void CheckChargeState()
    {
        if (ChargeActivated)
        {
            if (AutoFocus.AutoFocus)
            {
                AutoFocus.AutoFocusCamera.enabled = false;
            }

            var sec = Time.time - _chargeButtonTime;
            if (sec >= MaxChargeTime)
            {
                AButtonLoader.value = 1;
                ReleaseAButton();
            }
            else
            {
                AButtonLoader.value = sec / MaxChargeTime;
            }
        }
    }

    /// <summary>
    /// Cette méthode permet de récupérer les valeures d'entrée pour le déplacement (avec différents devices).
    /// </summary>
    private void GetInputMovement()
    {
        InputMovement = Vector2.zero;

#if UNITY_EDITOR
        InputMovement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#else
        var joystickScript = FindObjectOfType<Joystick>();
        if (joystickScript != null)
        {

            if (Mathf.Abs(joystickScript.position.x) <= 0.6f)
            {
                InputMovement.x = 0;
            }
            else
            {
                InputMovement.x = joystickScript.position.x;
            }

            if (Mathf.Abs(joystickScript.position.y) <= 0.15f)
            {
                InputMovement.y = 0;
            }
            else
            {
                InputMovement.y = joystickScript.position.y;
            }

        }
#endif
    }

    /// <summary>
    /// Cette méthode est appelée par l'animation pour vérifier si on est en train de charger une attaque ou non. Dans le cas d'une charge, on stoppe l'avancement de
    /// l'animation et on attend le 'release' du boutton.
    /// </summary>
    public void CheckAttackCharge()
    {
        if (ChargeActivated)
        {
            Debug.Log("ChargeActivated");
            CharAnimator.speed = 0;
        }
    }

    /// <summary>
    /// Cette méthode permet de recharger son énergie
    /// </summary>
    public void LoadKi()
    {
        if (Ki < 100 && !DashManager.IsDashing && !LoadingSkill && !TransformationManager.IsSuperSaiyan)
        {
            if (!LoadingSkill || ChargeActivated)
            {
                Ki += (_kiMultiplicator * Time.deltaTime);
            }
        }
    }

    [Obsolete]
    private string GetSkillName(bool rightAttack, bool punch, AttackTypes attackType)
    {
        var skillName = rightAttack ? "right_" : "left_";
        switch (attackType)
        {
            case AttackTypes.Weak:
                skillName += "weak_attack";
                break;
            case AttackTypes.Medium:
                skillName += "medium_attack";
                break;
            case AttackTypes.Strong:
                skillName += "strong_attack";
                break;
        }

        skillName += punch ? 1 : 2;

        return skillName;

    }

    /// <summary>
    /// Applique le résultat de l'attaque à l'adverssaire. 
    /// </summary>
    /// <param name="skill">Nom de l'attaque réalisée</param>
    public void ReleaseAttack(string skill)
    {
        var sec = Time.time - _chargeButtonTime;

        var tdm = new TakeDamageModel
        {
            Default = false,
            SkillName = skill,
            Emitter = transform,
            RightAttack = RightAttack,
            CanDodge = true,
            AttackMultiplicator = 1 + (sec / MaxChargeTime)
        };

        AudioClip clip;
        switch (skill)
        {

            case "combo_punch":
                tdm.AttackTypes = AttackTypes.Combo;
                tdm.NBAttack = NBAttack.Punch;

				var audioClip = AudioController.AudioClips["hit_combo1"];
				if(audioClip != null && GetComponent<AudioSource>() != null)
				{
					GetComponent<AudioSource>().PlayOneShot(audioClip);
				}

                GiveDamage(tdm);
                break;

            case "weak_attack_punch":
                tdm.AttackTypes = AttackTypes.Weak;
                tdm.NBAttack = NBAttack.Punch;

                clip = (sec >= (MaxChargeTime / 2) && ChargeActivated) ? AudioController.AudioClips["fat_punch"] : AudioController.AudioClips["weak_punch"];
                GetComponent<AudioSource>().PlayOneShot(clip);

                GiveDamage(tdm);
                break;

            case "weak_attack_kick":
                tdm.AttackTypes = AttackTypes.Weak;
                tdm.NBAttack = NBAttack.Kick;

                clip = (sec >= (MaxChargeTime / 2) && ChargeActivated) ? AudioController.AudioClips["fat_kick"] : AudioController.AudioClips["weak_kick"];
                GetComponent<AudioSource>().PlayOneShot(clip);

                GiveDamage(tdm);
                break;

            case "medium_attack_kick":
                tdm.AttackTypes = AttackTypes.Medium;
                tdm.NBAttack = NBAttack.Kick;
                clip = (sec >= (MaxChargeTime / 2) && ChargeActivated) ? AudioController.AudioClips["fat_kick"] : AudioController.AudioClips["weak_kick"];
                GetComponent<AudioSource>().PlayOneShot(clip);
                GiveDamage(tdm);
                break;

            case "medium_attack_punch":
                tdm.AttackTypes = AttackTypes.Medium;
                tdm.NBAttack = NBAttack.Punch;
                clip = (sec >= (MaxChargeTime / 2) && ChargeActivated) ? AudioController.AudioClips["fat_punch"] : AudioController.AudioClips["weak_punch"];
                GetComponent<AudioSource>().PlayOneShot(clip);
                GiveDamage(tdm);
                break;

            default:
                GetComponent<AudioSource>().PlayOneShot(AudioController.AudioClips["fat_kick"]);
                GiveDamage(skill);
                break;
        }
    }

    /// <summary>
    /// Permet d'envoyer à 'FocusEnemy' l'attaque qu'il va subir (en passant en paramètre le nom de l'attaque).
    /// </summary>
    public void GiveDamage(string skill)
    {
        if (FocusEnemy != null && CloseCombat)
        {
            FocusEnemy.SendMessage("TakeDamage", skill);
        }
    }

    /// <summary>
    /// Permet d'envoyer à 'FocusEnemy' l'attaque qu'il va subir (en passant en paramètre l'objet TakeDamageModel).
    /// </summary>
    /// <param name="model">Objet qui décrit l'attaque réalisée</param>
    /// <param name="restrictToCloseCombat">Est-ce que l'attaque ne peut être réalisée uniquement si on est au corp à corp.</param>
    public void GiveDamage(TakeDamageModel model, bool restrictToCloseCombat = true)
    {
        if (FocusEnemy != null && (!restrictToCloseCombat || CloseCombat))
        {
            FocusEnemy.SendMessage("ApplyDamage", model);
        }
    }

    /// <summary>
    /// Lance le chargement d'une attaque.
    /// </summary>
    /// <param name="skillName">Nom de l'attaque réalisée</param>
    public void ChargeAttack(string skillName)
    {
        if (AutoFocus.AutoFocus && CloseCombat)
        {
            var audioScreamObj = GameObject.Find("AudioScream");
            if (audioScreamObj != null)
            {
                var audioSource = GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }

            var audioAttackChargeObj = GameObject.Find("AudioAttackCharge");
            if (audioAttackChargeObj != null)
            {
                var audioSource = GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }

            AButtonLoader.gameObject.SetActive(true);
            _chargeButtonTime = Time.time;
            _skillCharged = skillName;
            ChargeActivated = true;
            DamageManager.DesactivateParticle = false;
        }
        else
        {
            ReleaseAttack(skillName);
        }
    }

    /// <summary>
    /// Méthode appellée par l'évènement 'PointUp' du bouton A.
    /// </summary>
    public void ReleaseAButton()
    {
        _btnClicked = false;
        if (ChargeActivated)
        {
            ReleaseAttack(_skillCharged);
            StopAttackCharge();
        }
    }

    /// <summary>
    /// Méthode qui permet de mettre dans une file d'attente les différentes instructions de combat (touches/boutons appuyées).
    /// </summary>
    /// <param name="key">Charactère qui correspond à la dernière touche / bouton appuyé</param>
    public void EnqueueKeyPressed(char key)
    {
        // Si il y a plus d'une seconde qui s'écoule entre l'appuie sur deux boutons pour activer un combo on réinitialise l'historique des touches (combo).
        if (Time.time - _comboTimer > 4)
        {
            _comboHistory = string.Empty;
        }

        const int maxQueueCount = 4;

        var tempQueue = new Queue<char>();
        if (_comboQueue.Count >= maxQueueCount)
        {
            // Si l'utilisateur a spammé plus de x coups, alors on recréé la queue en remplacant le 5e coup par le nouveau coup.
            for (int i = 0; i < maxQueueCount - 1; i++)
            {
                var charAttack = _comboQueue.Dequeue();
                tempQueue.Enqueue(charAttack);
            }
            _comboQueue.Enqueue(key);

            if (_comboHistory.Length == maxQueueCount)
            {
                _comboHistory = _comboHistory.Substring(0, _comboHistory.Length - 2) + key;
            }
        }
        else
        {
            _comboHistory += key;
        }

        _comboTimer = Time.time;
        _comboQueue.Enqueue(key);

    }

    /// <summary>
    /// Permet d'exposer la fonction ManageKeysPressed aux évènements OnClick des boutons uGUI. (le char n'étant pas compris...)
    /// </summary>
    public void ManageKeysPressed(string keyPressed)
    {
        ManageKeysPressed(keyPressed[0]);
    }

    public void StopVelocity(bool withYAxe = false)
    {
        InputMovement = Vector2.zero;
        GetComponent<Rigidbody>().velocity = new Vector3(0, withYAxe ? 0 : GetComponent<Rigidbody>().velocity.y, 0);
    }

    /// <summary>
    /// Méthode qui permet de traiter la file d'attente des attaques à réaliser. Cette méthode permet également de détecter les combos de coups.
    /// </summary>
    public void ManageAttackQueue()
    {

#if UNITY_EDITOR

        string comboKeys = _comboQueue.Aggregate("", (current, key) => current + key);
        if (ComboText != null)
        {
            ComboText.text = comboKeys;
        }

        if (ComboHistory != null)
        {
            ComboHistory.text = _comboHistory;
        }

#endif

        if (CanAttack && !LoadingSkill && !GettingHit)           //  && CurrentCharState != CharacterMoveState.Falling && CurrentCharState != CharacterMoveState.Jumping
        {
            if (_comboQueue.Count > 0)
            {
                if (!Attacking)
                {
                    Attacking = true;
                    var combo = false;
                    var key = _comboQueue.Dequeue();

                    if (_currentSpeed >= 0 && !key.Equals('C')) // On vérifie que l'on est pas dans le cas d'un saut
                    {
                        StopVelocity();
                    }

                    switch (key)
                    {
                        case 'B':

                            //switch (_comboHistory)
                            //{
                            //    //case "AAAAB":
                            //    //case "AAAAAB":
                            //    //    _comboHistory = string.Empty;
                            //    //    // combo = true;
                            //    //    break;
                            //}

                            if (!combo && !CloseCombat)
                            {
                                KiBlastManager.LaunchKiBlast();
                            }
                            else if (CloseCombat && !combo)
                            {
                                KiaiManager.DoKiai();
                            }

                            break;
                        case 'C':

                            Debug.Log("_comboHistory:" + _comboHistory);
                            switch (_comboHistory)
                            {
                                case "AAC":
                                    _comboHistory = string.Empty;
                                    combo = true;
                                    ThrowManager.StartSwing();
                                    break;

                                case "AAAC":
                                    _comboHistory = string.Empty;
                                    combo = true;
                                    TeleportationManager.StartFirstCombo();
                                    break;
                            }

                            if (!combo)
                            {
                                // Le saut est prioritaire par rapport aux attaques.
                                Attacking = false;
                                _comboQueue.Clear();
                                _comboHistory = string.Empty;
                                Jump();
                            }

                            break;

                        case 'A':

                            //if (!Grounded)
                            //{
                            //    this.transform.rigidbody.AddForce(Vector3.down * 1000f);
                            //    return;
                            //}

                            if (!CloseCombat)
                            {
								StopVelocity(false);
                                // Ce test permet de faire avancer le personnage lorsqu'il donne des coups (uniquement si il n'est pas déja au corp à corp).
                                GetComponent<Rigidbody>().AddForce(transform.forward * 400);
                            }
                            else if (!Grounded)
                            {
                                // Ce test permet de gérer le cas de figure où on attaque cell en autofocus en étant au dessus de lui à l'horizontale (en mode vol)
                                // Dans ce cas, on rétablit la gravité pour permettre de revenir au sol aprés l'attaque
                                CurrentCharState = CharacterMoveState.Falling;
                            }

                            var punch = Random.value > 0.5f;

                            string skillName = "";
                            switch (_comboHistory)
                            {
                                case "AAAAAA":
                                    skillName = "combo_punch";
                                    combo = true;
                                    break;
                            }

                            if (combo)
                            {
                                _comboHistory = string.Empty;
                            }
                            else
                            {
                                if (_comboHistory.Length > 2)
                                {
                                    // medium_attack
                                    skillName = punch ? "medium_attack_punch" : "medium_attack_kick";
                                }
                                else
                                {
                                    // weak_attack
                                    skillName = punch ? "weak_attack_punch" : "weak_attack_kick";
                                }

                                // Debug.Log(skillName);

                                if (punch)
                                {
                                    if (!RightAttack)
                                    {
                                        _currentAttackTrail =
                                            (GameObject)
                                                Instantiate(FootTrail, DamageManager.RightHand.position,
                                                    Quaternion.identity);
                                        _currentAttackTrail.transform.parent = DamageManager.RightHand;
                                    }
                                    else
                                    {
                                        _currentAttackTrail =
                                            (GameObject)
                                                Instantiate(FootTrail, DamageManager.LeftHand.position,
                                                    Quaternion.identity);
                                        _currentAttackTrail.transform.parent = DamageManager.LeftHand;
                                    }
                                }
                                else
                                {
                                    if (!RightAttack)
                                    {
                                        _currentAttackTrail =
                                            (GameObject)
                                                Instantiate(FootTrail, DamageManager.RightFoot.position,
                                                    Quaternion.identity);
                                        _currentAttackTrail.transform.parent = DamageManager.RightFoot;
                                    }
                                    else
                                    {
                                        if (skillName.Equals("medium_attack_kick")) // Rustine pour le coup de Goku
                                        {
                                            _currentAttackTrail =
                                                (GameObject)
                                                    Instantiate(FootTrail, DamageManager.RightFoot.position,
                                                        Quaternion.identity);
                                            _currentAttackTrail.transform.parent = DamageManager.RightFoot;
                                        }
                                        else
                                        {
                                            _currentAttackTrail =
                                                (GameObject)
                                                    Instantiate(FootTrail, DamageManager.LeftFoot.position,
                                                        Quaternion.identity);
                                            _currentAttackTrail.transform.parent = DamageManager.LeftFoot;
                                        }
                                    }
                                }

                            }

                            _currentAnimationTrigger = skillName;
                            CharAnimator.SetTrigger(skillName);

                            //Debug.Log("CharAnimator.SetTrigger(" + _currentAnimationTrigger + ") / right : " + RightAttack);

                            if (combo || _comboQueue.Count != 0 || !_btnClicked)
                            {
                                ReleaseAttack(skillName);
                            }
                            else
                            {
                                ChargeAttack(skillName);
                            }

                            break;
                    }
                }
            }
        }
        else
        {
            _comboQueue.Clear();
        }
    }

	public void ClearCombo()
	{
		_comboHistory = string.Empty;
		_comboQueue.Clear ();
	}

    /// <summary>
    /// Méthode qui est appellée à la suite d'un click 'PointerDown' sur un bouton.
    /// </summary>
    /// <param name="keyPressed">Caractère qui correspond au bouton sélectionnée</param>
    public void ManageKeysPressed(char keyPressed)
    {
        _btnClicked = true;
        if (!CanAttack || LoadingSkill || GettingHit)
        {
            _comboQueue.Clear();
        }
        else
        {
            EnqueueKeyPressed(keyPressed);
        }
    }

    /// <summary>
    /// Désactivation de la charge d'un coup
    /// </summary>
    public void StopAttackCharge()
    {
        ChargeActivated = false;
        _skillCharged = string.Empty;

        // Desactivation des bruits de charge (attaque)

        var audioObj = GameObject.Find("AudioScream");
        if (audioObj != null)
        {
            var audioSource = audioObj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }

        audioObj = GameObject.Find("AudioAttackCharge");
        if (audioObj != null)
        {
            var audioSource = audioObj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }

        if (AutoFocus.AutoFocus)
        {
            AutoFocus.AutoFocusCamera.enabled = true;
        }

        DamageManager.DesactivateParticle = true;
        AButtonLoader.gameObject.SetActive(false);
        CharAnimator.speed = 1;
    }

    /// <summary>
    /// Méthode qui gère les coups/attaque que subit notre personnage (permet de lancer nottament les animations associées).
    /// </summary>
    /// <param name="skill">Nom de l'attaque reçue</param>
    /// <param name="damage">Valeur des dégats occasionnés</param>
    /// <param name="enemy">L'objet 3D qui a émit l'attaque</param>
    public void TakeDamage(string skill, float damage, Transform enemy)
    {
        Life -= damage;

        if (!KoManager.Ko)
        {
            GettingHit = true;

            StopAllCoroutines();

            if (!AutoFocus.AutoFocus)
            {
                AutoFocus.AutoFocus = true;
            }

            if (ChargeActivated)
            {
                StopAttackCharge();
            }

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            FocusEnemy = enemy;

            CancelMyAttack();

            switch (skill)
            {
                case "combo1":
                    ActivateEndOfHitAnimationTrigger = true;
                    CharAnimator.Play("Hit Combo1");
                    break;
                case "combo2":
                    StartCoroutine(TakeCombo2(-enemy.forward));
                    break;

                case "right_weak_attack2":
                case "right_weak_attack1":
                    ActivateEndOfHitAnimationTrigger = true;
                    {
                        var hit = Resources.Load("CFXPrefabs_Mobile/Hits/CFXM_Hit_C White") as GameObject;
                        Instantiate(hit, transform.position, Quaternion.identity);
                    }
                    CharAnimator.SetTrigger("hit_weak_attack");
                    CharAnimator.SetBool("hit_right_attack", true);
                    break;

                case "left_weak_attack1":
                case "left_weak_attack2":
                    ActivateEndOfHitAnimationTrigger = true;
                    {
                        var hit = Resources.Load("CFXPrefabs_Mobile/Hits/CFXM_Hit_C White") as GameObject;
                        Instantiate(hit, transform.position, Quaternion.identity);
                    }
                    CharAnimator.SetTrigger("hit_weak_attack");
                    CharAnimator.SetBool("hit_right_attack", false);
                    break;

                case "top_weak_attack2":
                    ActivateEndOfHitAnimationTrigger = true;
                    {
                        var hit = Resources.Load("CFXPrefabs_Mobile/Hits/CFXM_Hit_C White") as GameObject;
                        Instantiate(hit, transform.position, Quaternion.identity);
                    }
                    CharAnimator.Play("TopWeakAttackHit");
                    break;

                case "saibaiman_explosion":
                    ActivateEndOfHitAnimationTrigger = true;
                    CharAnimator.SetTrigger("blast_hit");
                    break;
                default:
                    Debug.LogError(string.Format("La technique {0} n'est pas reconnue.", skill));
                    GettingHit = false;
                    break;
            }
        }
    }

    public override void ApplyDamage(TakeDamageModel model)
    {
        base.ApplyDamage(model);
        if (model.Default)
        {
            switch (model.SkillName)
            {
                case "kamehameha":
                    var dir = (transform.position - model.Emitter.position).normalized;
                    dir.y = 2f;
                    GetComponent<Rigidbody>().AddForce(dir * 200);
                    StartCoroutine(ReceiveKo());
                    Life -= 150f;
                    break;

                default:
                    Debug.LogError(string.Format("Technque inconnue: {0}", model.SkillName));
                    break;
            }
        }
    }

    private IEnumerator TakeCombo2(Vector3 direction)
    {
        CharAnimator.Play("Hit Combo2");
        yield return new WaitForSeconds(0.8f);
        transform.rotation = Quaternion.LookRotation(direction);

        var force = (direction + Vector3.up) * 400f;
        GetComponent<Rigidbody>().AddForce(force);

        yield return StartCoroutine(ReceiveKo());

        GettingHit = false;
    }

    /// <summary>
    /// Permet de stoper la capacité "charge d'énergie".
    /// </summary>
    public void CancelCharge()
    {
        CharAnimator.SetBool("loading_ki", false);
        if (FX_power_up_dust != null && FX_power_up_dust.isPlaying)
        {
            FX_power_up_dust.Stop();
        }

        if (KiChargeAudioSource != null && KiChargeAudioSource.isPlaying)
        {
            KiChargeAudioSource.Stop();
        }

        _kiMultiplicator = 1;
        CurrentSkill = null;

        // On stoppe l'aura
        foreach (Transform obj in GameObject.Find("aura2").transform)
        {
            obj.gameObject.SetActive(false);
        }

        Camera.main.gameObject.SetActive(true);
        CanMove = true;
    }

    /// <summary>
    /// Permet d'activer la capacité "charge d'énergie"
    /// </summary>
    public void ChargeKi()
    {
        CanMove = false;
        CurrentSkill = SkillType.KiCharge;
        CharAnimator.SetBool("loading_ki", true);

        //if (!KiChargeAudioSource.isPlaying)
        //{
        //    KiChargeAudioSource.Play();
        //}

        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/aura"));

        _kiMultiplicator = 20;

        // On active l'aura
        foreach (Transform obj in GameObject.Find("aura2").transform)
        {
            if ((obj.gameObject.layer == 12 && Grounded) || obj.gameObject.layer != 12)
                obj.gameObject.SetActive(true);
        }

        // On bouge la camera
        // Camera.main.GetComponent<OrbitCamera>().Shake();
    }

    private IEnumerator ReceiveKo()
    {
        AutoFocus.enabled = false;
        KoManager.Ko = true;
        if (!Grounded)
        {
            if (CurrentCharState == CharacterMoveState.Flying)
            {
                GetComponent<Rigidbody>().useGravity = true;
            }
        }
        while (!Grounded)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Méthode qui est activée à la mort de notre personnage
    /// </summary>
    public override void Die()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            enemy.StopAllCoroutines();
            enemy.CanMove = false;
            enemy.CanAttack = false;
        }
        CharAnimator.SetBool("die", true);
        Time.timeScale = 0;
        Loose = true;
    }

    /*
private void ManageViewFinder (Vector2 inputMovement)
{

    var movement = new Vector3 (inputMovement.x, inputMovement.y, 0);
    movement *= 10;


    var viewFinderScreenPos = Camera.main.WorldToScreenPoint (ViewFinder.position);
    var newScreenPos = viewFinderScreenPos + movement;


    if (newScreenPos.x <= _maxScreenXY.x && newScreenPos.x >= _minScreenXY.x
            && newScreenPos.y <= _maxScreenXY.y && newScreenPos.y >= _minScreenXY.y) {

            ViewFinder.position = Camera.main.ScreenToWorldPoint (newScreenPos);

    } 

}
*/
    /// <summary>
    /// Méthode qui gère les différents états suite à un saut (saut, vol, chute).
    /// </summary>
    public void Jump()
    {
        switch (CurrentCharState)
        {
            case CharacterMoveState.Flying:
                CurrentCharState = CharacterMoveState.Falling;
                break;

            case CharacterMoveState.Jumping:

                if (!Grounded && CanFly)
                {
                    _hasJumped = false;
                    GetComponent<Rigidbody>().useGravity = false;
                    GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0, GetComponent<Rigidbody>().velocity.z);
                    CurrentCharState = CharacterMoveState.Flying;
                }
                break;

            default:

                if (Grounded)
                {
                    CurrentCharState = CharacterMoveState.Jumping;
                    GetComponent<Rigidbody>().AddForce(new Vector3(0, JumpHeight, 0));
                    CharAnimator.Play("Jump");
                    GetComponent<AudioSource>().PlayOneShot(AudioController.AudioClips["jump"]);
                }
                else
                {
                    GetComponent<Rigidbody>().useGravity = false;
                    GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0, GetComponent<Rigidbody>().velocity.z);
                    CurrentCharState = CharacterMoveState.Flying;
                }

                break;
        }
    }

    #region Generic Beam

    public void ChargeGenericBeam()
    {
        if (!LoadingSkill && Ki >= 20)
        {
            CurrentSkill = SkillType.GenericBeam;

            //_loadingGBeam = true;

            CharAnimator.SetBool("gbeam_charge", true);

            // On play les animations de particules
            foreach (var particle in l_kame_effects)
            {
                particle.GetComponent<ParticleSystem>().Play();
            }

            // SON A REMETTRE QUAND TU AURAS REFACTORISE

            //if (!KameChargeAudioSource.isPlaying)
            //{
            //    KameChargeAudioSource.Play();
            //}
        }
    }

    #endregion


    //Juste pour les tests 
    public void ReloadScene()
    {
        Application.LoadLevelAsync("cell_game_scene");
    }
}

