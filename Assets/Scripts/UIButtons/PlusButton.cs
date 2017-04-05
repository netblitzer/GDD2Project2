using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlusButton : MonoBehaviour {

	public int Type;
	public Button buttonSelf;

	private Player player;

	void Start() {
		player = GameObject.Find ("Player").GetComponent<Player> ();
		buttonSelf.onClick.AddListener (Plus);
	}

	// make the button interactable
	public void Activate() {
		buttonSelf.interactable = true;
	}

	public void DeActivate() {
		buttonSelf.interactable = false;
	}

	void Plus()
	{
		player.AddStrength (Type);
	}

}
