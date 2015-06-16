using DG.Tweening;
using UnityEngine;
using System.Collections;

public class MoveWithRandomPath : MonoBehaviour
{

    private float xAxis, yAxis, zAxis;

    void Start()
    {
        Vector3 endPos = (transform.forward * 10 + transform.position);
        //DOTween.To(() => transform.position, x => transform.position = x, endPos, 10).SetEase(Ease.OutElastic);

        xAxis = transform.position.x;
        zAxis = transform.position.z;
        yAxis = transform.position.y;

        DOTween.To(() => xAxis, x => xAxis = x, endPos.x, 5).SetEase(Ease.InOutElastic);
        DOTween.To(() => yAxis, y => yAxis = y, endPos.y, 5).SetEase(Ease.OutCirc); ;
        DOTween.To(() => zAxis, z => zAxis = z, endPos.z, 5);  //.SetEase(Ease.InOutElastic);
    }

    void Update()
    {
        transform.position = new Vector3(xAxis, yAxis, zAxis);
    }
}
