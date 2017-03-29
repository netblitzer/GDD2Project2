using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float moveSpeed = 2.0f;

    protected Vector3 velocity;

    //property for velocity
    public Vector3 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    //internal reference to the CharacterController component
    protected CharacterController charControl;

    // Use this for initialization
    void Start()
    {
        velocity = new Vector3(0, 0, 0);
        charControl = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        //Get arrow key input
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            velocity = new Vector3(-moveSpeed, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            velocity = new Vector3(moveSpeed, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            
            velocity = new Vector3(0, 0, moveSpeed);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            
            velocity = new Vector3(0, 0, -moveSpeed);
        }
        else if(!Input.anyKey)
        {
            velocity = new Vector3(0, 0, 0);
        }

        charControl.Move(velocity * Time.deltaTime);

    }

    private void FixedUpdate()
    {


    }//end of fixed update




}//end of player