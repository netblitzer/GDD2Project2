using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCamReset : MonoBehaviour
{

    public GameObject gameManager;
    public GameObject cameraTarget = null;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GMan");
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player")
        {
            gameManager.GetComponent<GameManager>().RoomChanged();

        }
    }

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
}
