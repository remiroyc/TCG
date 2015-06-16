using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Stop Emitter (Legacy)")]
	[USequencerEvent("Particle System/Stop Emitter")]
	public class USParticleEmitterStopEvent : USEventBase 
	{
		public override void FireEvent()
		{	
			ParticleSystem particleSystem = AffectedObject.GetComponent<ParticleSystem>();
			if(!particleSystem)
			{
				Debug.Log("Attempting to emit particles, but the object has no particleSystem USParticleEmitterStartEvent::FireEvent");
				return;
			}
			
			particleSystem.Stop();
		}
		
		public override void ProcessEvent(float deltaTime)
		{
			
		}
	}
}