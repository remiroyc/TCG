using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Change Color")]
	[USequencerEvent("Render/Change Objects Color")]
	public class USChangeColor : USEventBase 
	{	
		public Color newColor;
		private Color previousColor;
		
		public override void FireEvent()
		{	
			if(!AffectedObject)
				return;
			
			if(!Application.isPlaying && Application.isEditor)
			{
				previousColor = AffectedObject.GetComponent<Renderer>().sharedMaterial.color;
				AffectedObject.GetComponent<Renderer>().sharedMaterial.color = newColor;
			}
			else
			{
				previousColor = AffectedObject.GetComponent<Renderer>().material.color;
				AffectedObject.GetComponent<Renderer>().material.color = newColor;
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
			if(!AffectedObject)
				return;
			
			if(!Application.isPlaying && Application.isEditor)
				AffectedObject.GetComponent<Renderer>().sharedMaterial.color = previousColor;
			else
				AffectedObject.GetComponent<Renderer>().material.color = previousColor;
		}
	}
}