using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Toggle Stabalize Feet")]
	[USequencerEvent("Animation (Mecanim)/Animator/Toggle Stabalize Feet")]
	public class USToggleAnimatorStabalizeFeet : USEventBase 
	{
		public bool stabalizeFeet = true;
		private bool prevStabalizeFeet;
			
		public override void FireEvent()
		{
			Animator animator = AffectedObject.GetComponent<Animator>();
			if(!animator)
			{
				Debug.LogWarning("Affected Object has no Animator component, for uSequencer Event", this);
				return;
			}
			
			prevStabalizeFeet = animator.stabilizeFeet;
			animator.stabilizeFeet = stabalizeFeet;
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
			
			animator.stabilizeFeet = prevStabalizeFeet;
		}
	}
}