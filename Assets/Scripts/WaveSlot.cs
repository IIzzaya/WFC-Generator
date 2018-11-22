using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaveSlot {
	CollaspeSystem collaspeSystem;
	public IntVector3 coordinate;
	public SuperpositionOfWaves superpositionOfWaves;
	public GameObject wavePrefab {
		get {
			if (!superpositionOfWaves.isObserved) return null;

			var waveHashCode = superpositionOfWaves.observedWaveHashCode;
			_wavePrefab = collaspeSystem.waveManager.GetWavePrefabViaHashCode((int) waveHashCode);
			return _wavePrefab;

		}
	}
	public GameObject _wavePrefab;

	public void CollaspeWave(Wave wave) {
		superpositionOfWaves.CollaspeWave(wave, coordinate, ref collaspeSystem);
	}
	public void CollaspeWave(Wave[] waves) {
		foreach (var item in waves) {
			// Debug.Log(item);
			superpositionOfWaves.CollaspeWave(item, coordinate, ref collaspeSystem);
		}
	}

	public void ExecuteWaveCollaspe() {
		// Debug.Log(coordinate);
		var toCollaspeArray = superpositionOfWaves.GetToCollaspeWaves();
		var waveManager = collaspeSystem.waveManager;
		var waves = waveManager.GetWaveViaHashCode(toCollaspeArray);
		// foreach (var item in waves) {
		// 	Debug.Log(item);
		// }

		CollaspeWave(waves);
		superpositionOfWaves.ClearToCollaspeArray();

		// Debug.Log(coordinate.ToString() + ": Execute wave collaspe");
	}

	public void MarkToCollaspeWave(int[] waveHashCodes) {
		// Debug.Log("MarkToCOllaspeWave Called");
		var shouldAddToArray = false;
		superpositionOfWaves.MarkToCollaspeWave(waveHashCodes, ref shouldAddToArray);
		if (shouldAddToArray) {
			collaspeSystem.AddToCollaspeArray(this);
		}
	}

	public void ObserveARandomWave() {
		var randomWaveHashCode = superpositionOfWaves.GetARandomUnCollaspedWaveHashCode();
		// Add all other waves into toCollaspeArray
		var toCollaspedWaveHashCodes = superpositionOfWaves.GetAllUnCollaspedWavesExcept(randomWaveHashCode);
		MarkToCollaspeWave(toCollaspedWaveHashCodes);
	}

	public WaveSlot(IntVector3 coordinate, CollaspeSystem system, int waveCount, int[] unCollaspedPortsPreset) {
		this.coordinate = coordinate;
		this.collaspeSystem = system;
		superpositionOfWaves = new SuperpositionOfWaves(waveCount, unCollaspedPortsPreset);
	}
}