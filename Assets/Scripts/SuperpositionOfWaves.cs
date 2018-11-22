using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SuperpositionOfWaves {

	public bool isObserved {
		get { return unCollaspedWavesCount > 1 ? false : true; }
	}

	public int? observedWaveHashCode {
		get {
			if (!isObserved) return null;
			if (_obervedWaveHashCode == null) {
				for (int i = 0; i < unCollaspedWaves.Length; i++) {
					if (unCollaspedWaves[i] == false) {
						_obervedWaveHashCode = i;

					}
				}
			}
			return _obervedWaveHashCode;
		}
	}
	public int? _obervedWaveHashCode;

	public int unCollaspedWavesCount;
	public bool[] unCollaspedWaves; // false means unCollasped

	public int[] unCollaspedPorts;

	public bool toCollaspe;
	public List<int> toCollaspeArray = new List<int>();
	public bool[] toCollaspeWavesHashCode; // false means unMarked

	public SuperpositionOfWaves(int waveCount, int[] unCollaspedPortsPreset) {
		unCollaspedWavesCount = waveCount;
		unCollaspedWaves = new bool[waveCount];
		toCollaspeWavesHashCode = new bool[waveCount];
		unCollaspedPorts = (int[]) unCollaspedPortsPreset.Clone();
	}

	public void MarkToCollaspeWave(int[] waveHashCodes, ref bool shouldAddToArray) {
		if (isObserved) return;

		var flag = false;
		foreach (var item in waveHashCodes) {
			if (!toCollaspeWavesHashCode[item]) {
				toCollaspeArray.Add(item);
				toCollaspeWavesHashCode[item] = true;

				flag = true;
			}
		}

		if (flag) {
			if (toCollaspe) {
				shouldAddToArray = false;
			} else {
				toCollaspe = true;
				shouldAddToArray = true;
			}
		}
	}

	public void ClearToCollaspeArray() {
		toCollaspe = false;
		toCollaspeArray.Clear();
		for (int i = 0; i < toCollaspeWavesHashCode.Length; i++) {
			toCollaspeWavesHashCode[i] = false;
		}
	}

	public int[] GetToCollaspeWaves() {
		return toCollaspeArray.ToArray();
	}

	public void CollaspeWave(Wave wave, IntVector3 coord, ref CollaspeSystem system) {
		if (!unCollaspedWaves[wave.hashCode]) {
			var sixSidePorts = wave.ports;
			// Debug.Log(wave);

			for (int i = 0; i < sixSidePorts.Length; i++) {

				unCollaspedPorts[sixSidePorts[i]]--;
				if (unCollaspedPorts[sixSidePorts[i]] == 0) {
					system.CollaspeNearbyWaveSlot(coord, (EPortDirection) i, sixSidePorts[i]);
				}
			}

			unCollaspedWaves[wave.hashCode] = true;
			unCollaspedWavesCount--;
		}
	}

	public int GetARandomUnCollaspedWaveHashCode() {
		var unCollaspedWavesList = new List<int>();
		for (int i = 0; i < unCollaspedWaves.Length; i++) {
			if (!unCollaspedWaves[i]) {
				unCollaspedWavesList.Add(i);
			}
		}

		var randomIndex = Random.Range(0, unCollaspedWavesCount);
		return unCollaspedWavesList[randomIndex];
	}

	public int[] GetAllUnCollaspedWavesExcept(int waveHashCode) {
		var waveHashCodeList = new List<int>();
		for (int i = 0; i < unCollaspedWaves.Length; i++) {
			if (!unCollaspedWaves[i])
				if (i != waveHashCode)
					waveHashCodeList.Add(i);
		}
		return waveHashCodeList.ToArray();
	}
}