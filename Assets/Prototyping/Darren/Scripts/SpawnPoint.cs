using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    public float spawnWeight = 1f;  // proportional amount for how great the spawn chance is
    public bool containsPlayer;     // flag to tell if the player is currently in this room to avoid spawning in it

    public float maxSpawnWeight = 100f;
    public float minSpawnWeight = 1f;

    // Use this for initialization
    void Start () {
        spawnWeight = 1f;
	}

    public void adjustSpawnChance(float _amount) {
        spawnWeight += _amount;

        if (spawnWeight < minSpawnWeight) {
            spawnWeight = minSpawnWeight;
        } else if (spawnWeight > maxSpawnWeight) {
            spawnWeight = maxSpawnWeight;
        }
    }
}
