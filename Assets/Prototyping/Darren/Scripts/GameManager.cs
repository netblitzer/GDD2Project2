using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Game Manager
/// </summary>
public class GameManager : MonoBehaviour {

    public GameObject myCamera;

    public bool shifting;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () {
        shifting = false;
	}

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () {

	}

    /// <summary>
    /// Move the Camera to the room the player is in.
    /// </summary>
    /// <param name="target">Location camera will move to</param>
    public void ChangeRoom(Vector3 target)
    {
        if(!shifting)
        {
            shifting = true;
            myCamera.GetComponent<MainCam>().MoveCamera(target);
        }

    }

    /// <summary>
    /// Set the bool based on if the camera is moving
    /// </summary>
    /// <param name="_shifting"></param>
    public void SetShifting(bool _shifting)
    {
        shifting = _shifting;
    }

    /// <summary>
    /// Player has finished changing rooms
    /// </summary>
    public void RoomChanged()
    {
        shifting = false;
    }
    
}//end of Game Manager
