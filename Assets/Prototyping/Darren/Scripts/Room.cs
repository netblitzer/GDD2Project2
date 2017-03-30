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


    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        roomContents = new List<GameObject>();

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
        if (playerStayTime > 0) { playerStayTime -= 1; }

        if (roomContents.Count > 0)
        {
            foreach (GameObject obj in roomContents)
            {
                if (obj.tag == "Player")
                {
                    if (playerStayTime < 300) { playerStayTime += 3; }

                    if (fixedObject != null)
                    {
                        fixedObject.GetComponent<Converter>().onRepair(1);
                    }
                }//end if player
                else if(obj.tag == "Enemy" && !containsEnemy)
                {
                    containsEnemy = true;
                }//end if enemy

            }//end for each in roomcontents

        }//end if

        if(containsEnemy && fixedObject != null)
        {
            fixedObject.GetComponent<Converter>().onHit(1);
        }

        roomContents.Clear();
        containsEnemy = false;
    }//end Update

    private void OnTriggerStay(Collider other)
    {
        roomContents.Clear();

        if (fixedObject != null) { roomContents.Add(fixedObject); }

        if (other.tag == "Enemy")
        {
            roomContents.Add(other.gameObject);
        }
        else if (other.tag == "Player")
        {
            roomContents.Add(other.gameObject);
        }
    }
}//end of Room
