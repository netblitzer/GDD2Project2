using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle what happens in a room.
/// </summary>
public class Room : MonoBehaviour {

    public List<GameObject> enemies;
    public List<GameObject> roomContents;

    private GameObject fixedObject = null;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        enemies = new List<GameObject>();
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
        if (roomContents.Count > 0)
        {
            foreach (GameObject obj in roomContents)
            {
                if(obj.tag == "Player")
                {
                    if (fixedObject != null)
                    {
                        fixedObject.GetComponent<Converter>().onRepair(1);
                    }
                }
            }
        }

    }//end Update

    private void OnTriggerStay(Collider other)
    {
        enemies.Clear();
        roomContents.Clear();

        if (fixedObject != null) { roomContents.Add(fixedObject); }

        if (other.tag == "Enemy")
        {
            enemies.Add(other.gameObject);
        }
        else if(other.tag == "Player")
        {
            roomContents.Add(other.gameObject);
        }
    }
}//end of Room
