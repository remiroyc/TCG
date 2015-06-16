using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Spawn Prefab")]
	[USequencerEvent("Spawn/Spawn Prefab")]
	public class USSpawnPrefabEvent : USEventBase 
	{
		public GameObject spawnPrefab = null;
		public Transform spawnTransform = null;
		public override void FireEvent()
		{
			if(!spawnPrefab)
			{
				Debug.Log("Attempting to spawn a prefab, but you haven't given a prefab to the event from USSpawnPrefabEvent::FireEvent");
				return;
			}
			
			if(spawnTransform)
				GameObject.Instantiate(spawnPrefab, spawnTransform.position, spawnTransform.rotation);
			else
				GameObject.Instantiate(spawnPrefab, Vector3.zero, Quaternion.identity);
		}
		
		public override void ProcessEvent(float deltaTime)
		{
			
		}
	}
}