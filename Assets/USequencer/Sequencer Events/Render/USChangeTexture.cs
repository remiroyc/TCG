using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Change Texture")]
	[USequencerEvent("Render/Change Objects Texture")]
	public class USChangeTexture : USEventBase 
	{	
		public Texture newTexture = null;
		private Texture previousTexture = null;
		
		public override void FireEvent()
		{	
			if(!AffectedObject)
				return;
			
			if(!newTexture)
			{
				Debug.LogWarning("you've not given a texture to the USChangeTexture Event", this);
				return;
			}
			
			if(!Application.isPlaying && Application.isEditor)
			{
				previousTexture = AffectedObject.GetComponent<Renderer>().sharedMaterial.mainTexture;
				AffectedObject.GetComponent<Renderer>().sharedMaterial.mainTexture = newTexture;
			}
			else
			{
				previousTexture = AffectedObject.GetComponent<Renderer>().material.mainTexture;
				AffectedObject.GetComponent<Renderer>().material.mainTexture = newTexture;
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
			
			if(!previousTexture)
				return;
			
			if(!Application.isPlaying && Application.isEditor)
				AffectedObject.GetComponent<Renderer>().sharedMaterial.mainTexture = previousTexture;
			else
				AffectedObject.GetComponent<Renderer>().material.mainTexture = previousTexture;
				
			previousTexture = null;
		}
	}
}