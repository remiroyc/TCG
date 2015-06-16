using UnityEngine;

public class Fireball : MonoBehaviour
{

    public AudioClip GroundExplosionSound;
    public float Speed;

    private float _timeLife;
    public Vector3 _originPos;
    private LineRenderer _lineRenderer;


    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _timeLife = Time.time;
        _originPos = transform.position;
        _lineRenderer.SetPosition(0, _originPos);
    }

    private void Update()
    {
        if (Time.time - _timeLife >= 10)
        {
            Destroy(gameObject);
        }
        transform.position += transform.forward * Speed * Time.deltaTime;
        _lineRenderer.SetPosition(1, transform.position);
        _lineRenderer.SetWidth(1f, 1f);
    }

    private void OnCollisionEnter(Collision collision)
    {

        var ge = Resources.Load("CFXPrefabs_Mobile/Explosions/CFXM_Explosion") as GameObject;
        Instantiate(ge, transform.position, Quaternion.identity);
        Destroy(gameObject);

        switch (collision.transform.tag.ToLower())
        {
            case "ground":
                if (GroundExplosionSound != null)
                {
                    AudioSource.PlayClipAtPoint(GroundExplosionSound, transform.position);
                }
                break;

            case "enemy":

                var enemy =  collision.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.ApplyDamage(new TakeDamageModel
                    {
                        SkillName = "kiblast",
                        Default = true,
                        CanDodge = true
                    });
                }
                break;

        }

    }

}
