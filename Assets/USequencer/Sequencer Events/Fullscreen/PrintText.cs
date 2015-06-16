using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Print Text")]
	public class PrintText : USEventBase 
	{
		public USEventBase.UILayer uiLayer = USEventBase.UILayer.Front;
		public string textToPrint = "";
		public Rect position = new Rect(0, 0, Screen.width, Screen.height);
		
		private string priorText = "";
		private string currentText = "";
		private bool display = false;
		
		public float printRatePerCharacter = 0.0f;
		
		public override void FireEvent()
		{	
			priorText = currentText;
			currentText = textToPrint;
			
			if(Duration > 0.0f)
				currentText = "";
			
			display = true;
		}
		
		public override void ProcessEvent(float deltaTime)
		{
			if(printRatePerCharacter <= 0.0f)
				currentText = textToPrint;
			else
			{
				int numCharacters = (int)(deltaTime / printRatePerCharacter);
				
				if(numCharacters < textToPrint.Length)
					currentText = textToPrint.Substring(0, numCharacters);
				else
					currentText = textToPrint;
			}
			
			display = true;
		}
		
		public override void StopEvent()
		{	
			UndoEvent();
		}
		
		public override void UndoEvent()
		{	
			currentText = priorText;
			display = false;
		}
		
		void OnGUI()
		{	
			if(!Sequence.IsPlaying)
				return;
	
			if(!display)
				return;
			
			int previousDepth = GUI.depth;
			
			GUI.depth = (int)uiLayer;
			GUI.Label(position, currentText);
			
			GUI.depth = previousDepth;
		}
	}
}