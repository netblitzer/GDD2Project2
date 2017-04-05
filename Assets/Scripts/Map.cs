using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public bool mapOpen = false;

    /*
    //Stuff for references
    public GameObject gameManager;
    GameManager = GameObject.findgameobjectwithtag(Gman);
    gameManager.GetComponent<GameManager>().FUNCTION_OR_VARIABLE
    */

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //map toggle control
        if (Input.GetKeyUp(KeyCode.M))
        {
            if (mapOpen)
                CloseMap();
            else
                OpenMap();
        }	
	}



    private void OpenMap()
    {
        //Check if camera can move??

        //Get and store previous camera position (reference to camera?)

        //move camera to map position

        //remove player control

    }

    private void CloseMap()
    {
        //Return camera to stored position

        //Return player control


    }
}
