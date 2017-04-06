using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Converter : MonoBehaviour
{
    public Button myPlusButton; // reference to the pluss button to activate when we enter the room this converter is in

    public Material[] materials;

    [SerializeField]
    private int health = 1000;

    [SerializeField]
    private bool active;

    [SerializeField]
    private string type;

	private PlusButton plusButton; // script for my plus button


    //Properties
    //---------------------------------------------------------
    public int Health
    {
        get { return health; }
    }

    public bool Actives
    {
        get { return active; }
    }

    public string Type
    {
        get { return type; }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        health = 1000;
        active = true;
        type = gameObject.tag;     // Question voodoo. . .?
                                   // Answer More voodoo. this not needed. gameObject is this. O.o
                                   // ^ Cool rap
                                   // YO
		plusButton = myPlusButton.GetComponent<PlusButton>();
    }

    /// <summary>
    /// Handles damage processing for converters
    /// </summary>
    /// <param name="damage">Integer value > 0 of how much to damage by</param>
    public void onHit(int damage)
    {
        if (damage >= health)
        {
            health = 0;
            active = false;
            gameObject.GetComponent<Renderer>().sharedMaterial = materials[1];
        }
        else
        {
            health -= damage;
        }
    }

    /// <summary>
    /// Handles repair processing for converters
    /// Could also find ways to combine with onHit, if that would be simpler.
    /// </summary>
    /// <param name="regen">Integer value > 0 of how much to repair by</param>
    public void onRepair(int regen)
    {
        if (health < 1 && regen > 0)
        {
            active = true;
            gameObject.GetComponent<Renderer>().sharedMaterial = materials[0];
        }

        health += regen;
        if (health > 1000)
        {
            health = 1000;
        }
    }

    /// <summary>
    /// Enable button for conversion
    /// </summary>
	public void ActivateConversion()
	{
		plusButton.Activate ();
	}

    /// <summary>
    /// Disable button for conversion
    /// </summary>
	public void DeactivateConversion()
	{
		plusButton.DeActivate ();
	}


}//end of Convertor
