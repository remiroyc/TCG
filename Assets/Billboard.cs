using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 dir = Camera.main.transform.forward;
        dir.y = 0.0f;
        dir.z = 0.0f;
        transform.rotation = Quaternion.LookRotation(dir);
	}
}
