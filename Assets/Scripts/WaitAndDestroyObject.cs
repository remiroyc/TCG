using UnityEngine;

public class WaitAndDestroyObject : MonoBehaviour
{

    public float MaxLifeTime;
    private float _time;

    void Start()
    {
        _time = Time.time;
    }

    void Update()
    {
        if (Time.time - _time >= MaxLifeTime)
        {
            Destroy(gameObject);
        }
    }
}
