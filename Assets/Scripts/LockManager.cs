using UnityEngine;
using System.Collections;

public class LockManager : MonoBehaviour {

    public GameObject Target;


	void Update () {
        var relativePos = Target.transform.position - transform.position;
        var rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = rotation;
	}
}
