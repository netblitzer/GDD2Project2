using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Door Trigger
/// Checks collisions with colliders and moves the camera if the player
/// is changing rooms.
/// </summary>
public class DoorTrigger : MonoBehaviour {

    public GameObject gameManager;

    public GameObject cameraTarget = null;

	/// <summary>
    /// Start up, ran once.
    /// Finds and sets the game manager.
    /// </summary>
	void Start () {
        gameManager = GameObject.FindGameObjectWithTag("GMan");
	}

    /// <summary>
    /// Fires when a collider enters the trigger space.
    /// </summary>
    /// <param name="other">Collider of gameObject in trigger</param>
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

    /// <summary>
    /// Keep shifting set to true until the player has left the door trigger
    /// </summary>
    /// <param name="other">Collider of gameObject that left trigger</param>
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            gameManager.GetComponent<GameManager>().SetShifting(true);
        }
    }

}//end of Door Trigger