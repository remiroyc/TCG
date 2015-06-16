using System.Collections;
using UnityEngine;

public abstract class Enemy : Player, IArtificialIntelligenceAttack
{

    /// <summary>
    /// Permet de déterminer la réactivité de l'ennemi
    /// </summary>

    public bool SpecialDistanceAttackEnabled;

    [Range(1, 100)]
    public float Agility = 50f;
    [Range(1, 100)]
    public float Strength = 100f;

    public float Speed = 1.5f;
    public GameObject AuraCell;
    [HideInInspector]
    public EnemyActions CurrentAction;
    [HideInInspector]
    public Transform Target;
    public float CloseCombatDistance = 0.8f;
    public float Distance;
    public Transform LeftHand, RightHand;
    public float IdleTime = 1f;
    public GameObject TpSprite;

    protected bool CloseCombat;
    protected float BeginMoveTime;
    protected bool ActivateEndOfAnimationTrigger = false;

    private Vector3 _destinationPos;
    private int _currentAttackState;
    private bool _checkEndAnim;

    private const float MaxWaitingTimeBeforeRandomAction = 2f;

    // 0.666f est la valeure max. Car si Agility = 1, l'enemi aura au maximum 2 chance sur 3 d'esquiver.
    public float DodgeProbability { get { return 0.666f * Agility; } }

    #region MONO BEHAVIOUR METHODS

    protected override void Update()
    {
        base.Update();
        Distance = Vector3.Distance(transform.position, MyCharacterController.Instance.transform.position);
        CloseCombat = (Distance <= CloseCombatDistance);

        if (GettingHit)
        {
            // Si on est en train d'etre attaqué, on subit les coups sans pouvoir faire aucune action...
            return;
        }
    }

    protected void FixedUpdate()
    {
        //Debug.Log(CurrentAction);
        switch (CurrentAction)
        {
            case EnemyActions.MoveHandToHand:
                UpdateMoveHandToHand();
                break;

            case EnemyActions.RandomMove:
                UpdateRandomMove();
                break;

            case EnemyActions.Attack:
                UpdateAttack();
                break;
        }
    }

    #endregion

    #region AI ATTACKS IMPLEMENTATION

    #region ACTIONS UPDATE

    public virtual void UpdateAttack()
    {
        Refocus();
        if (ActivateEndOfAnimationTrigger)
        {
            CheckEndOfAttackAnimation();
        }
    }

    public virtual void UpdateMoveHandToHand()
    {
        if ((Time.time - BeginMoveTime) >= 8f)
        {
            RandomMove();
        }

        if (Grounded)
        {
            var lr = Quaternion.LookRotation(MyCharacterController.Instance.transform.position - transform.position);
            lr.eulerAngles = new Vector3(0, lr.eulerAngles.y, lr.eulerAngles.z);
            transform.rotation = lr;

            if (CloseCombat)
            {
                if (CanAttack)
                {
                    StartCoroutine(Attack());
                }
                else
                {
                    RandomMove();
                }
            }
            else
            {
                var dir = MyCharacterController.Instance.transform.position - transform.position;
                dir.y = 0;
                var move = dir.normalized * Speed * Time.fixedDeltaTime;
                CharAnimator.SetFloat("speed", 1);
                GetComponent<Rigidbody>().MovePosition(transform.position + move);
            }
        }

    }

    /// <summary>
    /// Cette méthode permet de détecter le changement et la fin d'une animation (lorsque l'on est en mode combat). Ainsi on peut repasser le booléen Attacking à 'false'.
    /// </summary>
    private void CheckEndOfAttackAnimation()
    {
        var state = CharAnimator.GetCurrentAnimatorStateInfo(0);
        if (Attacking)
        {
            if (!_checkEndAnim)
            {
                if (_currentAttackState != state.nameHash)
                {
                    _currentAttackState = state.nameHash;
                    _checkEndAnim = true;
                }
            }
            else if (_currentAttackState != state.nameHash || (state.normalizedTime > 1 && !CharAnimator.IsInTransition(0)))
            {
                ActivateEndOfAnimationTrigger = false;
                Attacking = false;
                _checkEndAnim = false;
                StartCoroutine(WaitAndRandomAction(Random.Range(0.5f, 1.5f)));
            }
        }
        else
        {
            _currentAttackState = state.nameHash;
        }
    }

    public void UpdateRandomMove()
    {
        if (Grounded)
        {
            Vector3 dir = _destinationPos - transform.position;
            dir.y = 0;

            if ((Time.time - BeginMoveTime) >= 8f || (dir.x < 0.1f && dir.z < 0.1f))
            {
                // Dés qu'on a atteint le point de destination ou qu'il s'est écoule 8 sec de marche
                // Onrecible notre personnage et on change d'action.

                var lr = Quaternion.LookRotation(MyCharacterController.Instance.transform.position - transform.position);
                lr.eulerAngles = new Vector3(0, lr.eulerAngles.y, lr.eulerAngles.z);
                transform.rotation = lr;

                RandomAction();
            }
            else
            {
                var move = dir.normalized * Speed * Time.deltaTime;
                CharAnimator.SetFloat("speed", 1);
                transform.position += move;
            }
        }
    }

    #endregion

    public virtual void Refocus()
    {
        var relativePos = MyCharacterController.Instance.transform.position - transform.position;
        var lookRot = Quaternion.LookRotation(relativePos);
        lookRot.eulerAngles = new Vector3(0, lookRot.eulerAngles.y, lookRot.eulerAngles.z);
        transform.rotation = lookRot;
    }

    public abstract void RandomAction();

    public abstract IEnumerator CastSpecialDistanceAttack();

    public IEnumerator WaitAndRandomAction(float time)
    {
        yield return new WaitForSeconds(time);
        RandomAction();
    }

    public virtual IEnumerator Idle(float time)
    {
        CharAnimator.SetFloat("speed", 0);
        CurrentAction = EnemyActions.Idle;
        yield return new WaitForSeconds(time);
        RandomAction();
    }

    public virtual IEnumerator Attack(float waitTimeBeforeAttack = 0)
    {
        if (Attacking)
        {
            yield break;
        }
        else
        {
            CharAnimator.SetFloat("speed", 0);
            if (waitTimeBeforeAttack > 0)
            {
                yield return new WaitForSeconds(waitTimeBeforeAttack);
            }
            CurrentAction = EnemyActions.Attack;
            Attacking = true;
        }

        if (CloseCombat)
        {

            if (MyCharacterController.Instance.Grounded && MyCharacterController.Instance.KoManager.Ko)
            {
                ActivateEndOfAnimationTrigger = true;
                var kickAttack = (Random.value < 0.5f);
                if (kickAttack)
                {
                    CharAnimator.SetTrigger("bottom_weak_attack_punch");
                    MyCharacterController.Instance.TakeDamage("bottom_weak_attack2", 50f, transform);
                }
                else
                {
                    CharAnimator.SetTrigger("bottom_weak_attack_kick");
                    MyCharacterController.Instance.TakeDamage("bottom_weak_attack1", 50f, transform);
                }
                GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/meleeJab2"));
            }

            if (Random.value < 0.5f)
            {
                ActivateEndOfAnimationTrigger = true;
                var kickAttack = (Random.value < 0.5f);
                if (kickAttack)
                {
                    CharAnimator.SetTrigger("weak_attack_punch");
                    MyCharacterController.Instance.TakeDamage(RightAttack ? "right_weak_attack1" : "left_weak_attack1", 50f, transform);
                }
                else
                {
                    CharAnimator.SetTrigger("weak_attack_kick");
                    MyCharacterController.Instance.TakeDamage(RightAttack ? "right_weak_attack2" : "left_weak_attack2", 50f, transform);
                }
                GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/meleeJab2"));
                RightAttack = !RightAttack;
            }
            else
            {

                var rand = Random.value;
                if (rand <= 0.33f)
                {
                    ActivateEndOfAnimationTrigger = true;
                    CharAnimator.Play("Combo1");
                    MyCharacterController.Instance.TakeDamage("combo1", 150f, transform);
                    GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/pain2"));

                    GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/mediumkick"));
                    yield return new WaitForSeconds(0.34f);
                    GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/mediumpunch"));
                    yield return new WaitForSeconds(0.4f);
                    GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/powerHit6"));
                    yield return new WaitForSeconds(0.3f);
                    GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/mediumkick"));
                }
                else if (rand <= 0.66f)
                {
                    yield return StartCoroutine(Combo2());
                }

            }
        }
        else
        {
            if (Random.value < 0.33 && SpecialDistanceAttackEnabled)
            {
                yield return StartCoroutine(CastSpecialDistanceAttack());
            }
            else
            {
                var nbBlast = Random.Range(4, 6) + 1;
                yield return StartCoroutine(AttackWithBlast(nbBlast));
            }
        }
    }

    protected IEnumerator AttackWithBlast(int nbBlast)
    {
        while (nbBlast != 0)
        {
            --nbBlast;
            CharAnimator.SetTrigger("blast");
            var rot = Quaternion.LookRotation(transform.position - MyCharacterController.Instance.transform.position);
            var blast = Resources.Load<GameObject>("Prefabs/Fireball");
            Instantiate(blast, RightAttack ? RightHand.position : LeftHand.position, rot);
            RightAttack = !RightAttack;
            GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/kiblast"));
            yield return new WaitForSeconds(0.6f);
        }
        Attacking = false;
        RandomAction();
    }

    public virtual void MoveHandToHand()
    {
    }

    /// <summary>
    /// Permet de définir aléatoirement une nouvelle trajectoire de déplacement pour notre personnage
    /// </summary>
    public void RandomMove()
    {
        CurrentAction = EnemyActions.RandomMove;

        float minZ = transform.position.z - 10;
        float maxZ = transform.position.z + 10;
        float minX = transform.position.x - 10;
        float maxX = transform.position.x + 10;

        _destinationPos = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));

        var relativePos = _destinationPos - transform.position;
        var lookRot = Quaternion.LookRotation(relativePos);
        lookRot.eulerAngles = new Vector3(0, lookRot.eulerAngles.y, lookRot.eulerAngles.z);
        transform.rotation = lookRot;
        BeginMoveTime = Time.time;
    }

    #endregion

    #region COMBOS METHODS

    public virtual IEnumerator Combo1()
    {
        yield break;
    }

    public virtual IEnumerator Combo2()
    {
        yield break;
    }

    #endregion

    public override void GettingHitCompleted()
    {
        base.GettingHitCompleted();
        StartCoroutine(WaitAndRandomAction(Random.Range(0.5f, 1.5f))); // Ici il faudra mettre un calcul en fonction de l'agilité
    }

    public override void ApplyDamage(TakeDamageModel model)
    {
        base.ApplyDamage(model);
        StopAllCoroutines();
        ActivateEndOfAnimationTrigger = false;
    }

    public virtual void StartAttack()
    {
        CanMove = true;
        CanAttack = true;
        RandomAction();
    }

    #region DODGE SKILLS

    /// <summary>
    /// Méthode qui permet de réaliser une esquive (lancement des animations, déclenchement des sons, etc.)
    /// </summary>
    public virtual void Dodge()
    {
        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/Generic/Attack/meleemiss1"));

        if (Random.value > 0.2f && CanAttack)
        {
            // On a 20% de chance d'enchainer l'esquive avec un TP + Attack
            StartCoroutine(SqueezeEnemyAttack(1, true));
            DodgeWithTpAndAttack();
        }
        else
        {
            var prefab = Resources.Load<GameObject>("CFXM_Text Wooosh Ground");
            if (prefab != null)
            {
                Instantiate(prefab, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z),
                            Quaternion.identity);
            }

            // Le personnage a 50% de chance de redonner un coup aprés une esquive
            if (CanAttack && Random.value < 0.5f)
            {
                StartCoroutine(SqueezeEnemyAttack(1));
                // Aprés une esquive, la cible (goku par exemple) ne pourra pas attaquer pendant 1 seconde.
                StartCoroutine(Attack(0.5f));
            }
        }
    }

    /// <summary>
    /// Permet de désactiver les attaques de sa cible (notre personnage) pendant x secondes.
    /// Cette méthode est utilisée nottament aprés une esquive.
    /// </summary>
    protected IEnumerator SqueezeEnemyAttack(float time, bool removeAutoFocus = false)
    {
        if (removeAutoFocus)
        {
            MyCharacterController.Instance.AutoFocus.enabled = false;
        }

        // On réinitialise l'historique de combo
        MyCharacterController.Instance.ClearCombo();

        MyCharacterController.Instance.CanAttack = false;
        MyCharacterController.Instance.CanMove = false;

        yield return new WaitForSeconds(time);

        MyCharacterController.Instance.CanAttack = true;
        MyCharacterController.Instance.CanMove = true;

        if (removeAutoFocus)
        {
            MyCharacterController.Instance.AutoFocus.enabled = true;
        }
    }

    public void DodgeWithTpAndAttack()
    {
        if (TpSprite != null)
        {
            var tpSprite = Instantiate(TpSprite, this.transform.position, this.transform.rotation) as GameObject;
            Destroy(tpSprite, 0.2f);
        }

        transform.position = MyCharacterController.Instance.transform.position - MyCharacterController.Instance.transform.forward;
        transform.LookAt(MyCharacterController.Instance.transform);
        // Ajouter le son du tp ici
        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/teleport"));

        //yield return new WaitForSeconds(.1f);

        StartCoroutine(Attack(0.3f));
    }

    #endregion

    /// <summary>
    /// Méthode qui permet de gérer la mort du personnage et par conséquent d'activer la fin de partie
    /// </summary>
    public override void Die()
    {
        base.Die();
        MyCharacterController.Instance.Win = true;
    }

    public void TakeDamage(string skill)
    {
        ApplyDamage(new TakeDamageModel
        {
            SkillName = skill,
            Default = true,
            CanDodge = false
        });
    }


}
