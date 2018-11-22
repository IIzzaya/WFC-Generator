using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum EPortDirection {
	up = 0,
	down = 1,
	right = 2,
	left = 3,
	front = 4,
	back = 5
}

[System.Serializable]
public struct Wave {
	public int hashCode;
	public string name;
	public int weight;
	public int[] ports;

	public override string ToString() {
		var str = "";
		str += hashCode + "," + name + ": (";
		foreach (var item in ports) {
			str += item + ", ";
		}
		str += ")";
		return str;
	}
}

public class WaveManager : MonoBehaviour {

	[HideInInspector] public CollaspeSystem collaspeSystem;
	[HideInInspector] public MapGenerator mapGenerator;

	// public static WaveManager instance;
	private Wave[] waves;

	public int waveListLength = 0;
	public List<Wave> waveList = new List<Wave>();
	public List<GameObject> wavePrefabList = new List<GameObject>();

	public int portDictionaryLength = 0;
	public Dictionary<string, int> portDictionary = new Dictionary<string, int>();
	public List<List<int>> waveBindPort = new List<List<int>>();
	public List<List<int>> waveBindComparedPort = new List<List<int>>();

	public int[] unCollaspedPortsPreset;

	public IntVector3 waveSlotGridStartCoordinate;
	public IntVector3 waveSlotGridEndCoordinate;
	public WaveSlot[, , ] waveSlotGrid;
	public List<IntVector3> waveSlotCoordinateList;
	public WaveSlot[, , ] CreateWaveSlotGrid(IntVector3 start, IntVector3 end) {
		waveSlotGridStartCoordinate = new IntVector3(Mathf.Min(start.X, end.X), Mathf.Min(start.Y, end.Y), Mathf.Min(start.Z, end.Z));
		waveSlotGridEndCoordinate = new IntVector3(Mathf.Max(start.X, end.X), Mathf.Max(start.Y, end.Y), Mathf.Max(start.Z, end.Z));

		var size = waveSlotGridEndCoordinate - waveSlotGridStartCoordinate;

		waveSlotGrid = new WaveSlot[size.X, size.Y, size.Z];
		waveSlotCoordinateList = new List<IntVector3>();

		for (int i = 0; i < size.X; i++) {
			for (int j = 0; j < size.Y; j++) {
				for (int k = 0; k < size.Z; k++) {
					waveSlotGrid[i, j, k] = new WaveSlot(new IntVector3(i, j, k), collaspeSystem, waveListLength, unCollaspedPortsPreset);
					waveSlotCoordinateList.Add(new IntVector3(i, j, k));
				}
			}
		}

		return waveSlotGrid;
	}

	public WaveSlot GetRandomUnObservedWaveSlot() {
		// Pick a random position;
		while (waveSlotCoordinateList.Count > 0) {
			var randomValue = Random.Range(0, waveSlotCoordinateList.Count);
			var randomWaveSlot = GetWaveSlotWithCoordinate(waveSlotCoordinateList[randomValue]);
			if (randomWaveSlot.superpositionOfWaves.isObserved) {
				waveSlotCoordinateList.RemoveAt(randomValue);
			} else {
				// Debug.Log(randomWaveSlot.coordinate);
				return randomWaveSlot;
			}
		}

		return null;
	}

	public WaveSlot GetWaveSlotWithCoordinate(IntVector3 coord) {
		// Debug.Log(coord);
		return waveSlotGrid[coord.X, coord.Y, coord.Z];
	}

	void ConvertWavePrefabToWave(WavePrefab prefab, ref Wave wave) {
		wave.hashCode = waveListLength;
		wave.name = prefab.tagName;
		wave.weight = prefab.weight;
		var ports = new string[6];
		ports[EPortDirection.up.GetHashCode()] = prefab.ports.up;
		ports[EPortDirection.down.GetHashCode()] = prefab.ports.down;
		ports[EPortDirection.right.GetHashCode()] = prefab.ports.right;
		ports[EPortDirection.left.GetHashCode()] = prefab.ports.left;
		ports[EPortDirection.front.GetHashCode()] = prefab.ports.front;
		ports[EPortDirection.back.GetHashCode()] = prefab.ports.back;

		var newPorts = new int[6];
		for (int i = 0; i < 6; i++) {
			var portStr = ports[i] + "@" + i;

			if (portDictionary.ContainsKey(portStr)) {
				var value = portDictionary[portStr];
				newPorts[i] = value;
				waveBindPort[value].Add(wave.hashCode);
			} else {
				portDictionary.Add(portStr, portDictionaryLength);
				newPorts[i] = portDictionaryLength;
				waveBindComparedPort.Add(new List<int>());
				waveBindPort.Add(new List<int>());
				waveBindPort[portDictionaryLength].Add(wave.hashCode);
				portDictionaryLength++;
			}
		}

		if (prefab.rotateAroundYAxis) {

		}

		wave.ports = newPorts;
	}

	void AddWaveBindComparedPort() {
		// foreach (var item in portDictionary)
		// {
		// 	Debug.Log(item.Key);
		// }

		foreach (var item in portDictionary) {
			int i = System.Int32.Parse(item.Key[item.Key.Length - 1] + "");
			if (i % 2 == 0)
				i++;
			else i--;
			var newKey = item.Key.Substring(0, item.Key.Length - 1) + i.ToString();
			// Debug.Log(item.Key);
			// Debug.Log(newKey);
			// Debug.Log(portDictionary[newKey]);
			if (portDictionary.ContainsKey(newKey)) {
				var value = portDictionary[newKey];
				waveBindComparedPort[value].AddRange(waveBindPort[item.Value]);
			}

		}

		// var j = 0;
		// foreach (var item in waveBindPort) {
		// 	var str = j + ": ";
		// 	foreach (var listItem in item) {
		// 		str += listItem + ", ";
		// 	}
		// 	Debug.Log(str);
		// 	j++;
		// }

		// Debug.Log("---");
		// j = 0;
		// foreach (var item in waveBindComparedPort) {
		// 	var str = j + ": ";
		// 	foreach (var listItem in item) {
		// 		str += listItem + ", ";
		// 	}
		// 	Debug.Log(str);
		// 	j++;
		// }
	}

	public void CalculateUnCollaspedPortPreset() {
		unCollaspedPortsPreset = new int[portDictionaryLength];

		foreach (var item in waveList) {
			foreach (var subItem in item.ports) {
				unCollaspedPortsPreset[subItem]++;
			}
		}

		// var str = "";
		// foreach (var item in unCollaspedPortsPreset)
		// {
		// 	str += item + ", ";
		// }
		// Debug.Log(str);
	}

	// public SuperpositionOfWaves superpositionOfWavesPreset;
	// public void CreateSuperpositionOfWavesPreset() {
	// 	superpositionOfWavesPreset = new SuperpositionOfWaves(waveListLength, unCollaspedPortsPreset);
	// }

	// public static void RegisterWaveManager() {
	// 	instance = new WaveManager();
	// }

	// public static void RegisterWaveManager(WaveManager waveManager) {
	// 	if (instance == null)
	// 		instance = waveManager;
	// 	else if (instance != waveManager)
	// 		instance = waveManager;
	// }

	public void RegisterWaves(WavePrefab[] wavePrefabs) {

		foreach (var item in wavePrefabs) {
			var newWave = new Wave();
			ConvertWavePrefabToWave(item, ref newWave);
			waveList.Add(newWave);
			wavePrefabList.Add(item.gameObject);
			waveListLength++;
		}
		AddWaveBindComparedPort();
		CalculateUnCollaspedPortPreset();

		Debug.Log("[WM]: RegisterWaves Successful");
	}

	public Wave GetWaveViaHashCode(int hashCode) {
		return waveList[hashCode];
	}

	public Wave[] GetWaveViaHashCode(int[] hashCode) {
		var waveList = new List<Wave>();
		foreach (var item in hashCode) {
			waveList.Add(GetWaveViaHashCode(item));
		}
		return waveList.ToArray();
	}

	public GameObject GetWavePrefabViaHashCode(int hashCode) {
		return wavePrefabList[hashCode];
	}

	public string GetPortDetailViaPortHash(int portHash) {
		foreach (var item in portDictionary) {
			if (item.Value == portHash)
				return item.Key;
		}
		return "";
	}

	public Wave[] GetWaveViaComparedPort(int port) {
		var waves = new List<Wave>();
		foreach (var item in waveBindComparedPort[port]) {
			waves.Add(GetWaveViaHashCode(item));
		}
		return waves.ToArray();
	}

	public int[] GetWaveHashCodeViaComparedPort(int port) {
		return waveBindComparedPort[port].ToArray();
	}
}