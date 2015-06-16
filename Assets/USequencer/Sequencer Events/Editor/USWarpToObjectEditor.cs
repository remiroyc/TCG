using UnityEditor;
using UnityEngine;
using System.Collections;

namespace WellFired
{
	[CustomUSEditor(typeof(USWarpToObject))] 
	public class USWarpToObjectEditor : USEventBaseEditor
	{
		public override Rect RenderEvent(Rect myArea)
		{
			USWarpToObject warpEvent = TargetEvent as USWarpToObject;
			
			myArea.width += 10.0f;
			DrawDefaultBox(myArea);
			
			using(new GUIBeginArea(myArea))
			{
				GUILayout.Label(GetReadableEventName(), DefaultLabel);
				if (warpEvent)
				{
					GUILayout.Label(warpEvent.objectToWarpTo?warpEvent.objectToWarpTo.name:"null", DefaultLabel);
					GUILayout.Label(warpEvent.useObjectRotation?"Using Warp Rotation":"Keep Original Rotation", DefaultLabel);
				}
			}
	
			return myArea;
		}
	}
}