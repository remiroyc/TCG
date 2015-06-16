using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Toggle Apply Root Motion")]
	[USequencerEvent("Animation (Mecanim)/Animator/Toggle Apply Root Motion")]
	public class USToggleAnimatorApplyRootMotion : USEventBase 
	{
		public bool applyRootMotion = true;
		private bool prevApplyRootMotion;
			
		public override void FireEvent()
		{
			Animator animator = AffectedObject.GetComponent<Animator>();
			if(!animator)
			{
				Debug.LogWarning("Affected Object has no Animator component, for uSequencer Event", this);
				return;
			}
			
			prevApplyRootMotion = animator.applyRootMotion;
			animator.applyRootMotion = applyRootMotion;
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
			
			animator.applyRootMotion = prevApplyRootMotion;
		}
	}
}