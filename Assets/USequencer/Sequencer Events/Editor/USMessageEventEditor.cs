using UnityEditor;
using UnityEngine;
using System.Collections;

namespace WellFired
{
	[CustomUSEditor(typeof(USMessageEvent))]
	public class USMessageEventEditor : USEventBaseEditor
	{
		public override Rect RenderEvent(Rect myArea)
		{
			USMessageEvent messageEvent = TargetEvent as USMessageEvent;
	
			myArea = DrawDurationDefinedBox(myArea);
	
			using(new GUIBeginArea(myArea))
			{
				GUILayout.Label(GetReadableEventName(), DefaultLabel);
				if (messageEvent)
					GUILayout.Label(messageEvent.message, DefaultLabel);
			}
	
			return myArea;
		}
	}
}