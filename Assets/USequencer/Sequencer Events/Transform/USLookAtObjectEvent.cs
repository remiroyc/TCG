using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Look At Object")]
	[USequencerEvent("Transform/Look At Object")]
	public class USLookAtObjectEvent : USEventBase
	{
	    public GameObject objectToLookAt = null;
		public AnimationCurve inCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
		public AnimationCurve outCurve = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f));
		public float lookAtTime = 2.0f;
		
		private Quaternion sourceOrientation = Quaternion.identity;
		private Quaternion previousRotation = Quaternion.identity;
	
	    public override void FireEvent()
	    {
			if(!objectToLookAt)
			{
				Debug.LogWarning("The USLookAtObject event does not provice a object to look at", this);
				return;
			}    
			
			previousRotation = AffectedObject.transform.rotation;
			sourceOrientation = AffectedObject.transform.rotation;
	    }
	
	    public override void ProcessEvent(float deltaTime)
	    {
			if(!objectToLookAt)
			{
				Debug.LogWarning("The USLookAtObject event does not provice a object to look at", this);
				return;
			}
			
			float inDuration = inCurve[inCurve.length-1].time;
			float holdDuration = lookAtTime + inDuration;
			
			float ratio = 1.0f;
			if(deltaTime <= inDuration)
				ratio = Mathf.Clamp(inCurve.Evaluate(deltaTime), 0.0f, 1.0f);
			else if(deltaTime >= holdDuration)
				ratio = Mathf.Clamp(outCurve.Evaluate(deltaTime - holdDuration), 0.0f, 1.0f);
			
			Vector3 sourcePosition = AffectedObject.transform.position;
			Vector3 destinationPosition = objectToLookAt.transform.position;
			Vector3 toTarget = destinationPosition - sourcePosition;
			Quaternion targetOrientation = Quaternion.LookRotation(toTarget);
			
			AffectedObject.transform.rotation = Quaternion.Slerp(sourceOrientation, targetOrientation, ratio);
	    }
		
		public override void StopEvent()
		{
			UndoEvent();
		}
		
		public override void UndoEvent()
		{
			if(!AffectedObject)
				return;
			
			AffectedObject.transform.rotation = previousRotation;
		}
	}
}