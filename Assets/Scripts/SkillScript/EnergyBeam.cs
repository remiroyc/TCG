using System;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EnergyBeam : MonoBehaviour
{

    public GameObject OriginSphere;
    public GameObject EndSphere;
    public GameObject Beam;
    public float Speed = 10;
    public float MaxLiveTime = 10;
    public AudioClip GroundExplosionSound;
    public Vector3 KamehamehaDirection;
    public GameObject ExplosionPrefab;
    public LayerMask Mask = 1;
    public LineRenderer LineRenderer;
    public float SphereDectectionRadius = 2;
    public Player PlayerEmitter;

    private Vector3 _begin;
    private float _timeToLive;
    public float Interval;
    private float _timer;
    private bool _hasHit;
    private RaycastHit _hit;
    private float _timeTillImpact;
    private bool _collided;
    private Vector3 _originPosition;

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        _timer = 0;
        Interval = 0.3f;
        _timeToLive = Time.time;
        LineRenderer.useWorldSpace = true;
        LineRenderer.SetPosition(0, EndSphere.transform.position);
        LineRenderer.SetPosition(1, EndSphere.transform.position);
        _originPosition = EndSphere.transform.position;
    }

    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        // OriginSphere.transform.position = transform.position;
        _begin = EndSphere.transform.position;
        if (ExplosionPrefab == null)
        {
            ExplosionPrefab = Resources.Load<GameObject>("Prefabs/Bigkiplosion");
        }

        Debug.Log("Speed du kamehameha : " + Speed);

    }

    private bool _firstFrame = true;

    // ReSharper disable once UnusedMember.Local
    private void Update()
    {
        if (_collided)
        {
            DestroyKamehameha();
            return;
        }

        // On n'autorise pas un intervale plus petit que celui d'une frame.
        var usedInterval = Interval;
        if (Time.deltaTime > usedInterval)
        {
            usedInterval = Time.deltaTime;
        }



        // A chaque intervale, on raycast depuis la position actuelle de la sphere jusqu'à la position
        // que la sphere aura au prochain interval.
        if (!_hasHit && (_firstFrame || _timer >= usedInterval))
        {
            if (_firstFrame)
            {
                _firstFrame = false;
            }
            _timer = 0;
            var distanceThisInterval = Speed * usedInterval;

            // Ancienne méthode ci-dessous :
            // if (Physics.Raycast(_begin, KamehamehaDirection, out _hit, distanceThisInterval, Mask))

            var ray = new Ray(_begin, KamehamehaDirection);
            var end = _begin + (KamehamehaDirection * distanceThisInterval);

            // Debug.DrawLine(_begin, end, Color.red);
            // Debug.Break();

            if (Physics.SphereCast(ray, SphereDectectionRadius, out _hit, distanceThisInterval, Mask))
            {
                _hasHit = true;
                Debug.Log(string.Format("Collision de {0} avec {1}", transform.name, _hit.collider.name));

                if (Speed > 0)
                {
                    _timeTillImpact = _hit.distance / Speed;
                }
            }
            _begin = end;
        }

        _timer += Time.deltaTime;

        // Aprés que le raycast est traversé quelque chose, on attend que la sphere du kamehamhea
        // atteigne le point d'impact.
        if (_hasHit && _timer > _timeTillImpact)
        {
            ManageCollision(_hit.transform);
        }
        else
        {
            var newDirection = KamehamehaDirection * Speed * Time.deltaTime;
            EndSphere.transform.position += newDirection;
            LineRenderer.SetPosition(1, EndSphere.transform.position);
        }

    }

    private void ManageCollision(Transform colliderObj)
    {
        _collided = true;

        if (PlayerEmitter.IsMyPlayer)
        {
            Camera.main.transform.DOShakeRotation(1.5f, 50, 5);
        }

        var explosion = Instantiate(ExplosionPrefab, EndSphere.transform.position, Quaternion.identity);
        transform.FindChild("End").GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/explosion2"));

        Debug.Log(string.Format("{0} a heurté {1} ({2})", transform.name, colliderObj.name, colliderObj.tag));

        switch (colliderObj.tag.ToLower())
        {
            case "ground":
                if (GroundExplosionSound != null)
                {
                    AudioSource.PlayClipAtPoint(GroundExplosionSound, transform.position);
                }
                break;

            case "enemy":
                var enemy = colliderObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.ApplyDamage(new TakeDamageModel
                    {
                        Emitter = PlayerEmitter.transform,
                        SkillName = "kamehameha",
                        Default = true,
                        CanDodge = false
                    });
                }
                else
                {
                    Debug.LogError("enemy == null");
                }
                break;

            case "player":

                var player = colliderObj.GetComponent<Player>();
                if (player != null)
                {
                    player.ApplyDamage(new TakeDamageModel
                    {
                        Emitter = PlayerEmitter.transform,
                        SkillName = "kamehameha",
                        Default = true,
                        CanDodge = false
                    });
                }
                else
                {
                    Debug.LogError("player == null");
                }
                break;

        }

        StartCoroutine(WaitAndDestroyKamehameha(3f));
        Destroy(explosion, 6f);
    }

    private IEnumerator WaitAndDestroyKamehameha(float sec)
    {
        yield return new WaitForSeconds(sec);
        DestroyKamehameha();
    }

    // ReSharper disable once UnusedMember.Local
    //private void LateUpdate()
    //{
    //    if (Time.time - _timeToLive >= MaxLiveTime && !_collided)
    //    {
    //        DestroyKamehameha();
    //    }
    //}

    public void DestroyKamehameha()
    {
        foreach (var particle in GetComponents<ParticleSystem>())
        {
            particle.Stop();
        }
        foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        {
            particle.Stop();
        }
        Destroy(gameObject);

        if (PlayerEmitter.IsMyPlayer)
        {
            MyCharacterController.Instance.KamehamehaManager.CancelKame();
        }
    }

}
