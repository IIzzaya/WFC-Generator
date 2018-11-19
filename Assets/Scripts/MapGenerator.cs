using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public float slotSize = 2.0f;
	public int xCount = 16;
	public int zCount = 16;
	public int yCount = 16;

	public GameObject defaultBlock;

	public WavePrefab[] wavePrefabs;

	public WaveManager waveManager;

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;

		var cubeSize = new Vector3(xCount, yCount, zCount) * slotSize;

		Gizmos.DrawWireCube(cubeSize / 2f, cubeSize);
	}

	private void Awake() {
		Debug.Log("Awake");

		if (waveManager)
			WaveManager.RegisterWaveManager(waveManager);
		else
			WaveManager.RegisterWaveManager();

		WaveManager.RegisterWaves(wavePrefabs);

		// for (int portHash = 0; portHash < WaveManager.instance.portDictionaryLength; portHash++) {
		// 	Debug.Log(WaveManager.GetPortDetailViaPortHash(portHash));
		// 	var array = WaveManager.GetWaveViaComparedPort(portHash);
		// 	foreach (var item in array) {
		// 		Debug.Log(item.name);
		// 	}
		// }

	}

	private void Start() {
		if (defaultBlock == null) return;

		for (int i = 0; i < xCount; i++) {
			for (int j = 0; j < zCount; j++) {
				for (int k = 0; k < yCount; k++) {
					Instantiate(defaultBlock, new Vector3(i, k, j) * slotSize, Quaternion.identity);
				}
			}
		}
	}
}