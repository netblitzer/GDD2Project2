using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinusButton : MonoBehaviour {

	public int Type;
	public Button buttonSelf;

	private Player player;

	// Use this for initialization
	void Start () 
	{
		player = GameObject.Find ("Player").GetComponent<Player> ();
		buttonSelf.onClick.AddListener (Minus);
	}
	
	void Minus()
	{
		player.SubStrength (Type);
	}
}
