using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct Ports {
	public string up, down, right, left, front, back;
}

[Serializable]
public struct WavePrefabToGenerate {
	public GameObject prefab;
	public Vector3 position;
	public Quaternion rotation;

	public WavePrefabToGenerate(GameObject prefab, Vector3 position, Quaternion? rotation) {
		this.prefab = prefab;
		this.position = position;
		if (rotation != null) this.rotation = (Quaternion) rotation;
		else this.rotation = Quaternion.identity;
	}
}

public class WavePrefab : MonoBehaviour {

	[Header("Profile")]
	public int id;
	public string tagName;
	[HideInInspector] public string idTagName {
		get {
			return id.ToString().PadLeft(3, '0') + "_" + tagName;
		}
	}
	public int weight = 1;
	public Ports ports;

	public bool rotateAroundYAxis;
	public bool flipXYPanel;
	public bool flipZYPanel;

	/* 
		public static void LoadJson() {
			string path = Application.dataPath + "/Data/" + name + ".json";
			if (File.Exists(path)) {
				string jsonData = File.ReadAllText(path);
				var data = JsonUtility.FromJson<UnformattedBlockData>(jsonData);
				Debug.Log(data.branch);

				blockData.id = data.id;
				blockData.name = data.name;
				var v3 = data.size.Split(',');
				var size = new Vector3(float.Parse(v3[0]), float.Parse(v3[1]), float.Parse(v3[2]));
				blockData.size = size;
				// blockData.branch = data.branch;
			}
		}
	*/

#if UNITY_EDITOR

	[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
	private void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		var cubeSize = new Vector3(1f, 1f, 1f);
		Gizmos.DrawWireCube(transform.position + cubeSize / 2f, cubeSize);

		Vector3 position = transform.position;

		var style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;

		style.fontSize = 22;
		style.normal.textColor = Color.black;
		// Handles.Label(position + transform.rotation * new Vector3(1, 1, 1) / 2f, tagName, style);
		style.fontSize = 12;
		style.normal.textColor = Color.green;
		Gizmos.DrawWireSphere(position + transform.rotation * new Vector3(1, 2, 1) / 2f, 0.1f);
		Handles.Label(position + transform.rotation * new Vector3(1, 2, 1) / 2f, ports.up, style);
		Gizmos.DrawWireSphere(position + transform.rotation * new Vector3(1, 0, 1) / 2f, 0.1f);
		Handles.Label(position + transform.rotation * new Vector3(1, 0, 1) / 2f, ports.down , style);
		style.normal.textColor = Color.red;
		Gizmos.DrawWireSphere(position + transform.rotation * new Vector3(2, 1, 1) / 2f, 0.1f);
		Handles.Label(position + transform.rotation * new Vector3(2, 1, 1) / 2f, ports.right, style);
		Gizmos.DrawWireSphere(position + transform.rotation * new Vector3(0, 1, 1) / 2f, 0.1f);
		Handles.Label(position + transform.rotation * new Vector3(0, 1, 1) / 2f, ports.left, style);
		style.normal.textColor = Color.blue;
		Gizmos.DrawWireSphere(position + transform.rotation * new Vector3(1, 1, 2) / 2f, 0.1f);
		Handles.Label(position + transform.rotation * new Vector3(1, 1, 2) / 2f, ports.front, style);
		Gizmos.DrawWireSphere(position + transform.rotation * new Vector3(1, 1, 0) / 2f, 0.1f);
		Handles.Label(position + transform.rotation * new Vector3(1, 1, 0) / 2f, ports.back, style);

		// for (int i = 0; i < 6; i++) {
		// 	if (modulePrototype.Faces[i].Walkable) {
		// 		Gizmos.color = Color.red;
		// 		Gizmos.DrawLine(modulePrototype.transform.position + Vector3.down * 0.1f, modulePrototype.transform.position + modulePrototype.transform.rotation * Orientations.Rotations[i] * Vector3.forward + Vector3.down * 0.1f);
		// 	}
		// }
	}

#endif

	/*
	//Saves the tree to a file
	public void SaveFormation(string fileName) {
		//We want to change the name, if applicable!
		activeFormation.name = fileName;
		FormationJson newSave = activeFormation.ToData();
		string savedData = JsonUtility.ToJson(newSave);
		string path = Application.dataPath + "/Data/RockTypes.json";
		if (File.Exists(path) == true) {
			string[] rockData = File.ReadAllLines(path);
			//First, the tree data itself, not including the branches
			bool replaced = false;
			for (int iProfile = 0; iProfile < rockData.Length; ++iProfile) {
				//Read in the branch profile
				FormationJson existingData = JsonUtility.FromJson<FormationJson>(rockData[iProfile]);
				if (existingData.name.ToLower() == newSave.name.ToLower()) {
					rockData[iProfile] = savedData;
					replaced = true;
					break;
				}
			}
			//If we replaced, write everything back in
			//Otherwise, append
			if (replaced == true) {
				File.WriteAllLines(path, rockData);
			} else {
				//Don't forget the newline
				File.AppendAllText(path, "\n" + savedData);
			}
		} else {
			File.WriteAllText(path, savedData);
		}

		//Don't forget the branches!
		path = Application.dataPath + "/Data/Rocks/" + activeFormation.name + ".json";
		//We don't care, since this is the entire tree definition, so we'll overwrite anything we find
		string[] layerData = new string[layers.Count];
		for (int iB = 0; iB < layerData.Length; ++iB) {
			layerData[iB] = JsonUtility.ToJson(layers[iB].ToData());
		}
		//Now write it all down
		File.WriteAllLines(path, layerData);
	}

	//Loads a particular type of rock formation
	private void LoadRock(string formation) {
		//Clear this out
		layers.Clear();
		//Load the JSON layer profiles for this species
		string[] jsonData = File.ReadAllLines(Application.dataPath + "/Data/Rocks/" + formation + ".json");
		colors = new Texture2D(jsonData.Length, 1);

		//Scan in all the branch type data
		for (int iProfile = 0; iProfile < jsonData.Length; ++iProfile) {
			//Read in the branch profile
			RockJson layerData = JsonUtility.FromJson<RockJson>(jsonData[iProfile]);
			//According to documentation, the constructor for the object isn't executed during FromJson, so I need a separate function for parsing
			RockLayer newLayer = new RockLayer(layerData);
			//Add the color appropriately
			colors.SetPixel(iProfile, 1, newLayer.color);
			//Add to the list
			layers.Add(newLayer);
		}

		//Apply the colors, confirming the selections made above
		colors.Apply();
	}
	 */
}