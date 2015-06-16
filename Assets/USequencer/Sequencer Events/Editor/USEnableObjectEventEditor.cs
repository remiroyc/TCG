using UnityEditor;
using UnityEngine;
using System.Collections;

namespace WellFired
{
	[CustomUSEditor(typeof(USEnableObjectEvent))]
	public class USEnableObjectEventEditor : USEventBaseEditor
	{
		public override Rect RenderEvent(Rect myArea)
		{
			USEnableObjectEvent toggleEvent = TargetEvent as USEnableObjectEvent;
	
			DrawDefaultBox(myArea);

			using(new GUIBeginArea(myArea))
			{
				GUILayout.Label(toggleEvent.enable?"Enable : ":"Disable : ", DefaultLabel);
				GUILayout.Label(toggleEvent.AffectedObject.name, DefaultLabel);
			}
	
			return myArea;
		}
	}
}