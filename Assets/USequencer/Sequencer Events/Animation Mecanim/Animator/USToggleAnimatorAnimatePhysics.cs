using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Toggle Animate Physics")]
	[USequencerEvent("Animation (Mecanim)/Animator/Toggle Animate Physics")]
	public class USToggleAnimatorAnimatePhysics : USEventBase 
	{
		public bool animatePhysics = true;
		private bool prevAnimatePhysics;
			
		public override void FireEvent()
		{
			Animator animator = AffectedObject.GetComponent<Animator>();
			if(!animator)
			{
				Debug.LogWarning("Affected Object has no Animator component, for uSequencer Event", this);
				return;
			}
			
			prevAnimatePhysics = animator.animatePhysics;
			animator.animatePhysics = animatePhysics;
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
			
			animator.animatePhysics = prevAnimatePhysics;
		}
	}
}