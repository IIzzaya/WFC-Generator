using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// BFS

public class CollaspeSystem : MonoBehaviour {

    [HideInInspector] public MapGenerator mapGenerator;
    [HideInInspector] public WaveManager waveManager;

    public WavePrefabToGenerate[] wavePrefabToGenerate;

    public List<WaveSlot> toCollaspeWaveSlotArray = new List<WaveSlot>();

    public List<WaveSlot> finishedCollaspeWaveSlotArray = new List<WaveSlot>();

    public void StartCollaspe() {
        var count = 0;
        while (toCollaspeWaveSlotArray.Count > 0) {
            var toCollaspeWaveSlot = toCollaspeWaveSlotArray[0];

            toCollaspeWaveSlot.ExecuteWaveCollaspe();

            finishedCollaspeWaveSlotArray.Add(toCollaspeWaveSlot);
            if (toCollaspeWaveSlot.wavePrefab) {
                Debug.Log(toCollaspeWaveSlot.coordinate + ": " + toCollaspeWaveSlot.wavePrefab.name);
                var prefab = new WavePrefabToGenerate(toCollaspeWaveSlot.wavePrefab, toCollaspeWaveSlot.coordinate.ToVector3(), Quaternion.identity);
                var obj = Instantiate(prefab.prefab, prefab.position, prefab.rotation);
                Destroy(obj.GetComponent<WavePrefab>());
            }
            toCollaspeWaveSlotArray.RemoveAt(0);

            count++;
        }
        // Debug.Log(count);
    }

    public void AddToCollaspeArray(WaveSlot waveSlot) {
        toCollaspeWaveSlotArray.Add(waveSlot);
    }

    public bool CollaspeNearbyWaveSlot(IntVector3 coord, EPortDirection direction, int portHashCode) {
        var coordStart = waveManager.waveSlotGridStartCoordinate;
        var coordEnd = waveManager.waveSlotGridEndCoordinate;

        var startShift = IntVector3.zero;
        var endShift = IntVector3.zero;

        switch (direction) {
            case EPortDirection.up:
                endShift = IntVector3.down;
                break;
            case EPortDirection.down:
                startShift = IntVector3.up;
                break;
            case EPortDirection.right:
                endShift = IntVector3.left;
                break;
            case EPortDirection.left:
                startShift = IntVector3.right;
                break;
            case EPortDirection.front:
                endShift = IntVector3.back;
                break;
            case EPortDirection.back:
                startShift = IntVector3.front;
                break;
        }
        // Debug.Log("Coord: " + coord);
        // Debug.Log("Start: " + (coordStart + startShift));
        // Debug.Log("End: " + (coordEnd + endShift));

        if (coord.Inside(coordStart + startShift, coordEnd + endShift)) {
            var dir = IntVector3.zero - startShift - endShift;

            var nearbyCoord = coord + dir;
            // Debug.Log("Nearby: " + nearbyCoord);
            var waveSlot = waveManager.GetWaveSlotWithCoordinate(nearbyCoord);
            var waveToCollaspe = waveManager.GetWaveHashCodeViaComparedPort(portHashCode);
            waveSlot.MarkToCollaspeWave(waveToCollaspe);
            // waveSlot.CollaspeWave(waveToCollaspe);
        }

        return false;
    }

    public WavePrefabToGenerate[] CollaspeRectRange(IntVector3 start, IntVector3 end) {
        while (true) {

            // Pick a random unobserved position;
            var randomSlot = waveManager.GetRandomUnObservedWaveSlot();
            if (randomSlot == null) return null;
            // Observe a random wave;
            randomSlot.ObserveARandomWave();

            StartCollaspe();
        }

        return wavePrefabToGenerate;
    }
}