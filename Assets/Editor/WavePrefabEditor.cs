using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WavePrefab))]
public class WavePrefabEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		// WavePrefab wavePrefab = (WavePrefab) target;
		// if (GUILayout.Button("Distribute")) {
		// 	int i = 0;
		// 	foreach (Transform transform in wavePrefab.transform.parent) {
		// 		transform.localPosition = Vector3.forward * i * 1f * 2f;
		// 		i++;
		// 	}
		// }

		// if (GUILayout.Button("Guess connectors")) {
		// 	foreach (var face in wavePrefab.Faces) {
		// 		face.Fingerprint = null;
		// 	}
		// 	wavePrefab.GuessConnectors();
		// }

		// if (GUILayout.Button("Reset connectors")) {
		// 	foreach (var face in wavePrefab.Faces) {
		// 		face.ResetConnector();
		// 	}
		// }

		// if (GUILayout.Button("Reset exlusion rules in all prototypes")) {
		// 	foreach (var prototype in wavePrefab.transform.parent.GetComponentsInChildren<ModulePrototype>()) {
		// 		foreach (var face in prototype.Faces) {
		// 			face.ExcludedNeighbours = new ModulePrototype[0];
		// 		}
		// 	}
		// }
	}
}