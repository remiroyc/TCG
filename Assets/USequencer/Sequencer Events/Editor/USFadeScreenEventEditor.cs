using UnityEditor;
using UnityEngine;
using System.Collections;

namespace WellFired
{
	[CustomUSEditor(typeof(USFadeScreenEvent))]
	public class USFadeScreenEventEditor : USEventBaseEditor
	{
		public override Rect RenderEvent(Rect myArea)
		{
			USFadeScreenEvent FadeScreenEvent = TargetEvent as USFadeScreenEvent;
			
			float endPosition = ConvertTimeToXPos(FadeScreenEvent.FireTime + (FadeScreenEvent.Duration<=0.0f?3.0f:FadeScreenEvent.Duration));
			myArea.width = endPosition - myArea.x;
			
			DrawDefaultBox(myArea);

			using(new GUIBeginArea(myArea))
				GUILayout.Label(GetReadableEventName(), DefaultLabel);

			return myArea;
		}
	}
}