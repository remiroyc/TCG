using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Toggle Object")]
	[USequencerEvent("Object/Toggle Object")]
	public class USEnableObjectEvent : USEventBase
	{
	    public bool enable = false;
		private bool prevEnable = false;
		
	#if (UNITY_3_5)
		public bool enableRecursively = true;
	#else
	#endif
		
		public override void FireEvent()
		{
	#if (UNITY_3_5)
			prevEnable = AffectedObject.active;
	#else
			prevEnable = AffectedObject.activeSelf;
	#endif
			
	#if (UNITY_3_5)
			if(enableRecursively)
		        AffectedObject.SetActiveRecursively(enable);
			else
				AffectedObject.active = enable;
	#else
			AffectedObject.SetActive(enable);
	#endif
		}
		
		public override void ProcessEvent(float deltaTime)
		{
			
		}
		
		public override void StopEvent()
		{
			UndoEvent();
		}
		
		public override void UndoEvent()
		{
			if(!AffectedObject)
				return;
			
	#if (UNITY_3_5)
			if(enableRecursively)
		        AffectedObject.SetActiveRecursively(prevEnable);
			else
				AffectedObject.active = prevEnable;
	#else
			AffectedObject.SetActive(prevEnable);
	#endif
		}
	}
}