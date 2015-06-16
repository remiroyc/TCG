using UnityEngine;
using System.Collections;

public class ShockwaveManager : MonoBehaviour {

	int radius = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(radius < 100)
		{
			radius++;

			var x = transform.localScale.x;
			var y = transform.localScale.y;
			var z = transform.localScale.z;

			x += radius/100;
			y += radius/100;
			z += radius/100;

			transform.localScale = new Vector3(x,y,z);
		}
		else
			radius = 0;


	}
}
