using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Set uSequence Playback Rate")]
	[USequencerEvent("Sequence/Set Playback Rate")]
	public class USSetPlaybackRateEvent : USEventBase 
	{
		public USSequencer sequence = null;
		public float playbackRate = 1.0f;
		private float prevPlaybackRate = 1.0f;
		
		public override void FireEvent()
		{	
			if(!sequence)
				Debug.LogWarning("No sequence for USSetPlaybackRate : " + name, this);
			
			if (sequence)
			{
				prevPlaybackRate = sequence.PlaybackRate;
				sequence.PlaybackRate = playbackRate;
			}
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
			if(sequence)
				sequence.PlaybackRate = prevPlaybackRate;
		}
	}
}