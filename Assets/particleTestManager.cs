using UnityEngine;
using System.Collections;

public class particleTestManager : MonoBehaviour {
	public GameObject player;


	void Start () {
		
	}


	void FixedUpdate () {
		this.transform.rotation = player.transform.rotation;
	}
}