using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour {

    public Dictionary<Vector2, SpawnPoint> spawns;
    public List<Enemy> enemies;
    public List<Vector2> keys;
    public GameObject enemyPrefab;

    [Tooltip("A multiplier to make things more or less difficult through the game")]
    public float difficulty = 1f;
    
    [Tooltip("An adjustable value for the chance for enemies to split.")]
    public float enemySplitChance;
    
    // variables for the enemy stats
    [Tooltip("The max health of the enemy.")]
    public int maxHealth = 10;

    [Tooltip("The max speed of the enemy.")]
    public int maxSpeed = 4;

    [Tooltip("The max speed of the enemy.")]
    public float maxDamage = 1f;

    [Tooltip("The max force of the enemy controller.")]
    public int maxForce = 10;

    private Vector2 roomSize = new Vector2(14.25f, 10f);

    private int enemyCount;

    // Use this for initialization
    void Start () {
        enemyCount = 0;

        //spawns = new List<SpawnPoint>();
        enemies = new List<Enemy>();
        spawns = new Dictionary<Vector2, SpawnPoint>();

        // find all the spawns in the scene
        SpawnPoint[] unsortedSpawns = FindObjectsOfType<SpawnPoint>();

        // sort the spawns
        sortSpawns(unsortedSpawns);

        // initialize enemies
        Physics.IgnoreLayerCollision(8, 8, true);
        //Physics.IgnoreLayerCollision(8, 9, true);

        keys = spawns.Keys.ToList<Vector2>();

        int startAmount = (int)(Random.Range(10f, 20f) * difficulty);
        //int startAmount = 500;
        for (int i = 0; i < startAmount; i++) {
            GameObject newEnemy = GameObject.Instantiate<GameObject>(enemyPrefab);

            // spawn them in a random room
            int room = (int)Random.Range(0f, spawns.Count);
            
            // initialize the enemy
            Enemy eScript = newEnemy.GetComponent<Enemy>();
            Vector3 spawnLoc = (spawns[keys[room]].gameObject.transform.position - new Vector3(Random.Range(-7f, 7f), 10.5f, Random.Range(-4f, 4f)));
            eScript.init(maxHealth, maxSpeed, maxForce, maxDamage, (EnemyTypes)((int)Random.Range(0, 3)), spawnLoc, null);
            enemyCount++;
            eScript.name = "Enemy Number " + enemyCount.ToString();

            enemies.Add(eScript);
        }
	}

    // function to sort the rays into a proper 
    private void sortSpawns(SpawnPoint[] _unsorted) {
        float minX, minY;
        minX = minY = float.MaxValue;

        // find the minimum positions
        foreach(SpawnPoint room in _unsorted) {
            if (room.transform.position.x < minX) {
                minX = room.transform.position.x;
            }

            if (room.transform.position.y < minY) {
                minY = room.transform.position.y;
            }
        }

        // sort rooms based on position from mins
        foreach(SpawnPoint room in _unsorted) {
            Vector3 pos = room.transform.position;
            Vector2 loc = new Vector2((int)pos.x / roomSize.x, (int)pos.z / roomSize.y);

            //Debug.Log("Room - Pos: " + pos + "  key: " + loc);

            spawns.Add(loc, room);
        }
    }
	
	// Update is called once per frame
	void Update () {

        int[] types = new int[3];

        List<Enemy> enemiesToRemove = new List<Enemy>();
        int count = enemies.Count;

        for (int i = 0; i < count; i++) {
            Enemy e = enemies[i];

            e.calcMovement();

            //e.damageEnemy(Random.Range(0f, 0.1f));

            // check if the enemy is still alive
            if (e.isDead()) {
                enemiesToRemove.Add(e);
            }
            else {    // otherwise, add it to the 'breeding stock'
                types[(int)e.type]++;
                /*
                // see if the enemy splits
                if (Random.Range(0, 1f) < enemySplitChance) {
                    GameObject newEnemy = GameObject.Instantiate(enemyPrefab);
                    Enemy eScript = newEnemy.GetComponent<Enemy>();
                    if (Random.Range(0, 1) > 0.5f) {
                        eScript.init(10, 2, 10, 2, (int)e.type, new Vector3(Random.Range(-10f, 10f), 0.5f, Random.Range(-10f, 10f)), null);
                        enemies.Add(eScript);
                    }
                    else {    // some won't have a target and will just wander
                        eScript.init(10, 2, 10, 2, (int)e.type, new Vector3(Random.Range(-10f, 10f), 0.5f, Random.Range(-10f, 10f)), null);
                        enemies.Add(eScript);
                    }
                }
                */
            }
        }

        foreach (Enemy e in enemiesToRemove) {
            enemies.Remove(e);
            GameObject.Destroy(e.gameObject);
        }
    }
}
