using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public Transform ViewFinder, Character;

    public float DistanceMax;
    private float _inputX, _inputY;

    private void Start()
    {
        transform.position = ViewFinder.transform.position + (Vector3.back * DistanceMax);
    }

    private void Update()
    {
        var rotation = Character.rotation;

        _inputX += Input.GetAxis("Mouse X") * Time.deltaTime;
        _inputY += Input.GetAxis("Mouse Y") * Time.deltaTime;

        Character.transform.LookAt(transform.position);
        Character.Rotate(Vector3.up * 180);


        const float maxThetaAngle = 90 * Mathf.Deg2Rad;
        var angleTheta = _inputX * 100 * Mathf.Deg2Rad;

        if (angleTheta > maxThetaAngle)
        {
            angleTheta = maxThetaAngle;
        }
        else if (angleTheta < -maxThetaAngle)
        {
            angleTheta = -maxThetaAngle;
        }

        const float maxPhiAngle = 90 * Mathf.Deg2Rad;
        float anglePhi = _inputY * Mathf.Deg2Rad * 100;
        if (anglePhi > maxPhiAngle)
        {
            anglePhi = maxPhiAngle;
        }
        else if (anglePhi < -maxPhiAngle)
        {
            anglePhi = -maxPhiAngle;
        }

        Debug.Log(string.Format("angleTheta : {0} anglePhi: {1}", (angleTheta * Mathf.Rad2Deg), (anglePhi * Mathf.Rad2Deg)));

        Vector3 pos = Vector3.zero;
        // transform.position = ViewFinder.transform.position + (-ViewFinder.forward * DistanceMax) + (Vector3.up * CameraOffset);

        // Debug.Log (Mathf.Sin(angleTheta));


        pos.y = DistanceMax * Mathf.Sin(angleTheta) * Mathf.Sin(anglePhi);
        pos.z = DistanceMax * Mathf.Sin(angleTheta) * Mathf.Cos(anglePhi);
        pos.x = DistanceMax * Mathf.Cos(angleTheta);

        transform.position = new Vector3(Character.position.x - pos.x, Character.position.y - pos.y, Character.position.z - pos.z);

        var lookRotation = Quaternion.LookRotation(Character.position - transform.position);
        transform.rotation = lookRotation;

    }


}

