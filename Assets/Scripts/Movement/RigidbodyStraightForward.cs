using UnityEngine;
using System.Collections;

public class RigidbodyStraightForward : MonoBehaviour
{

    public float Speed = 30f;

    private void FixedUpdate()
    {
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().MovePosition(transform.position + (-transform.forward * Time.deltaTime * Speed));
        }
    }
}
