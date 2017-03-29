using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    //is the door a top/bottom door or a left/right
    //only one of these should be true
    public bool LR = false;
    public bool TB = false;

    //doors default starting state
    public bool open = true;

    private Vector3 pos;

	// Use this for initialization
	void Start ()
    {
        Vector3 startPos = transform.position;

        if (LR)
        {
            if (open) { startPos.z = 1.52f; }
            else { startPos.z = 0; }
        }
        else if (TB)
        {
            if (open) { startPos.x = 1.52f; }
            else { startPos.x = 0; }
        }

        transform.position = startPos;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OpenDoor()
    {
        pos = transform.position;

        if (LR) { pos.z = 1.52f; }
        else if (TB) { pos.x = 1.52f; }

        transform.position = pos;
    }

    public void closeDoor()
    {
        pos = transform.position;

        if (LR) { pos.z = 0; }
        else if (TB) { pos.x = 0f; }

        transform.position = pos;
    }

    //TB doors, z is 0 if closed, 1.52 if open
    //LR doors, x is 0 if closed, 1.52 if open
}
