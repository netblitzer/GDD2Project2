using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    public float spawnWeight = 1f;  // proportional amount for how great the spawn chance is
    public bool containsPlayer;     // flag to tell if the player is currently in this room to avoid spawning in it

    public float maxSpawnWeight = 100f;
    public float minSpawnWeight = 1f;

    private float lateAdjust;
    public List<SpawnPoint> neighbors;

    // Use this for initialization
    void Start () {
        spawnWeight = 1f;
        lateAdjust = 0f;
        neighbors = new List<SpawnPoint>();
	}

    void Update () {
        this.adjustSpawnChance(lateAdjust);
        lateAdjust = 0f;
    }

    public void adjustSpawnChance(float _amount) {
        spawnWeight += _amount;

        if (spawnWeight < minSpawnWeight) {
            spawnWeight = minSpawnWeight;
        } else if (spawnWeight > maxSpawnWeight) {
            spawnWeight = maxSpawnWeight;
        }

        if (_amount > 1) {
            foreach (SpawnPoint s in neighbors) {
                s.adjustSpawnChance(_amount * 0.5f);
            }
        }
    }

    // same as adjust spawn chance, but happens once per frame at the same time, rather than on function call
    public void lateAdjustSpawnChance(float _amount) {
        lateAdjust += _amount;
    }

    public void addNeighbor (SpawnPoint _neighbor) {
        neighbors.Add(_neighbor);
    }
}
