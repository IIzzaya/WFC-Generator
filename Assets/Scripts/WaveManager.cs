using System;
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

[Serializable]
public struct Wave {
	public int id;
	public string name;
	public int weight;
	public int[] ports;
}

public class WaveManager : MonoBehaviour {
	public static WaveManager instance;
	private Wave[] waves;

	public int waveListLength = 0;
	public List<Wave> waveList = new List<Wave>();
	public List<GameObject> wavePrefabList = new List<GameObject>();

	public int portDictionaryLength = 0;

	public Dictionary<string, int> portDictionary = new Dictionary<string, int>();
	public List<List<int>> waveBindPort = new List<List<int>>();
	public List<List<int>> waveBindComparedPort = new List<List<int>>();

	private void ConvertWavePrefabToWave(WavePrefab prefab, ref Wave wave) {
		wave.id = waveListLength;
		wave.name = prefab.name;
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
				waveBindPort[value].Add(wave.id);
			} else {
				portDictionary.Add(portStr, portDictionaryLength);
				newPorts[i] = portDictionaryLength;
				waveBindComparedPort.Add(new List<int>());
				waveBindPort.Add(new List<int>());
				waveBindPort[portDictionaryLength].Add(wave.id);
				portDictionaryLength++;
			}
		}

		if (prefab.rotateAroundYAxis) {
			
		}

		wave.ports = newPorts;
	}

	private void AddWaveBindComparedPort() {
		// foreach (var item in portDictionary)
		// {
		// 	Debug.Log(item.Key);
		// }

		foreach (var item in portDictionary) {
			int i = Int32.Parse(item.Key[item.Key.Length - 1] + "");
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

	public static void RegisterWaveManager() {
		instance = new WaveManager();
	}

	public static void RegisterWaveManager(WaveManager waveManager) {
		if (instance == null)
			instance = waveManager;
		else if (instance != waveManager)
			instance = waveManager;
	}

	public static void RegisterWaves(WavePrefab[] wavePrefabs) {

		foreach (var item in wavePrefabs) {
			var newWave = new Wave();
			instance.ConvertWavePrefabToWave(item, ref newWave);
			instance.waveList.Add(newWave);
			instance.wavePrefabList.Add(item.gameObject);
			instance.waveListLength++;
		}
		instance.AddWaveBindComparedPort();

		Debug.Log("[WM]: RegisterWaves Successful");
	}

	public static Wave GetWaveViaID(int id) {
		return instance.waveList[id];
	}

	public static GameObject GetWavePrefabViaID(int id) {
		return instance.wavePrefabList[id];
	}

	public static string GetPortDetailViaPortHash(int portHash) {
		foreach (var item in instance.portDictionary)
		{
			if (item.Value == portHash)
				return item.Key;
		}
		return "";
	}

	public static Wave[] GetWaveViaComparedPort(int port) {
		var waves = new List<Wave>();
		foreach (var item in instance.waveBindComparedPort[port]) {
			waves.Add(GetWaveViaID(item));
		}
		return waves.ToArray();
	}
}