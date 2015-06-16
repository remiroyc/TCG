using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Send Message")]
	[USequencerEvent("Signal/Send Message")]
	public class USSendMessageEvent : USEventBase 
	{
		public GameObject receiver = null;
		public string action = "OnSignal";
		
		public override void FireEvent()
		{
			if(!Application.isPlaying)
				return;
			
			if (receiver)
				receiver.SendMessage (action);
			else
				Debug.LogWarning ("No receiver of signal \""+action+"\" on object "+receiver.name+" ("+receiver.GetType().Name+")", receiver);
		}
		
		public override void ProcessEvent(float deltaTime)
		{
			
		}
	}
}