using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle what happens in a room.
/// </summary>
public class Room : MonoBehaviour
{
	private List<GameObject> roomContents;
	private List<GameObject> toRemove;

    private GameObject convertor = null;
    private GameObject generator = null;
    private GameObject player = null;

    private bool containsEnemy = false;
	private bool containsPlayer = false;

    [SerializeField]
    private int repair;
    [SerializeField]
    private int damage;

    private SpawnPoint spawn;
	private GameManager gameManager;
	private Converter converterBehavior;
    private Generator generatorBehavior;

    //Properties and publics
    public int roomCount;

    public GameObject[] InRoom;

    public GameObject Convertor
    { get { return convertor; } }

    public GameObject Generator
    { get { return generator; } }

    public int Repair
    {
        get { return repair; }
        set { repair = value; }
    }

    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        roomContents = new List<GameObject>();
		toRemove = new List<GameObject> ();
        spawn = gameObject.GetComponentInChildren<SpawnPoint>();
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();

        damage = 1;
        repair = 1;

        //Find and add any generators or convetors to the room list.
        foreach (Transform child in transform)
        {
            if (child.tag == "CircleConvertor" || child.tag == "SquareConvertor" || child.tag == "TriangleConvertor")
            {
                convertor = child.gameObject;
                converterBehavior = convertor.GetComponent<Converter>();
                roomContents.Add(convertor);
            }
            else if (child.tag == "Generator")
            {
                generator = child.gameObject;
                generatorBehavior = generator.GetComponent<Generator>();
                roomContents.Add(generator);
            }
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// Check what is in a room
    /// </summary>
    void Update()
    {
        //Get updated values from game manager for testing
        //---------------------------------------------------------------------
        damage = gameManager.enemyDamageRate;
        repair = gameManager.playerRepairRate;    
        //---------------------------------------------------------------------

        roomCount = roomContents.Count;
        InRoom = roomContents.ToArray();

        //Do stuff based on what is in the room.
        if (spawn != null)
        {
            spawn.adjustSpawnChance(-0.25f);
        }

        containsEnemy = false;

        if (roomContents.Count > 0)
        {
            foreach (GameObject obj in roomContents)
            {
                if (obj.tag == "Player")
                {

                    if (spawn != null)
                    {
                        spawn.adjustSpawnChance(1.5f);
                    }

                    if (generator != null)
                    {
                        generator.GetComponent<Generator>().onRepair(repair);
                    }
                    else if (convertor != null)
                    {
                        convertor.GetComponent<Converter>().onRepair(repair);
                    }
                }//end if player

                else if (obj.tag == "Enemy")
                {
					if (obj.GetComponent<Enemy> ().isDead()) 
					{
						toRemove.Add (obj.gameObject);
					} 

					containsEnemy = true;
					obj.GetComponent<Enemy> ().target = player;
				
                }//end if enemy

            }//end for each in roomcontents
			foreach (GameObject g in toRemove) 
			{
				roomContents.Remove (g);
			}

        }//end if

        //If a room has enemies in it, damage the convertor in the room.
        if (containsEnemy && convertor != null)
        {
            converterBehavior.onHit(damage);
        }
        else if (containsEnemy && generator != null)
        {
            generatorBehavior.onHit(damage);
        }

        containsEnemy = false;

        foreach (GameObject g in roomContents)
        {
            if (g.tag == "Enemy")
            {
                containsEnemy = true;
                break;
            }
        }
    }//end Update

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            roomContents.Add(other.gameObject);
        }

        if (other.tag == "Player")
        {
            roomContents.Add(other.gameObject);
            player = other.gameObject;
            containsPlayer = true;

            // activate plus button on UI if there is a converter in this room with the player
            if (convertor != null)
            {
                converterBehavior.ActivateConversion();
            }

            if (spawn != null)
            {
                spawn.containsPlayer = true;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            roomContents.Remove(other.gameObject);
        }

        if (other.tag == "Player")
        {
            roomContents.Remove(other.gameObject);
            player = null;
            containsPlayer = false;

            if (spawn != null)
            {
                spawn.containsPlayer = false;
            }

			if (convertor != null) 
			{
				converterBehavior.DeactivateConversion ();
			}
        }
    }

}//end of Room
