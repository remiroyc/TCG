using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Set Playback Speed")]
	[USequencerEvent("Animation (Mecanim)/Animator/Set Playback Speed")]
	public class USSetAnimatorPlaybackSpeed : USEventBase 
	{
		public float playbackSpeed = 1.0f;
		private float prevPlaybackSpeed;
			
		public override void FireEvent()
		{
			Animator animator = AffectedObject.GetComponent<Animator>();
			if(!animator)
			{
				Debug.LogWarning("Affected Object has no Animator component, for uSequencer Event", this);
				return;
			}
			
			prevPlaybackSpeed = animator.speed;
			animator.speed = playbackSpeed;
		}
		
		public override void ProcessEvent(float runningTime)
		{
			
		}
		
		public override void StopEvent()
		{
			UndoEvent();
		}
		
		public override void UndoEvent()
		{
			Animator animator = AffectedObject.GetComponent<Animator>();
			if(!animator)
				return;
			
			animator.speed = prevPlaybackSpeed;
		}
	}
}