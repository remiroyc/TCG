using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(KamehamehaScript))]
public class TeleportationComboScript : MonoBehaviour
{
    public MyCharacterController CharController;
    public KamehamehaScript KamehamehaScript;
    public AudioClip MultiTPSound, TP1Sound, TP2Sound;
    public ParticleSystem FX_teleport, FX_project_down;
    public Camera CameraTp;
    public float DistanceProjection = 20f;
    public float DistanceFoeProjection = 4f;
    public GameObject tpSpriteEffect;
    public GameObject gokuMesh1;
    public GameObject gokuMesh2;

    /// <summary>
    /// Nombre souhaité de téléportations pendant le combo d'attaque
    /// </summary>
    public int NbAttacks = 8;

    /// <summary>
    /// Indique sur le combo de téléportation a été activé.
    /// </summary>
    private bool _teleportationActived;
    private bool _teleported;
    /// <summary>
    /// Compteur pour les téléportations
    /// </summary>
    private int _tpCount;
    private Enemy _enemy;
    private bool _enemyCanAttack, _enemyCanMove;

    private bool _teleportationAttack;
    private bool _inCouroutine;
    public GameObject CloneGokuPrefab;

    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        if (MultiTPSound == null)
        {
            MultiTPSound = Resources.Load<AudioClip>("Sounds/multi_tp");
        }
        if (TP1Sound == null)
        {
            TP1Sound = Resources.Load<AudioClip>("Sounds/teleport00");
        }
        if (TP2Sound == null)
        {
            TP2Sound = Resources.Load<AudioClip>("Sounds/teleport");
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void Update()
    {
        if (CharController.FocusEnemy != null)
        {
            _enemy = CharController.FocusEnemy.GetComponent<Enemy>();

        }
        else
        {
            return;
        }

        if (_teleportationAttack)
        {
            _enemy.GetComponent<TrailRenderer>().enabled = true;
            CameraTp.gameObject.SetActive(true);
            //On fait tourner la cam
            //CameraTp.transform.RotateAround(CameraTp.transform.position, Vector3.up, 20 * Time.deltaTime);
            CameraTp.transform.LookAt(_enemy.transform);
            CameraTp.transform.Translate(Vector3.left * Time.deltaTime);

            if (_tpCount <= NbAttacks)
            {
                if (!_inCouroutine)
                {
                    StartCoroutine(AttackAndTp());
                }
            }
            else
            {
                if (!_inCouroutine && Vector3.Distance(CharController.FocusEnemy.position, CharController.transform.position) >= DistanceProjection)
                {
                    StartCoroutine(FinishTpAttack());
                }
            }
        }
    }

    private IEnumerator FinishTpAttack()
    {
        _inCouroutine = true;

        tpSpriteEffect.SetActive(false);
        gokuMesh1.SetActive(true);
        gokuMesh2.SetActive(true);

        if (CharController.FocusEnemy != null)
        {
            transform.position = CharController.FocusEnemy.transform.position + CharController.FocusEnemy.up * 1.8f + (CharController.FocusEnemy.transform.forward * 0.7f);
        }

        AudioSource.PlayClipAtPoint(TP1Sound, transform.position, 1);

        _enemy.GetComponent<Rigidbody>().velocity = _enemy.GetComponent<Rigidbody>().velocity / 10;
        _enemy.GetComponent<Rigidbody>().useGravity = false;

        CharController.GetComponent<Rigidbody>().useGravity = false;
        yield return new WaitForSeconds(0.5f);
        CharController.CharAnimator.Play("Projection Down");

        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/pain2"), transform.position, 1);
        GetComponent<AudioSource>().PlayOneShot(CharController.AudioController.AudioClips["hit_projection"]);

        yield return new WaitForSeconds(0.2f);

        FX_project_down.Play();

        yield return new WaitForSeconds(0.2f);
        _enemy.KoManager.Ko = true;
        _enemy.GetComponent<Rigidbody>().useGravity = true;
        _enemy.GetComponent<Rigidbody>().AddForce(new Vector3(0f, -30f, 20f), ForceMode.VelocityChange);


        this.GetComponent<AudioSource>().PlayOneShot(CharController.AudioController.AudioClips["projection"]);
        this.GetComponent<AudioSource>().PlayOneShot(CharController.AudioController.AudioClips["swoop_hit"]);

        while (!_enemy.Grounded)
        {
            yield return new WaitForSeconds(1.5f);
        }

        this.GetComponent<AudioSource>().Stop();

        CharController.GetComponent<Rigidbody>().useGravity = true;
        CameraTp.gameObject.SetActive(false);

        // KamehamehaScript.LaunchKame(true);

        CharController.CanMove = true;
        CharController.CanAttack = true;
        CharController.Attacking = false;

        _tpCount = 0;
        _enemy.CanMove = _enemyCanMove;
        _enemy.CanAttack = _enemyCanAttack;
        CharController.Attacking = false;
        _inCouroutine = false;
        _teleportationAttack = false;
    }

    public IEnumerator PlayTeleportationCombo()
    {



        CharController.GetComponent<AudioSource>().PlayOneShot(TP2Sound);

        // On change la camera
        CameraTp.gameObject.SetActive(true);

        transform.position = CharController.FocusEnemy.transform.position + new Vector3(0, DistanceFoeProjection, -1f);
        _teleported = true;

        CharController.CharAnimator.Play("Projection Down");
        FX_project_down.Play();

        yield return new WaitForSeconds(0.4f);

        GetComponent<AudioSource>().PlayOneShot(CharController.AudioController.AudioClips["fat_punch"]);

        //CharController.FocusEnemy.rigidbody.AddForce(new Vector3(0, -10, 10f), ForceMode.VelocityChange);


        //FX_hit_floor.transform.position = new Vector3 (FocusEnemy.transform.position.x, 0f, FocusEnemy.transform.position.z);
        //FX_hit_floor.Play ();

        // On change la camera
        CameraTp.gameObject.SetActive(false);
    }

    public void StartFirstCombo()
    {
        // On se teleporte plusieurs fois autour de l'ennemi pour lui porter un coup 
        //if (CharController.CanAttack && !CharController.LoadingSkill && !CharController.Attacking && CharController.FocusEnemy != null && CharController.CloseCombat)
        //{
        CharController.CanMove = false;
        CharController.CanAttack = true;
        CharController.Attacking = true;

        var enemyScript = CharController.FocusEnemy.GetComponent<Enemy>();

        _enemyCanAttack = _enemy.CanAttack;
        _enemyCanMove = _enemy.CanMove;
        enemyScript.CanMove = false;
        enemyScript.CanAttack = false;

        CharController.GetComponent<AudioSource>().PlayOneShot(MultiTPSound);

        //MainCameraController.enabled = false;
        //CameraTp.gameObject.SetActive(true);

        _teleportationAttack = true;
        //}
    }

    private IEnumerator AttackAndTp()
    {
        tpSpriteEffect.SetActive(false);
        gokuMesh1.SetActive(true);
        gokuMesh2.SetActive(true);

        _inCouroutine = true;
        ++_tpCount;

        if (!FX_teleport.isPlaying)
        {
            FX_teleport.Play();
        }

        var tdm = new TakeDamageModel
        {
            Default = false,
            SkillName = string.Empty,
            Emitter = transform,
            CanDodge = false,
            AttackMultiplicator = 2
        };

        if (_tpCount == NbAttacks + 1)
        {
            _enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = (-_enemy.transform.forward * 0.7f) + _enemy.transform.position;
            AudioSource.PlayClipAtPoint(TP1Sound, transform.position, 1);

            yield return new WaitForSeconds(0.2f);

            _enemy.GetComponent<Rigidbody>().useGravity = false;
            tdm.Default = true;
            tdm.SkillName = "projection_up_kick";
            tdm.AttackMultiplicator = 1;
            CharController.CharAnimator.Play("Projection Up");
            GetComponent<AudioSource>().PlayOneShot(CharController.AudioController.AudioClips["fat_kick"]);

            yield return new WaitForSeconds(0.25f);

        }
        else
        {
            // On créé un clone rémanant
            var clone = Instantiate(CloneGokuPrefab, this.transform.position, Quaternion.LookRotation(_enemy.transform.position - this.transform.position)) as GameObject;
            clone.transform.Rotate(Vector3.left * 90);
            clone.transform.position = new Vector3(clone.transform.position.x, -2.53f, clone.transform.position.z);
            Destroy(clone, .5f);

            Vector3 tpPosition;
            var rand = Random.value;
            if (rand < 0.33f)
            {
                tpPosition = (_enemy.transform.right * 0.7f) + _enemy.transform.position;
            }
            else if (rand < 0.66f)
            {
                tpPosition = (-_enemy.transform.right * 0.7f) + _enemy.transform.position;
            }
            else
            {
                tpPosition = (_enemy.transform.forward * 0.7f) + _enemy.transform.position;
            }

            _enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = tpPosition;
            yield return new WaitForSeconds(0.08f);

            if (Random.value > 0.5f)
            {
                tdm.RightAttack = true;
                CharController.RightAttack = true;
                CharController.CharAnimator.SetBool("right_attack", CharController.RightAttack);

                //On fait une animation random pour le coup
                var rndm = Random.value;

                if (rndm > 0.3f && rndm < 0.5f)
                {
                    CharController.CharAnimator.SetTrigger("weak_attack_punch");
                }
                else if (rndm > 0.5f && rndm < 0.75)
                    CharController.CharAnimator.SetTrigger("medium_attack_kick");
                else
                    CharController.CharAnimator.SetTrigger("weak_attack_kick");

            }
            else
            {
                tdm.RightAttack = false;
                CharController.RightAttack = false;
                CharController.CharAnimator.SetBool("right_attack", CharController.RightAttack);

                //On fait une animation random pour le coup
                var rndm = Random.value;

                if (rndm > 0.3f && rndm < 0.5f)
                    CharController.CharAnimator.SetTrigger("weak_attack_punch");
                else if (rndm > 0.5f && rndm < 0.75)
                    CharController.CharAnimator.SetTrigger("medium_attack_kick");
                else
                    CharController.CharAnimator.SetTrigger("weak_attack_kick");
            }

            GetComponent<AudioSource>().PlayOneShot(CharController.AudioController.AudioClips["weak_punch"]);
        }

        CharController.GiveDamage(tdm, restrictToCloseCombat: false);

        tpSpriteEffect.SetActive(true);

        yield return new WaitForSeconds(0.08f);

        gokuMesh1.SetActive(false);
        gokuMesh2.SetActive(false);

        yield return new WaitForSeconds(0.08f);



        _inCouroutine = false;
    }

    public void ManageProjection()
    {
        if (!_teleportationActived && CharController.CloseCombat
            && CharController.CanAttack && !CharController.LoadingSkill
            && !CharController.Attacking && CharController.FocusEnemy != false)
        {
            if (CharController.FocusEnemy.GetComponent<Rigidbody>() != null)
            {
                CharController.Attacking = true;
                CharController.CharAnimator.Play("Projection Up");
                CharController.FocusEnemy.GetComponent<Rigidbody>().velocity = new Vector3(0, 15, 0);

                //soundProjection.Play();
                CharController.GetComponent<AudioSource>().PlayOneShot(CharController.AudioController.AudioClips["fat_kick"]);

                CharController.Ki -= 10;
                _teleportationActived = true;
            }
        }
        else
        {
            float diff = Mathf.Abs(transform.position.y - CharController.FocusEnemy.transform.position.y);
            if (diff >= DistanceProjection && _teleportationActived)
            {
                print(diff);
                StartCoroutine(PlayTeleportationCombo());
            }

            if (_teleported && CharController.Grounded)
            {
                CharController.FocusEnemy.GetComponent<TrailRenderer>().enabled = false;
                CharController.Attacking = false;
                _teleportationActived = false;
                _teleported = false;
            }
        }
    }

}
