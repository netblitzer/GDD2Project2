using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCamReset : MonoBehaviour
{

    public GameObject gameManager;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GMan");
    }

    void OnTriggerEnter(Collider col)
    {
        gameManager.GetComponent<GameManager>().RoomChanged();
    }
}
