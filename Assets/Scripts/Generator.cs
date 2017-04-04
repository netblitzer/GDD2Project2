using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{

    private int health;
    private bool powering;

    public int Health
    {
        get { return health; }
    }

    public bool Powering
    {
        get { return powering; }
    }

    // Use this for initialization
    void Start()
    {
        health = 100;
        powering = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Handles damage processing for generators
    /// </summary>
    /// <param name="damage">Integer value > 0 of how much to damage by</param>
    public void onHit(int damage)
    {
        if (damage >= health)
        {
            health = 0;
            powering = false;
        }
        else
        {
            health -= damage;
        }
    }

    /// <summary>
    /// Handles repair processing for generators
    /// Could also find ways to combine with onHit, if that would be simpler.
    /// </summary>
    /// <param name="regen">Integer value > 0 of how much to repair by</param>
    public void onRepair(int regen)
    {
        if (health == 0 && regen > 0)
        {
            powering = true;
        }

        health += regen;
        if (health > 100)
        {
            health = 100;
        }
    }
}
