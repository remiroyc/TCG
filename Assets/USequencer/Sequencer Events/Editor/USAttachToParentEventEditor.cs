using UnityEditor;
using UnityEngine;
using System.Collections;

namespace WellFired
{
	[CustomUSEditor(typeof(USAttachToParentEvent))]
	public class USAttachToParentEventEditor : USEventBaseEditor
	{
		public override Rect RenderEvent(Rect myArea)
		{
			USAttachToParentEvent attachEvent = TargetEvent as USAttachToParentEvent;
	
			DrawDefaultBox(myArea);
	
			using(new GUIBeginArea(myArea))
			{
				GUILayout.Label(GetReadableEventName(), DefaultLabel);
				if (attachEvent)
					GUILayout.Label(attachEvent.parentObject?attachEvent.parentObject.name:"null", DefaultLabel);
			}
			
			return myArea;
		}
	}
}