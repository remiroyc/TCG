using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using WaterPlusEditorInternal;
#endif

//Max depth = 25m
[ExecuteInEditMode]
public class WaterPlus : EditorWindow 
{
	private const string waterSystemPath = "WaterPlus/";		//NOTE: You may change this path as per your convenience.
	
	//private bool doneBakingLabel = false;
	
	[MenuItem("Window/Water+")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow( typeof(WaterPlus) );
	}
	
	//private RuntimePlatform platform;
	
	void Start() {
		#if UNITY_EDITOR
		//WaterPlusBaker.editorWindow = this;
		WaterPlusBaker.bakeStartTime = -100.0f;
		WaterPlusBaker.bakeStage = -1;
		
		//platform = Application.platform;
		#endif
	}
	
	void OnGUI()
	{
		Color waterUIColor = new Color(100.0f / 255.0f, 200.0f / 255.0f, 1.0f);
		GUI.backgroundColor = waterUIColor;
		
		GUIStyle logoLabelStyle = new GUIStyle();
		logoLabelStyle.normal.textColor = waterUIColor;
		logoLabelStyle.fontSize = 20;
		logoLabelStyle.alignment = TextAnchor.MiddleCenter;
		
		int yPos = 5;
		
		GUI.Label(new Rect(10, yPos, 300, 30), "Water+", logoLabelStyle);
		
		//yPos += 20;
		
		GUI.Label(new Rect(10, yPos += 20, 300, 20), "__________________________________________");
		
		#if UNITY_EDITOR
		WaterPlusBaker.waterSurfaceTransform = EditorGUI.ObjectField(new Rect(10, yPos += 20,300,20),
                "Water surface",
                WaterPlusBaker.waterSurfaceTransform,
                typeof( Transform ) ) as Transform;
		
		GUIStyle separatorLabelStyle = new GUIStyle();
		separatorLabelStyle.normal.textColor = Color.white;
		
		//Lightmaps
		
		int actionNumber = 1;
		
		//if (platform == RuntimePlatform.WindowsEditor) {
			GUI.Label(new Rect(10, yPos += 25, 300, 20), "_________________Lightmaps_________________", separatorLabelStyle);
			WaterPlusBaker.lightmapWetnessHeightString = EditorGUI.TextField( new Rect(10, yPos += 25, 300, 20), "Wetness height", WaterPlusBaker.lightmapWetnessHeightString);
			
			WaterPlusBaker.lightmapWetnessAmountString = EditorGUI.TextField( new Rect(10, yPos += 25, 300, 20), "Wetness amount", WaterPlusBaker.lightmapWetnessAmountString);
			
			if (GUI.Button(new Rect(10, yPos += 25, 300, 30), "(" + (actionNumber++) + ")Update Lightmaps") ) {
				WaterPlusBaker.UpdateLightmaps();
			}
		//}
		
		//yPos += 60;
		
		if (GUI.Button(new Rect(10, yPos += 35, 300, 30), "(" + (actionNumber++) + ")Build Cubemap") ) { 
			WaterPlusBaker.BakeCubemap();
		}
		
		//yPos += 30;
		
		WaterPlusBaker.terrainLayerMask = LayerMaskField(new Rect(10, yPos += 50, 300, 20), "Terrain layers: ", WaterPlusBaker.terrainLayerMask);
		WaterPlusBaker.waterMapResString = EditorGUI.TextField( new Rect(10, yPos += 25, 300, 20), "Water maps resolution", WaterPlusBaker.waterMapResString);
		/*WaterPlusBaker.refractionLayerMask = LayerMaskField(new Rect(10, yPos += 25, 300, 20), "Refraction layers: ", WaterPlusBaker.refractionLayerMask);
		//EditorGUI.LayerField( new Rect(10, 115, 300, 20), "Terrain layer:", terrainLayer);
		
		
		WaterPlusBaker.refractionMapResString = EditorGUI.TextField( new Rect(10, yPos += 25, 300, 20), "Refraction map resolution",WaterPlusBaker.refractionMapResString);
		
		WaterPlusBaker.refractionMapScaleString = EditorGUI.TextField( new Rect(10, yPos += 25, 300, 20), "Refraction map scale", WaterPlusBaker.refractionMapScaleString);
		
		//projectRefractionTexture = EditorGUI.Toggle( new Rect(10, 190, 300, 20), "Project refraction texture", projectRefractionTexture);
		
		//yPos += 110;
		
		WaterPlusBaker.shouldProjectRefractionMap = EditorGUI.Toggle( new Rect(10, yPos += 25, 300, 20), "Project refractions (slow)", WaterPlusBaker.shouldProjectRefractionMap);
		*/
		//yPos += 25;
		
		if (GUI.Button(new Rect(10, yPos += 25, 300, 30), "(" + (actionNumber++) + ")Bake Water Maps") ) {
			//Init the baker
			WaterPlusBaker.waterSystemPath = waterSystemPath;
			//WaterPlusBaker.editorWindow = this;
			
			WaterPlusBaker.bakeProgress = 0.0f;
			WaterPlusBaker.bakeStartTime = Time.realtimeSinceStartup + 1.0f;
		}
		
		GUIStyle infoLabelStyle = new GUIStyle();
		
		//yPos += 35;
		
		if (WaterPlusBaker.bakeProgress >= 1.0f) {
			infoLabelStyle.normal.textColor = Color.green;
			GUI.Label(new Rect(10, yPos += 35, 300, 20), "Bake completed successfully.", infoLabelStyle);
			//yPos += 25;
		} else if (WaterPlusBaker.bakeProgress >= 0.0f){
			EditorGUI.ProgressBar( new Rect(10, yPos += 35, 300, 20), WaterPlusBaker.bakeProgress, WaterPlusBaker.bakingTask);
			//yPos += 25;
		}
		
		infoLabelStyle.normal.textColor = Color.yellow;
		GUI.Label(new Rect(10, yPos += 35, 300, 60), "Bake might take some time, depending\n" +
												"on the resolution and scene size.\n", infoLabelStyle);
		
		//Flowmaps
		GUI.Label(new Rect(10, yPos += 65, 300, 20), "_________________Flowmaps_________________", separatorLabelStyle);
		
		if (GUI.Button(new Rect(10, yPos += 25, 300, 30), "Normalize and adjust speed") ) {
			WaterPlusBaker.AdjustFlowmap();
		}
		/*#else
		GUIStyle infoLabelStyle = new GUIStyle();
		infoLabelStyle.normal.textColor = Color.red;
		GUI.Label(new Rect(10, yPos += 35, 300, 80), "Baking of water maps is supported only in\n" +
													"standalone platforms.\n" +
													"Please switch to a standalone in build settings.\n" +
													"After you're done baking, you may switch back.", infoLabelStyle);*/
		#endif
	}
	
	#if UNITY_EDITOR
	void Update() {
		//WaterPlusBaker.editorWindow = this;	
		WaterPlusBaker.EditorUpdate();
	}
	#endif
	
	#region Helpers
	public static LayerMask LayerMaskField (Rect position, string label, LayerMask selected) {
	    return LayerMaskField (position, label, selected, true);
	}
	
	public static LayerMask LayerMaskField (Rect position, string label, LayerMask selected, bool showSpecial) {
	
	    List<string> layers = new List<string>();
	    List<int> layerNumbers = new List<int>();
	
	    string selectedLayers = "";
	
	    for (int i=0;i<32;i++) {
	
	       string layerName = LayerMask.LayerToName (i);
	
	       if (layerName != "") {
	         if (selected == (selected | (1 << i))) {
	
	          if (selectedLayers == "") {
	              selectedLayers = layerName;
	          } else {
	              selectedLayers = "Mixed";
	          }
	         }
	       }
	    }
	
	    //EventType lastEvent = Event.current.type;
	
	    if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand) {
	       if (selected.value == 0) {
	         layers.Add ("Nothing");
	       } else if (selected.value == -1) {
	         layers.Add ("Everything");
	       } else {
	         layers.Add (selectedLayers);
	       }
	       layerNumbers.Add (-1);
	    }
	
	    if (showSpecial) {
	       layers.Add ((selected.value == 0 ? "[X] " : "     ") + "Nothing");
	       layerNumbers.Add (-2);
	
	       layers.Add ((selected.value == -1 ? "[X] " : "     ") + "Everything");
	       layerNumbers.Add (-3);
	    }
	
	    for (int i=0;i<32;i++) {
	
	       string layerName = LayerMask.LayerToName (i);
	
	       if (layerName != "") {
	         if (selected == (selected | (1 << i))) {
	          layers.Add ("[X] "+layerName);
	         } else {
	          layers.Add ("     "+layerName);
	         }
	         layerNumbers.Add (i);
	       }
	    }
	
	    bool preChange = GUI.changed;
	
	    GUI.changed = false;
	
	    int newSelected = 0;
	
	    if (Event.current.type == EventType.MouseDown) {
	       newSelected = -1;
	    }
	
	    newSelected = EditorGUI.Popup(position, label,newSelected,layers.ToArray(),EditorStyles.layerMaskField);
		//EditorGUI.MaskField(position, newSelected, layers.ToArray();
	
	    if (GUI.changed && newSelected >= 0) {
	       //newSelected -= 1;
	
	       //Debug.Log (lastEvent +" "+newSelected + " "+layerNumbers[newSelected]);
	
	       if (showSpecial && newSelected == 0) {
	         selected = 0;
	       } else if (showSpecial && newSelected == 1) {
	         selected = -1;
	       } else {
	
	         if (selected == (selected | (1 << layerNumbers[newSelected]))) {
	          selected &= ~(1 << layerNumbers[newSelected]);
	          //Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To False "+selected.value);
	         } else {
	          //Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To True "+selected.value);
	          selected = selected | (1 << layerNumbers[newSelected]);
	         }
	       }
	    } else {
	       GUI.changed = preChange;
	    }
	
	    return selected;
	}
	#endregion
}
