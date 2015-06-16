using UnityEditor;
using UnityEngine;
using System.Collections;

namespace WellFired
{
	[CustomUSEditor(typeof(USSpawnPrefabEvent))]
	public class USSpawnPrefabEventEditor : USEventBaseEditor
	{
		public override Rect RenderEvent(Rect myArea)
		{
			USSpawnPrefabEvent spawnEvent = TargetEvent as USSpawnPrefabEvent;
	
			DrawDefaultBox(myArea);
	
			using(new GUIBeginArea(myArea))
			{
				GUILayout.Label("Spawn : ", DefaultLabel);
				if (spawnEvent)
					GUILayout.Label(spawnEvent.spawnPrefab?spawnEvent.spawnPrefab.name:"null", DefaultLabel);
				GUILayout.Label("At : ", DefaultLabel);
				GUILayout.Label(spawnEvent.spawnTransform?spawnEvent.spawnTransform.name:"Identity", DefaultLabel);
			}
	
			return myArea;
		}
	}
}