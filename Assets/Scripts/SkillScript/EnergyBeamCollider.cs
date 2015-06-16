using DG.Tweening;
using UnityEngine;

public class EnergyBeamCollider : MonoBehaviour
{

    public EnergyBeam EnergyBeamBase;
    public AudioClip GroundExplosionSound;
    public GameObject ExplosionPrefab;
    public bool Collided = false;
    public float Speed;

    private Vector3 _begin;
    private const float Interval = 0.4f;
    private float _timer = 0.0f;
    private bool hasHit = false;
    private RaycastHit _hit;


    //private void Start()
    //{
    //    if (ExplosionPrefab == null)
    //    {
    //        ExplosionPrefab = Resources.Load<GameObject>("Prefabs/Bigkiplosion");
    //    }
    //    _begin = transform.position;
    //    _timer = Interval + 1;
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Collided = true;
    //    Debug.Log(string.Format("Collision du kamehameha avec {0}", collision.collider.name));

    //    if (Random.value >= 0.5f) // Une chance sur deux de shake la caméra.
    //    {
    //        Camera.main.transform.DOShakeRotation(1.5f, 10, 5);
    //    }

    //    var explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);

    //    switch (collision.transform.tag.ToLower())
    //    {
    //        case "ground":
    //            if (GroundExplosionSound != null)
    //            {
    //                AudioSource.PlayClipAtPoint(GroundExplosionSound, transform.position);
    //            }
    //            break;

    //        case "enemy":
    //            var enemy = collision.collider.GetComponent<Enemy>();
    //            if (enemy != null)
    //            {
    //                enemy.ApplyDamage(new TakeDamageModel
    //                {
    //                    Emitter = MyCharacterController.Instance.transform,
    //                    SkillName = "kamehameha",
    //                    Default = true,
    //                    CanDodge = true
    //                });
    //            }
    //            break;
    //    }

    //    StartCoroutine(WaitAndDestroyKamehameha(3f));
    //    Destroy(explosion, 6f);

    //}



    //private IEnumerator WaitAndDestroyKamehameha(float sec)
    //{
    //    yield return new WaitForSeconds(sec);
    //    EnergyBeamBase.DestroyKamehameha();
    //    //On peut rebouger a nouveau
    //    MyCharacterController.Instance.CharAnimator.SetBool("kame_controlling", false);
    //    MyCharacterController.Instance.CanMove = true;
    //    MyCharacterController.Instance.CanAttack = true;
    //    // On rattache le perso de la camera
    //    MyCharacterController.Instance.GetComponent<FollowCamera>().enabled = true;
    //}

}
