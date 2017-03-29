using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject myCamera;

    public bool shifting;

	// Use this for initialization
	void Start () {
        shifting = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void ChangeRoom(Vector3 target)
    {
        if(!shifting)
        {
            shifting = true;
            myCamera.GetComponent<MainCam>().MoveCamera(target);
        }

    }

    public void SetShifting(bool _shifting)
    {
        shifting = _shifting;
    }

    public void RoomChanged()
    {
        shifting = false;
    }
    
}
