﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float maxSpeed;
	public GameObject bulletPrefab;

	private Vector3 velocity;
	private float speed;
	private Vector3 prevMousePosition;
	private float shootFreezeTime;

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
		speed = 0f;
		shootFreezeTime = 0f;
		velocity = Vector3.zero;
		prevMousePosition = Input.mousePosition;
		charControl = gameObject.GetComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update()
	{
		// rotate the player using mouse
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit mouseToGround;
		if (Physics.Raycast(ray, out mouseToGround)) { 
			Vector3 mousePoint2D = new Vector3 (mouseToGround.point.x, 0, mouseToGround.point.z);
			Vector3 player2D = new Vector3 (transform.position.x, 0, transform.position.z);
			Vector3 playerToPoint = mousePoint2D - player2D;

			if (prevMousePosition != Input.mousePosition){ // prevent jittery movement
				transform.forward = Vector3.Lerp (transform.forward, playerToPoint, .05f);
			}
		}
		prevMousePosition = Input.mousePosition;

		// move
		//Get arrow key input
		if (Input.anyKey) {
			speed += .1f;
		}


		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
		{
			velocity += new Vector3(-1, 0, 0);
		}
		else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			velocity += new Vector3(1, 0, 0);
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{

			velocity += new Vector3(0, 0, 1);
		}
		else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{

			velocity += new Vector3(0, 0, -1);
		}

		// friction
		else if (!Input.anyKey)
		{
			if (speed > 0f) 
			{
				speed -= 0.1f;
				// make sure speed gets to 0
				if (speed < 0.1f) 
				{
					speed = 0f;
				}
			} 
			else if (speed < 0f) 
			{
				speed += 0.1f;
				// make sure speed gets to 0
				if (speed > -0.1f) 
				{
					speed = 0f;
				}
			}
		} 
		Debug.Log (velocity);
		// clamp speed
		speed = Mathf.Clamp(speed, 0, maxSpeed);
		charControl.Move(velocity.normalized * speed * Time.deltaTime);
		velocity = Vector3.zero;

		// shoot logic
		if (shootFreezeTime > 0f) 
		{
			shootFreezeTime -= Time.deltaTime;
			if (shootFreezeTime < 0.1f) {
				shootFreezeTime = 0f;
			}
		}
		if (Input.GetMouseButtonDown(0) && shootFreezeTime == 0f)
		{
			Shoot ();
			shootFreezeTime = 0.5f;
		}
	}

	// create a bullet yo!
	private void Shoot() 
	{
		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		bullet.transform.position = transform.position + transform.forward;
		Bullet bulletBehavior = bullet.GetComponent<Bullet> ();
		bulletBehavior.Fire (transform.forward);
	}
}//end of player