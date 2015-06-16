using UnityEngine;

public class TestKameha : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        var pos = transform.position;
        pos.z += 0.1f;
        transform.position = pos;
    }
}
