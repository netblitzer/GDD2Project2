using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Game Manager
/// </summary>
public class JessGameManager : MonoBehaviour {

    public GameObject myCamera;

    public bool shifting;

    public List<GameObject> generators;
    public JessPlayer player;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () {
        shifting = false;

        generators = new List<GameObject>(GameObject.FindGameObjectsWithTag("Generator"));
	}

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () {
        int currEnergy = 0;
        foreach (GameObject g in generators)
        {
            Generator gen = g.GetComponent<Generator>();    // Get generator script
            if (gen.Powering)
            {
                currEnergy++;
            }
        }

        player.SetPlayerEnergy(currEnergy);
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
