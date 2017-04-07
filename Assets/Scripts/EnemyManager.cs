using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyManager : MonoBehaviour {

    public Dictionary<Vector2, SpawnPoint> spawns;
    public List<Enemy> enemies;
    public List<Vector2> keys;
    public GameObject enemySquarePrefab;
    public GameObject enemyCirclePrefab;
    public GameObject enemyTrianglePrefab;

    [Tooltip("A multiplier to make things more or less difficult through the game")]
    public float difficulty = 1f;
    
    [Tooltip("An adjustable value for the chance (out of 10000 frames) for enemies to split where they are.")]
    public float enemySplitChance = 1f;
    [Tooltip("An adjustable value for the chance (out of 10000 frames) for enemies to spawn into the map.")]
    public float enemySpawnChance = 10f;

    // variables for the enemy stats
    [Tooltip("The max health of the enemy.")]
    public int maxHealth = 10;

    [Tooltip("The max speed of the enemy.")]
    public int maxSpeed = 4;

    [Tooltip("The max speed of the enemy.")]
    public float maxDamage = 0.2f;

    [Tooltip("The max force of the enemy controller.")]
    public int maxForce = 10;

    private Vector2 roomSize = new Vector2(14.25f, 10f);

    private int enemyCount;

    private Vector2 min, max;

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

        int startAmount = (int)(Random.Range(15, 20) * difficulty);
        //int startAmount = 500;
        for (int i = 0; i < startAmount; i++) {
            //GameObject newEnemy = GameObject.Instantiate<GameObject>(enemyPrefab);

            //Create an Enemy based on type;
            GameObject newEnemy;
            EnemyTypes type = (EnemyTypes)(Random.Range(0, 3));
            switch (type)
            {
                default:
                case EnemyTypes.square:
                    newEnemy = GameObject.Instantiate<GameObject>(enemySquarePrefab);
                    break;
                case EnemyTypes.circle:
                    newEnemy = GameObject.Instantiate<GameObject>(enemyCirclePrefab);
                    break;
                case EnemyTypes.triangle:
                    newEnemy = GameObject.Instantiate<GameObject>(enemyTrianglePrefab);
                    break;
            }


            // spawn them in a random room
            int room;
            do {
                room = Random.Range(0, spawns.Count);
            } while (keys[room] == Vector2.zero);
            
            // initialize the enemy
            Enemy eScript = newEnemy.GetComponent<Enemy>();
            Vector3 spawnLoc = (spawns[keys[room]].gameObject.transform.position - new Vector3(Random.Range(-7f, 7f), 10.5f, Random.Range(-4f, 4f)));
            eScript.init(maxHealth, maxSpeed, maxForce, maxDamage, type, spawnLoc, null);
            enemyCount++;
            eScript.name = "Enemy Number " + enemyCount.ToString();

            enemies.Add(eScript);
        }
	}

    // function to sort the rays into a proper 
    private void sortSpawns(SpawnPoint[] _unsorted) {
        min = new Vector2(float.MaxValue, float.MaxValue);
        max = new Vector2(float.MinValue, float.MinValue);

        // find the minimum positions
        foreach(SpawnPoint room in _unsorted) {
            if (room.transform.position.x < min.x) {
                min.x = room.transform.position.x;
            }

            if (room.transform.position.z < min.y) {
                min.y = room.transform.position.z;
            }

            if (room.transform.position.x > max.x) {
                max.x = room.transform.position.x;
            }

            if (room.transform.position.z > max.y) {
                max.y = room.transform.position.z;
            }
        }


        min = new Vector2(min.x / roomSize.x, min.y / roomSize.y);
        max = new Vector2(max.x / roomSize.x, max.y / roomSize.y);

        // sort rooms based on position from mins
        foreach (SpawnPoint room in _unsorted) {
            Vector3 pos = room.transform.position;
            Vector2 loc = new Vector2((int)(pos.x / roomSize.x), (int)(pos.z / roomSize.y));

            //Debug.Log("Room - Pos: " + pos + "  key: " + loc);

            spawns.Add(loc, room);
        }

        keys = spawns.Keys.ToList<Vector2>();

        for (int i = 0; i < keys.Count; i++) {
            Vector2 key = keys[i];

            if (key.x > min.x) {
                spawns[key].addNeighbor(spawns[new Vector2(key.x - 1, key.y)]);
            }
            if (key.x < max.x) {
                spawns[key].addNeighbor(spawns[new Vector2(key.x + 1, key.y)]);
            }
            if (key.y > min.y) {
                spawns[key].addNeighbor(spawns[new Vector2(key.x, key.y - 1)]);
            }
            if (key.y < max.y) {
                spawns[key].addNeighbor(spawns[new Vector2(key.x, key.y + 1)]);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        int[] types = { 10, 10, 10 };
        int chance;

        List<Enemy> enemiesToRemove = new List<Enemy>();

        // find the total weight of all rooms right now
        float totalWeight = 0;
        for (int i = 0; i < keys.Count; i++) {
            totalWeight += spawns[keys[i]].spawnWeight;
        }

        int count = enemies.Count; // before adding any new enemies or splitting

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

                chance = Random.Range(0, 10000);
                
                if (enemySplitChance * difficulty >= chance) {
                    // split the enemy
                    GameObject newEnemy;

                    switch (e.type) {
                        default:
                        case EnemyTypes.square:
                            newEnemy = GameObject.Instantiate<GameObject>(enemySquarePrefab);
                            break;
                        case EnemyTypes.circle:
                            newEnemy = GameObject.Instantiate<GameObject>(enemyCirclePrefab);
                            break;
                        case EnemyTypes.triangle:
                            newEnemy = GameObject.Instantiate<GameObject>(enemyTrianglePrefab);
                            break;
                    }

                    // initialize the enemy
                    Enemy eScript = newEnemy.GetComponent<Enemy>();
                    Vector3 spawnLoc = (e.gameObject.transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)));
                    eScript.init(maxHealth, maxSpeed, maxForce, maxDamage, e.type, spawnLoc, null);
                    enemyCount++;
                    eScript.name = "Enemy Number " + enemyCount.ToString();

                    // add it to the enemy list
                    enemies.Add(eScript);
                    types[(int)e.type]++;
                }
            }
        }   // end for loop on each enemy



        // new enemy spawning (not spliting)
        
        chance = Random.Range(0, 10000);

        if (enemySpawnChance * difficulty > chance) {
            // spawn a new enemy somewhere based on total weights

            // random weight to spawn into
            float spawnWeight = Random.Range(0, totalWeight);
            float checkedWeight = 0f;
            // find which room the spawn is in
            for (int j = 0; j < keys.Count; j++) {
                // check if it's this room
                if (spawnWeight > checkedWeight && spawnWeight <= (checkedWeight + spawns[keys[j]].spawnWeight)) {
                    
                    if (spawns[keys[j]].containsPlayer) {
                        j = 0;
                        checkedWeight = 0f;
                        chance = Random.Range(0, 10000);
                    }

                    // spawn an enemy
                    GameObject newEnemy;
                    EnemyTypes type = EnemyTypes.square;

                    // figure out what type it is
                    int typeRand = Random.Range(0, enemies.Count);
                    int checkedTypesWeight = 0;
                    for (int k = 0; k < 3; k++) {
                        if (typeRand >= checkedTypesWeight && typeRand < checkedTypesWeight + types[k]) {
                            // enemy is this type
                            type = (EnemyTypes)k;
                            break;
                        } else {
                            checkedTypesWeight += types[k];
                        }
                    }


                    switch (type) {
                        default:
                        case EnemyTypes.square:
                            newEnemy = GameObject.Instantiate<GameObject>(enemySquarePrefab);
                            break;
                        case EnemyTypes.circle:
                            newEnemy = GameObject.Instantiate<GameObject>(enemyCirclePrefab);
                            break;
                        case EnemyTypes.triangle:
                            newEnemy = GameObject.Instantiate<GameObject>(enemyTrianglePrefab);
                            break;
                    }

                    // initialize the enemy
                    Enemy eScript = newEnemy.GetComponent<Enemy>();
                    Vector3 spawnLoc = (spawns[keys[j]].gameObject.transform.position - new Vector3(Random.Range(-7f, 7f), 10.5f, Random.Range(-4f, 4f)));
                    eScript.init(maxHealth, maxSpeed, maxForce, maxDamage, type, spawnLoc, null);
                    enemyCount++;
                    eScript.name = "Enemy Number " + enemyCount.ToString();

                    enemies.Add(eScript);
                    break;
                }
                else {
                    checkedWeight += spawns[keys[j]].spawnWeight;
                }
            }
        }

        chance = Random.Range(0, 10000);

        if (enemySpawnChance * difficulty > chance)
        {
            // spawn a new enemy somewhere based on total weights

            // random weight to spawn into
            float spawnWeight = Random.Range(0, totalWeight);
            float checkedWeight = 0f;
            // find which room the spawn is in
            for (int j = 0; j < keys.Count; j++)
            {
                // check if it's this room
                if (spawnWeight > checkedWeight && spawnWeight <= (checkedWeight + spawns[keys[j]].spawnWeight))
                {

                    if (spawns[keys[j]].containsPlayer)
                    {
                        j = 0;
                        checkedWeight = 0f;
                        chance = Random.Range(0, 10000);
                    }

                    // spawn an enemy
                    GameObject newEnemy;
                    EnemyTypes type = (EnemyTypes)Random.Range(0, 3);

                    switch (type)
                    {
                        default:
                        case EnemyTypes.square:
                            newEnemy = GameObject.Instantiate<GameObject>(enemySquarePrefab);
                            break;
                        case EnemyTypes.circle:
                            newEnemy = GameObject.Instantiate<GameObject>(enemyCirclePrefab);
                            break;
                        case EnemyTypes.triangle:
                            newEnemy = GameObject.Instantiate<GameObject>(enemyTrianglePrefab);
                            break;
                    }

                    // initialize the enemy
                    Enemy eScript = newEnemy.GetComponent<Enemy>();
                    Vector3 spawnLoc = (spawns[keys[j]].gameObject.transform.position - new Vector3(Random.Range(-7f, 7f), 10.5f, Random.Range(-4f, 4f)));
                    eScript.init(maxHealth, maxSpeed, maxForce, maxDamage, type, spawnLoc, null);
                    enemyCount++;
                    eScript.name = "Enemy Number " + enemyCount.ToString();

                    enemies.Add(eScript);
                    break;
                }
                else
                {
                    checkedWeight += spawns[keys[j]].spawnWeight;
                }
            }
        }


        foreach (Enemy e in enemiesToRemove) {
            enemies.Remove(e);
            GameObject.Destroy(e.gameObject);
        }
    }
}
