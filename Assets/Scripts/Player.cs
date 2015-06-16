using UnityEngine;

public abstract class Player : MonoBehaviour
{

	#region Public properties

    public LayerMask EnemyLayerMask;
	public LayerMask GroundLayer;
	public AudioClip[] PainAudioClips;
    public bool CanAttack = true;
    public bool CanMove = true;
	public bool CanFly = true;
	public bool IsMyPlayer = false;
	public bool DisplayLifeBar = false;
    public Animator CharAnimator;
    public KoRecoverScript KoManager;
    public float MaxLife = 2000;
    
	[HideInInspector]
	public bool Grounded;
	[HideInInspector]
	public bool RightAttack = true;
	[HideInInspector]
	public bool Attacking = false;

    /// <summary>
    /// Hauteur qui permet de tracer un raycast pour savoir si le personnage est en contact avec le sol
    /// </summary>
    public float HeightGroundRaycast = 0.9f;

	[HideInInspector]
    /// <summary>
    /// Booléen qui permet de savoir que le joueur est en train d'être attaqué et par conséquent ne peut effectuer aucune action
    /// </summary>
    public bool GettingHit = false;

	[HideInInspector]
    /// <summary>
    /// Active la fonction qui permet de détecter la fin d'animation de réception d'un coup. 
    /// Grâce à cette détection on peut repasser GettingHit à false.
    /// </summary>
    protected bool ActivateEndOfHitAnimationTrigger = false;

    public SkillType? CurrentSkill = null;

    /// <summary>
    /// Permet de savoir si on est en train de charger une technique spéciale
    /// </summary>
    public bool LoadingSkill
    {
        get { return CurrentSkill.HasValue; }
    }

    public float Life
    {
        get { return _life; }
        set
        {
            if (value <= 0)
            {
                _life = 0;
                Die();
            }
            else if (value >= MaxLife)
            {
                _life = MaxLife;
            }
            else
            {
                _life = value;
            }
        }
    }

    public bool IsDied
    {
        get
        {
            return Life <= 0;
        }
    }

	#endregion

	#region Private properties

	private float _life;
	private int _currentHitState;
	private bool _checkEndHitAnim;

	#endregion

	#region MonoBehaviour Methods

    protected virtual void Awake()
    {
        Life = MaxLife;
        if (CharAnimator == null)
        {
            CharAnimator = transform.GetComponentInChildren<Animator>();
        }
    }

    protected virtual void Update()
    {
        Grounded = Physics.Raycast(transform.position, Vector3.down, HeightGroundRaycast, GroundLayer);
        CharAnimator.SetBool("grounded", Grounded);
        CharAnimator.SetBool("right_attack", RightAttack);

        if (ActivateEndOfHitAnimationTrigger)
        {
            CheckEndOfHitAnimation();
        }

        if (!CanMove && GettingHit)
        {
            // On ne fait plus aucune action...
            return;
        }

    }

	#endregion

	public virtual void Die()
	{
		CharAnimator.Play("Die");
	}

    /// <summary>
    /// Cette méthode permet de détecter le changement et la fin d'une animation de hit (lorsque l'on est en train de subir des coups). 
    /// Ainsi on peut repasser le booléen GettingHit à 'false'.
    /// </summary>
    private void CheckEndOfHitAnimation()
    {
        var state = CharAnimator.GetCurrentAnimatorStateInfo(0);
        if (GettingHit)
        {
            if (!_checkEndHitAnim)
            {
                if (_currentHitState != state.nameHash)
                {
                    _currentHitState = state.nameHash;
                    _checkEndHitAnim = true;
                }
            }
            else if (_currentHitState != state.nameHash || (state.normalizedTime > 1 && !CharAnimator.IsInTransition(0)))
            {
                GettingHitCompleted();
            }
        }
        else
        {
            _currentHitState = state.nameHash;
        }
    }

    public virtual void GettingHitCompleted()
    {
        ActivateEndOfHitAnimationTrigger = false;
        GettingHit = false;
        _checkEndHitAnim = false;
    }

    /// <summary>
    /// Joue un son au hasard parmis la liste des "bruitages de coups".
    /// </summary>
    public void PlayRandomPainAudioClip()
    {
        if (PainAudioClips != null && PainAudioClips.Length != 0)
        {
            var nb = Random.Range(0, PainAudioClips.Length - 1);
            var selectedClip = PainAudioClips[nb];
            if (selectedClip != null)
            {
                if (GetComponent<AudioSource>() != null && !GetComponent<AudioSource>().isPlaying)
                {
                    GetComponent<AudioSource>().PlayOneShot(selectedClip, 1);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(selectedClip, transform.position);
                }
            }
        }
    }

    public virtual void ApplyDamage(TakeDamageModel model)
    {
        if (Attacking)
        {
            Attacking = false;
        }
    }

}
