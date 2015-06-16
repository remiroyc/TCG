using System.Collections;
using UnityEngine;

public class SaibaimanScript : Enemy
{

    /// <summary>
    /// Distance de l'impact de l'explosion de Saibaiman
    /// </summary>
    public float MaxMagnitudeForExplosionImpact = 0.2f;

    /// <summary>
    /// Temps maximum entre le moment où Saibaiman déclenche son attaque Kamikaze et l'explosion (si il n'a 
    /// pas touché notre joueur entre temps.
    /// </summary>
    public float MaxTimeExplode = 1.5f;

    /// <summary>
    /// Valeure du dommage occasionné par une attaque de type Kamikaze
    /// </summary>
    public float KamikazeExplosionDamage = 50f;

    public float KamikazeAttackSpeed = 300f;

    private bool _jumpAttack = false;
    private Vector3 _finalTargetPosition;
    private float _timerExplosion = 0;
    private bool _shooted = false;

    /// <summary>
    /// Tolérence entre le point d'impact final et la position du saibaiman (lors de l'attaque Kamikaze).
    /// </summary>
    private const float TargetToleranceMagnitude = 0.2f;

    protected override void Update()
    {
        base.Update();

        if (_shooted)
        {
            return;
        }

        if (Target != null && CanAttack && CanMove)
        {
            var dir = Target.position - transform.position;
            var magn = dir.sqrMagnitude;

            if (_jumpAttack)
            {
                if (Time.time - _timerExplosion >= MaxTimeExplode || (_finalTargetPosition - transform.position).sqrMagnitude <= TargetToleranceMagnitude)
                {
                    Explode(playerIsTouched: magn <= MaxMagnitudeForExplosionImpact);
                }
                transform.position = Vector3.MoveTowards(transform.position, _finalTargetPosition, Time.deltaTime * Speed);
            }
            else
            {

                var rotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
                transform.position = Vector3.MoveTowards(transform.position, Target.position, Time.deltaTime * Speed);
                CharAnimator.SetFloat("speed", 1);

                if (magn < 3)
                {
                    _jumpAttack = true;
                    _finalTargetPosition = Target.position;
                    _timerExplosion = Time.time;
                    CharAnimator.Play("JumpAttack");
                }

            }

        }
        else
        {
            CharAnimator.SetFloat("Speed", 0);
        }
    }

    public void Explode(bool playerIsTouched)
    {
        var virus = Resources.Load<GameObject>("CFXPrefabs_Mobile/Misc/CFXM_Virus");
        Instantiate(virus, transform.position, Quaternion.identity);
        if (playerIsTouched)
        {
            MyCharacterController.Instance.TakeDamage("saibaiman_explosion", KamikazeExplosionDamage, null);
        }
        Destroy(transform.gameObject);
    }

    public override void ApplyDamage(TakeDamageModel model)
    {
        PlayRandomPainAudioClip();

        _shooted = true;
        var hit = Resources.Load("CFXPrefabs_Mobile/Hits/CFXM_Hit_C White") as GameObject;
        Instantiate(hit, transform.position, Quaternion.identity);
        var dir = transform.position - model.Emitter.position;

        //var rotation = Quaternion.LookRotation(dir);
        //transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        //dir = transform.TransformDirection(dir);

        GetComponent<Rigidbody>().AddForce(dir * KamikazeAttackSpeed);
        CharAnimator.Play("HitBlowUp");
        Life = 0;
    }

    public void OnParticleCollision(GameObject other)
    {
        if (other.tag == "FX_Kameha")
        {
            _shooted = true;
            var direction = transform.position - other.transform.position;
            var rotation = Quaternion.LookRotation(other.transform.position - transform.position);
            transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);

            direction = direction.normalized;
            direction.y = 1;
            direction *= 250;

            CharAnimator.Play("HitBlowUp");
            GetComponent<Rigidbody>().AddForce(direction);

        }
    }

    IEnumerator WaitAndDead()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public override void Die()
    {
        StartCoroutine(WaitAndDead());
    }

    public override void StartAttack()
    {
        CanMove = true;
        CanAttack = true;
    }

    public override void RandomAction()
    {
    }

    public override void Dodge()
    {
    }


    public override IEnumerator CastSpecialDistanceAttack()
    {
        yield return null;
    }
}
