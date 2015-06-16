using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Pause uSequence")]
	[USequencerEvent("Sequence/Pause uSequence")]
	public class USPauseSequenceEvent : USEventBase 
	{
		public USSequencer sequence = null;
		
		public override void FireEvent()
		{	
			if(!sequence)
				Debug.LogWarning("No sequence for USPauseSequenceEvent : " + name, this);
			
			if (sequence)
				sequence.Pause();
		}
		
		public override void ProcessEvent(float deltaTime)
		{
			
		}
	}
}