using UnityEngine;

public class DestroyObjectWithCollision : MonoBehaviour
{

    public GameObject ExplosionGameObject;
    public float MaxLifeTime = 10f;
    private float _time;
    public AudioClip ExplosionSound;

    private void Start()
    {
        _time = Time.time;
    }

    private void LateUpdate()
    {
        if (Time.time - _time >= MaxLifeTime)
        {
            DestroyObj();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (ExplosionSound != null)
        //{
        //    audio.PlayOneShot(ExplosionSound);
        //}
        DestroyObj();
    }

    protected void DestroyObj()
    {
        if (ExplosionGameObject != null)
        {
            Instantiate(ExplosionGameObject, transform.position, Quaternion.identity);
        }
        GetComponent<ParticleSystem>().Stop();
        Destroy(gameObject);
    }

}
