using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle the Camera movement from room to room.
/// </summary>
public class RoomCam : MonoBehaviour {

    public GameObject gameManager;
    public GameObject cameraTarget = null;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GMan");
    }

    /// <summary>
    /// Tells the Game Manager that the player has entered a room.
    /// This may not be needed anymore and will be tested.
    /// </summary>
    /// <param name="col">Collider in the trigger</param>
    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player")
        {
            gameManager.GetComponent<GameManager>().RoomChanged();

        }
    }

    /// <summary>
    /// Tells Game Manager where the camera should move to.
    /// </summary>
    /// <param name="other">Collider that entered the trigger</param>
    void OnTriggerEnter(Collider other)
    {
        if (cameraTarget != null)
        {
            if (other.tag == "Player")
            {
                gameManager.GetComponent<GameManager>().ChangeRoom(cameraTarget.transform.position);
            }
        }
    }

}//end of Room Cam
