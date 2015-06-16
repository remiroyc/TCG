using UnityEngine;
using System.Collections;

public class FlowmapAnimator : MonoBehaviour {
	
    public float flowSpeed;
	
	private Material currentMaterial;
	
	private float cycle, halfCycle;
	
	private float flowMapOffset0;
	private float flowMapOffset1;
	
	//private float flowTide = 0.0f;
	
	private bool hasTide;
	
	void Reset() {
		flowSpeed = .25f;	
	}
	
	// Use this for initialization
	void Start () {
		//Destroy(this);
		currentMaterial = GetComponent<Renderer>().material;
		
		cycle = 6.0f;//flowSpeed * 3.0f * .1f;//.15f;
		//Debug.Log("flowSpeed: " + flowSpeed + " cycle: " + cycle);
		halfCycle = cycle * .5f;
		
		flowMapOffset0 = 0.0f;
		flowMapOffset1 = halfCycle;
		
		//flowSpeed *= -1.0f;
		
		currentMaterial.SetFloat("halfCycle", halfCycle);
		
		/*if ( currentMaterial.HasProperty("_normalStrength") ) {
			float normalStrength = currentMaterial.GetFloat("_normalStrength") * flowSpeed;
			Debug.Log("normalStrength: " + normalStrength);
			normalStrength = Mathf.Clamp(normalStrength, 0.01f, 50.0f);
				
			currentMaterial.SetFloat("_normalStrength", normalStrength);
		}
		
		if ( currentMaterial.HasProperty("flowTide") )
			hasTide = true;
		else
			hasTide = false;*/
	}
	
	//private float animationValue = 0.0f;
	//private float deltaAnimation = 0.0f;
	
	// Update is called once per frame
	void Update () {
		//flowMapOffset0 += Time.deltaTime;
		//flowMapOffset1 += Time.deltaTime;
		flowMapOffset0 += flowSpeed * Time.deltaTime;
        flowMapOffset1 += flowSpeed * Time.deltaTime;
        /*if ( flowMapOffset0 >= cycle )
            flowMapOffset0 = .0f;

        if ( flowMapOffset1 >= cycle )
            flowMapOffset1 = .0f;
		*/
		
		while (flowMapOffset0 >= cycle) {
			//Debug.Log("flowMapOffset0: " + flowMapOffset0 + " cycle: " + cycle);
			flowMapOffset0 -= cycle;
		}
		
		while (flowMapOffset1 >= cycle) {
			//Debug.Log("Bigger than 250");
			flowMapOffset1 -= cycle;
		}
		
		
		currentMaterial.SetFloat("flowMapOffset0", flowMapOffset0);
		currentMaterial.SetFloat("flowMapOffset1", flowMapOffset1);
		
		//const float tideAnimationSpeed = .5f;
		
		/*if (hasTide) {
			
			//Derivative, are we going upwards or downwards?
			if (deltaAnimation >= 0.0f) {
				animationValue += Time.deltaTime * 1.5f * tideAnimationSpeed;
				deltaAnimation = Mathf.Cos(animationValue) * Time.deltaTime * 1.5f * tideAnimationSpeed;
			} else {
				animationValue += Time.deltaTime * tideAnimationSpeed;
				deltaAnimation = Mathf.Cos(animationValue) * Time.deltaTime * tideAnimationSpeed;
			}
			
			flowTide += deltaAnimation;
			
			Debug.Log("flowTide: " + flowTide);
			
			//flowTide = (Mathf.Sin(Time.time / 2.0f) + 1.0f) * .5f;
			currentMaterial.SetFloat("flowTide", (flowTide + 1.0f) * .5f );
		}*/
		
		/*if (animationValue >= 1.0f)
			animationValue = 0.0f;*/
		
		//terrainMaterial.SetFloat("_AnimationValue", animationValue);
	}
}
