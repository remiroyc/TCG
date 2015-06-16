using UnityEditor;
using UnityEngine;
using System.Collections;

namespace WellFired
{
	[CustomUSEditor(typeof(USPauseResumeAudioEvent))]
	public class USPauseResumeAudioEventEditor : USEventBaseEditor
	{
		public override Rect RenderEvent(Rect myArea)
		{
			USPauseResumeAudioEvent audioEvent = TargetEvent as USPauseResumeAudioEvent;
	
			DrawDefaultBox(myArea);
	
			using(new GUIBeginArea(myArea))
			{
				if(audioEvent)
					GUILayout.Label(audioEvent.pause?"Pause Audio":"Resume Audio", DefaultLabel);
			}
	
			return myArea;
		}
	}
}