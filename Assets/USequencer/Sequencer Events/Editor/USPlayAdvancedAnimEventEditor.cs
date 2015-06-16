using UnityEditor;
using UnityEngine;
using System.Collections;

namespace WellFired
{
	[CustomUSEditor(typeof(USPlayAdvancedAnimEvent))]
	public class USPlayAdvancedAnimEventEditor : USEventBaseEditor
	{
		public override Rect RenderEvent(Rect myArea)
		{
			USPlayAdvancedAnimEvent animEvent = TargetEvent as USPlayAdvancedAnimEvent;
	
			float endPosition = ConvertTimeToXPos(animEvent.FireTime + (animEvent.Duration<=0.0f?3.0f:animEvent.Duration));
			myArea.width = endPosition - myArea.x;
	
			DrawDefaultBox(myArea);
	
			using(new GUIBeginArea(myArea))
			{
				GUILayout.Label(GetReadableEventName(), DefaultLabel);
				if(animEvent)
					GUILayout.Label("Animation : " + animEvent.animationClip, DefaultLabel);
			}
	
			return myArea;
		}
	}
}