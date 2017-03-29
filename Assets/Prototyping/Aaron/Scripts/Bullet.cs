using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float bulletSpeed;

	private Vector3 velocity;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		transform.position += velocity * bulletSpeed * Time.deltaTime;
	}

	public void Fire(Vector3 direction) {
		velocity = new Vector3(direction.x, 0, direction.z).normalized;	
	}
}
