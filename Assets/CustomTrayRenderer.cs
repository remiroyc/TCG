using UnityEngine;
using System.Collections;

public class CustomTrayRenderer : MonoBehaviour {
    public GameObject character;
    public GameObject mainCam;
    private LineRenderer line;


	// Use this for initialization
	void Start () {
        line = this.gameObject.GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        //float characterAndTrailDist = (character.transform.position - this.transform.position).z;

        //trailObject.transform.localScale = new Vector3(characterAndTrailDist,0f,1f);
        //trailObject.transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.back, mainCam.transform.rotation * Vector3.up);

        var startingPoint = character.transform.position;
        var endPoint = this.transform.position;


        line.SetPosition(0, endPoint);
        line.SetPosition(1, startingPoint);

        //print(endPoint);
        
	}
}
