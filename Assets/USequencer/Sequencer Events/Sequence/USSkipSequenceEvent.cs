using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Skip uSequence")]
	[USequencerEvent("Sequence/Skip uSequence")]
	public class USSkipSequenceEvent : USEventBase 
	{
		public USSequencer sequence = null;
		
		public bool skipToEnd = true;
		public float skipToTime = -1.0f;
		
		public override void FireEvent()
		{	
			if(!sequence)
			{
				Debug.LogWarning("No sequence for USSkipSequenceEvent : " + name, this);
				return;
			}
			
			if(!skipToEnd && skipToTime < 0.0f && skipToTime > sequence.Duration)
			{
				Debug.LogWarning("You haven't set the properties correctly on the Sequence for this USSkipSequenceEvent, either the skipToTime is invalid, or you haven't flagged it to skip to the end", this);
				return;
			}
			
			if(skipToEnd)
				sequence.SkipTimelineTo(sequence.Duration);
			else
				sequence.SkipTimelineTo(skipToTime);
		}
		
		public override void ProcessEvent(float deltaTime)
		{
			
		}
	}
}