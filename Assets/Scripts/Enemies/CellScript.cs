using System.Collections;
using UnityEngine;

public class CellScript : Enemy
{
    public ParticleSystem FireballParticule;
    public ParticleSystem KamehaParticule;
    public ParticleSystem KamehaParticule2;
    public bool EnableKamehameha = true;
    private GameObject _kamehameha;

    #region MONO BEHAVIOUR METHODS

    // ReSharper disable once UnusedMember.Local
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.transform.tag.ToLower())
        {
            case "blast":
                Life -= 60;
                CharAnimator.Play("ReceiveKiblast");
                GettingHit = false;
                RandomAction();
                break;
            default:
                var log = string.Format("Cell a collisionné avec {0} qui n'est pas un objet pris en compte.", collision.transform.tag);
                Debug.LogWarning(log);
                break;
        }
    }

    /// <summary>
    /// Evènement de détection de collision. Notre joueur est rentré en contact avec une particule.
    /// Cette méthode nous permet de gérer les attaques de type boules de feu (kamehameha, genkidama, blast, etc).
    /// </summary>
    public void OnParticleCollision(GameObject other)
    {
        if (other.tag == "FX_Genkidama")
        {
            GameObject.Find("explosion_sound").GetComponent<AudioSource>().Play();
            Life -= 1500f;

            if (Life <= 0f)
            {
                Die();
            }
        }
    }

    #endregion

    #region FIGHT METHODS

    public override IEnumerator Combo2()
    {
        ActivateEndOfAnimationTrigger = true;
        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/Cell/pain4"));

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = false;

        var capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.isTrigger = true;

        CharAnimator.Play("Combo2");
        MyCharacterController.Instance.TakeDamage("combo2", 350f, transform);

        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/pain2"));

        yield return new WaitForSeconds(0.5f);
        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/powerHit1"));

        yield return new WaitForSeconds(0.5f);
        GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/throw"));

        yield return new WaitForSeconds(1f);

        capsuleCollider.isTrigger = false;
        GetComponent<Rigidbody>().useGravity = true;

    }

    /// <summary>
    /// Méthode qui permet de gérer les encaissement de notre personnage ainsi que ses esquives
    /// </summary>
    public override void ApplyDamage(TakeDamageModel model)
    {
        base.ApplyDamage(model);

        if (CurrentSkill == SkillType.Kamehameha)
        {
            if (_kamehameha != null)
            {
                Destroy(_kamehameha);
            }
            KamehaParticule.Stop();
            KamehaParticule2.Stop();
            CurrentSkill = null;
        }

        var relativePos = MyCharacterController.Instance.transform.position - transform.position;
        var lookRot = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lookRot.eulerAngles.y, transform.rotation.eulerAngles.z);

        if (!model.Default)
        {
            var randVal = Random.Range(0, 100);
            var dodge = (model.CanDodge && randVal <= DodgeProbability);

            Debug.Log(string.Format("Lancer d'esquive : {0} / {1}", randVal, DodgeProbability));

            CharAnimator.SetBool("dodge", dodge);
            CharAnimator.SetBool("hit_right_attack", model.RightAttack);

            switch (model.AttackTypes)
            {

                case AttackTypes.Weak:
                    {
                        CharAnimator.SetTrigger("hit_weak_attack");
                        if (!dodge)
                        {
                            ActivateEndOfHitAnimationTrigger = true;
                            GettingHit = true;

                            PlayRandomPainAudioClip();
                            Instantiate(Resources.Load("CFXM_Hit_A Red"), transform.position, Quaternion.identity);
                            Life -= (200 * model.AttackMultiplicator);

                        }
                        else
                        {
                            Dodge();
                        }
                    }
                    break;

                case AttackTypes.Medium:
                    {
                        CharAnimator.SetTrigger("hit_medium_attack");
                        if (!dodge)
                        {
                            GettingHit = true;
                            Life -= 300;
                            PlayRandomPainAudioClip();
                            Instantiate(Resources.Load("CFXM_Hit_A Red"), transform.position, Quaternion.identity);
                            StartCoroutine(WaitAndRandomAction(Random.Range(0.5f, 1.5f)));
                        }
                        else
                        {
                            Dodge();
                        }
                    }


                    break;

                case AttackTypes.Combo:

                    switch (model.SkillName)
                    {
                        case "combo_punch":
                            {
                                if (!dodge)
                                {
                                    Life -= 350;
                                    StartCoroutine(PlayComboAttackHit());
                                }
                            }
                            break;
                        default:
                            Debug.LogError("Combo inconnu : " + model.SkillName);
                            break;
                    }

                    break;

            }

        }
        else
        {

            CharAnimator.SetBool("dodge", false);

            // L'attaque possède la propriété "Default" a true, cela signifie qu'elle a été générée probablement via un animation event
            // On va devoir donc retrouver les propriètés de l'attaque uniquement grâce au nom de la capacité.
            switch (model.SkillName)
            {
                case "dash_attack":
                    {
                        GettingHit = true;
                        Life -= 100;
                        // _animator.Play("ReceivePunchComboAttack");
                        RandomAction();
                    }
                    break;

                case "weak_combo":
                    {
                        StartCoroutine(PlayComboAttackHit());
                        Life -= 200;
                    }
                    break;

                case "throw":
                    Instantiate(Resources.Load("CFXM_Text Wham"), transform.position, Quaternion.identity);
                    CharAnimator.SetBool("swung", true);
                    Life -= 200;
                    GettingHit = false;
                    RandomAction();
                    break;

                case "kiai":
                    Instantiate(Resources.Load("CFXM_Text Wham"), transform.position, Quaternion.identity);
                    CharAnimator.SetTrigger("hit_kiai");
                    Life -= 30;
                    GettingHit = false;
                    RandomAction();
                    break;

                //case "low_kick":
                //    Life -= 50;
                //    var hit = Resources.Load("CFXPrefabs_Mobile/Hits/CFXM_Hit_C White") as GameObject;
                //    Instantiate(hit, transform.position, Quaternion.identity);
                //    _animator.Play("ReceiveLowKick");
                //    _gettingHit = false;
                //    RandomAction();
                //    break;

                case "spiral_kick":
                    Life -= 300;
                    var boum = Resources.Load("CFXPrefabs_Mobile/Texts/CFXM_Text Boom") as GameObject;
                    Instantiate(boum, transform.position, Quaternion.identity);
                    var dir = (transform.position - MyCharacterController.Instance.transform.position).normalized;
                    dir.y = 1;
                    GetComponent<Rigidbody>().AddForce(dir * 350f);
                    RandomAction();
                    break;

                case "projection_up_kick":
                    Life -= 300;
                    GetComponent<Rigidbody>().AddForce(transform.up * 1000);
                    CharAnimator.Play("Hit Blow Back");
                    break;

                case "kamehameha":

                    Life -= 1000;
                    var direction = transform.position - model.Emitter.position;
                    GetComponent<Rigidbody>().AddForce(direction * 100);

                    break;

                default:
                    Debug.LogError(string.Format("Technque inconnue: {0}", model.SkillName));
                    break;
            }
        }

    }

    public override IEnumerator CastSpecialDistanceAttack()
    {
        Attacking = true;
        CurrentSkill = SkillType.Kamehameha;
        CanAttack = false;
        CanMove = false;

        //CharAnimator.Play("ChargeFireBall");
        CharAnimator.Play("Attacks.ChargeKameha");

        if (KamehaParticule != null)
        {
            KamehaParticule.Play();
        }

        if (KamehaParticule2 != null)
        {
            KamehaParticule2.Play();
        }

        var audioClip = Resources.Load<AudioClip>("Sounds/Cell/cell_kamehame");
        GetComponent<AudioSource>().PlayOneShot(audioClip);

        audioClip = Resources.Load<AudioClip>("Sounds/kame_charge");
        GetComponent<AudioSource>().PlayOneShot(audioClip);

        yield return new WaitForSeconds(4f);

        CharAnimator.Play("Attacks.ThrowKameha");

        yield return new WaitForSeconds(.1f);

        if (KamehaParticule != null)
        {
            KamehaParticule.Stop();
            KamehaParticule.Clear();
        }

        if (KamehaParticule2 != null)
        {
            KamehaParticule2.Stop();
            KamehaParticule2.Clear();
        }

        //CharAnimator.Play("ThrowFireball");

        var fireball = Resources.Load("Prefabs/KamehaGlowPrefab") as GameObject;


        audioClip = Resources.Load<AudioClip>("Sounds/Cell/cell_ha");
        GetComponent<AudioSource>().PlayOneShot(audioClip);
        audioClip = Resources.Load<AudioClip>("Sounds/kamehameha_fire");
        GetComponent<AudioSource>().PlayOneShot(audioClip);

        var direction = (transform.position - MyCharacterController.Instance.transform.position).normalized;
        var kamePosition = transform.position + (-direction * 0.2f) + Vector3.up / 7;
        _kamehameha = (GameObject)Instantiate(fireball, kamePosition, Quaternion.LookRotation(direction));

        if (_kamehameha != null)
        {
            var energyBeam = _kamehameha.GetComponent<EnergyBeam>();
            energyBeam.KamehamehaDirection = -direction;
            energyBeam.PlayerEmitter = this;
            energyBeam.Mask = EnemyLayerMask;
            Destroy(_kamehameha, 7f);
        }

        //audioClip = Resources.Load<AudioClip>("Sounds/Cell/fire_scream");
        AudioSource.PlayClipAtPoint(audioClip, transform.position, 1);

        yield return new WaitForSeconds(Random.Range(2f, 4f));
        Attacking = false;
        CurrentSkill = null;

        CanAttack = true;
        CanMove = true;

        RandomAction();
    }

    ///// <summary>
    ///// Méthode qui permet d'appliquer la réception du coup "medium attack"
    ///// </summary>
    //private IEnumerator PlayWeakAttackHit()
    //{

    //    PlayRandomPainAudioClip();

    //    Instantiate(Resources.Load("CFXM_Hit_A Red"), transform.position, Quaternion.identity);
    //    _animator.Play("ReceiveMediumAttack");
    //    yield return new WaitForSeconds(0.5f);
    //    _gettingHit = false;
    //    yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
    //    RandomAction();
    //}

    /// <summary>
    /// Méthode qui permet d'appliquer la réception du coup "combo"
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayComboAttackHit()
    {

        if (PainAudioClips.Length > 0)
        {
            var nb = Random.Range(0, PainAudioClips.Length - 1);
            var selectedClip = PainAudioClips[nb];
            if (selectedClip != null)
            {
                AudioSource.PlayClipAtPoint(selectedClip, transform.position);
            }
        }

        Instantiate(Resources.Load<GameObject>("CFXM_Text Wham"), transform.position, Quaternion.identity);
        CharAnimator.Play("Combo1Hit");
        yield return new WaitForSeconds(0.5f);
        GettingHit = false;
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        RandomAction();
    }

    public override IEnumerator Combo1()
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region MOVEMENT METHODS

    ///// <summary>
    ///// Permet au personnage de recibler automatiquement le héros
    ///// </summary>
    ///// <returns></returns>
    //private IEnumerator RefocusAndWait()
    //{
    //    CurrentAction = EnemyActions.Refocus;
    //    Refocus();
    //    yield return new WaitForSeconds(1);
    //    RandomAction();
    //}

    #endregion

    #region ANOTHER GAME METHODS

    /// <summary>
    /// Permet de tirer au sort la nouvelle action que va effectuer le personnage
    /// </summary>
    public override void RandomAction()
    {
        if (Attacking)
        {
            Attacking = false;
        }

        // Si notre personnage est à plus de 50 mètres il ne peut que faire l'action d'essayer de venir au corp à corp.
        if (Distance >= 50)
        {
            if (CanMove)
            {
                BeginMoveTime = Time.time;
                CurrentAction = EnemyActions.MoveHandToHand;
            }
        }
        else
        {
            var val = Random.value;
            if (val <= 0.25f)
            {
                StartCoroutine(Idle(IdleTime));
            }
            else if (val <= 0.5f)
            {
                if (CanAttack)
                {
                    StartCoroutine(Attack());
                }
                else if (CanMove)
                {
                    RandomMove();
                }
            }
            else if (val <= 0.75f)
            {
                if (CanAttack)
                {
                    BeginMoveTime = Time.time;
                    CurrentAction = EnemyActions.MoveHandToHand;
                }
                else if (CanMove)
                {
                    RandomMove();
                }
            }
            else if (CanMove)
            {
                RandomMove();
            }
        }
    }

    #endregion

}

