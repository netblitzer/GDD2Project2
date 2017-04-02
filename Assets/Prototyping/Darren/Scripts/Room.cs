using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle what happens in a room.
/// </summary>
public class Room : MonoBehaviour {

    public List<GameObject> roomContents;

    public int playerStayTime;

    private GameObject fixedObject = null;
    private bool containsEnemy = false;
    private SpawnPoint spawn;


    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        roomContents = new List<GameObject>();
        spawn = gameObject.GetComponentInChildren<SpawnPoint>();

        //Find and add any generators or convetors to the room list.
        foreach (Transform child in transform)
        {
            if (child.tag == "Generator" || child.tag == "CircleConvertor" || child.tag == "SquareConvertor" || child.tag == "TriangleConvertor")

            {
                fixedObject = child.gameObject;
                roomContents.Add(fixedObject);
            }
        }

    }

    /// <summary>
    /// Update is called once per frame
    /// Check what is in a room
    /// </summary>
    void Update()
    {
        //Do stuff based on what is in the room.
        if (playerStayTime > 1) {
            playerStayTime -= 1;
        }
        if (spawn != null) {
            spawn.adjustSpawnChance(-0.25f);
        }

        GameObject Ply = null;
        List<Enemy> enemiesInRoom = new List<Enemy>();

        if (roomContents.Count > 0)
        {
            foreach (GameObject obj in roomContents)
            {
                if (obj.tag == "Player")
                {
                    //Debug.Log(enemiesInRoom.Count);
                    Ply = obj;

                    if (playerStayTime < 300) {
                        playerStayTime += 3;
                    }
                    if (spawn != null) {
                        spawn.adjustSpawnChance(1);
                    }

                    if (fixedObject != null)
                    {
                        fixedObject.GetComponent<Converter>().onRepair(1);
                    }
                }//end if player
                else if(obj.tag == "Enemy" && !containsEnemy)
                {
                    containsEnemy = true;
                    enemiesInRoom.Add(obj.GetComponent<Enemy>());
                }//end if enemy

            }//end for each in roomcontents

        }//end if

        //Debug.Log(enemiesInRoom.Count);

        //If a room has enemies in it, damage the convertor in the room.
        if (containsEnemy && fixedObject != null)
        {
            fixedObject.GetComponent<Converter>().onHit(1);
        }
        
        //Tell spawnpoint that the player is in the room.
        if (spawn != null && Ply != null) {
            spawn.containsPlayer = true;

            //Debug.Log(enemiesInRoom);

            foreach (Enemy e in enemiesInRoom) {
                e.target = Ply;
            }
        } else {
            foreach (Enemy e in enemiesInRoom) {
                e.target = null;
            }
        }

        roomContents.Clear();
        containsEnemy = false;
    }//end Update

    private void OnTriggerStay(Collider other)
    {
        if (fixedObject != null) { roomContents.Add(fixedObject); }

        if (other.tag == "Enemy")
        {
            roomContents.Add(other.gameObject);
        }

        if (other.tag == "Player")
        {
            roomContents.Add(other.gameObject);

            if (spawn != null) {
                spawn.containsPlayer = true;
            }
        }
    }//end On Trigger Stay

}//end of Room
