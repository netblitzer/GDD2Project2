using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manager : MonoBehaviour {

    public List<enemy> Enemies;
    public GameObject enemyPrefab;
    public GameObject target;

    public float enemySplitChance = 0f;

	// Use this for initialization
	void Start () {
        Enemies = new List<enemy>();

        int randCount = Random.Range(5, 15);

        for (int i = 0; i < randCount; i++) {
            GameObject e = GameObject.Instantiate(enemyPrefab);
            enemy eScript = e.GetComponent<enemy>();
            
            if (Random.Range(0, 1) > 0.5f) {
                eScript.init(10, 2, 10, 2, (int)Random.Range(0, 3), new Vector3(Random.Range(-10f, 10f), 0.5f, Random.Range(-10f, 10f)), null);
                Enemies.Add(eScript);
            } else {    // some won't have a target and will just wander
                eScript.init(10, 2, 10, 2, (int)Random.Range(0, 3), new Vector3(Random.Range(-10f, 10f), 0.5f, Random.Range(-10f, 10f)), null);
                Enemies.Add(eScript);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {

        // keys to kill off enemies on your own
        if (Input.GetKeyDown(KeyCode.A)) {
            for (int i = 0; i < Enemies.Count; i++) {
                if (Enemies[i].type == EnemyType.triangle) {
                    Enemies.RemoveAt(i);
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            for (int i = 0; i < Enemies.Count; i++) {
                if (Enemies[i].type == EnemyType.square) {
                    Enemies.RemoveAt(i);
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            for (int i = 0; i < Enemies.Count; i++) {
                if (Enemies[i].type == EnemyType.circle) {
                    Enemies.RemoveAt(i);
                    break;
                }
            }
        }


        int[] types = new int[3];

        List<enemy> enemiesToRemove = new List<enemy>();
        int count = Enemies.Count;

		for (int i = 0; i < count; i++) {
            enemy e = Enemies[i];

            e.calcMovement();

            //e.damageEnemy(Random.Range(0f, 0.1f));

            // check if the enemy is still alive
            if (e.isDead()) {
                enemiesToRemove.Add(e);
            } else {    // otherwise, add it to the 'breeding stock'
                types[(int)e.type]++;

                // see if the enemy splits
                if (Random.Range(0, 1f) < enemySplitChance) {
                    GameObject newEnemy = GameObject.Instantiate(enemyPrefab);
                    enemy eScript = newEnemy.GetComponent<enemy>();
                    if (Random.Range(0, 1) > 0.5f) {
                        eScript.init(10, 2, 10, 2, (int)e.type, new Vector3(Random.Range(-10f, 10f), 0.5f, Random.Range(-10f, 10f)), null);
                        Enemies.Add(eScript);
                    }
                    else {    // some won't have a target and will just wander
                        eScript.init(10, 2, 10, 2, (int)e.type, new Vector3(Random.Range(-10f, 10f), 0.5f, Random.Range(-10f, 10f)), null);
                        Enemies.Add(eScript);
                    }
                }
            }
        }

        foreach(enemy e in enemiesToRemove) {
            Enemies.Remove(e);
            GameObject.Destroy(e.gameObject);
        }
	}
}
