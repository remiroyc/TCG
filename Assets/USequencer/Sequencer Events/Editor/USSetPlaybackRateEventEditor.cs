using UnityEditor;
using UnityEngine;
using System.Collections;

namespace WellFired
{
	[CustomUSEditor(typeof(USSetPlaybackRateEvent))]
	public class USSetPlaybackRateEventEditor : USEventBaseEditor
	{
		public override Rect RenderEvent(Rect myArea)
		{
			USSetPlaybackRateEvent setPlaybackRateEvent = TargetEvent as USSetPlaybackRateEvent;
	
			DrawDefaultBox(myArea);
	
			using(new GUIBeginArea(myArea))
			{
				if(setPlaybackRateEvent)
				{
					GUILayout.Label("Set Playback Rate for : " + (setPlaybackRateEvent.sequence?setPlaybackRateEvent.sequence.name:"null"), DefaultLabel);			
					GUILayout.Label(setPlaybackRateEvent.sequence?setPlaybackRateEvent.sequence.name:"null", DefaultLabel);
				}
				if (setPlaybackRateEvent)
					GUILayout.Label("Playback Rate : " + setPlaybackRateEvent.playbackRate, DefaultLabel);
			}
	
			return myArea;
		}
	}
}