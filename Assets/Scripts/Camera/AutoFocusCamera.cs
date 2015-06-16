using UnityEngine;

public class AutoFocusCamera : MonoBehaviour
{

    public float Distance = 10;
    private Quaternion _decalage;

    private void LateUpdate()
    {
        var dir = MyCharacterController.Instance.transform.position + (-MyCharacterController.Instance.transform.forward * Distance) + Vector3.down / 2 ;
        dir.y += 1f;
        transform.position = Vector3.Slerp(transform.position, dir , Time.deltaTime * 3);
        var angle = (Quaternion.LookRotation(MyCharacterController.Instance.transform.position - (transform.position ))) ;
        transform.rotation = Quaternion.Slerp(transform.rotation, angle, Time.deltaTime * 3);
    }

}
