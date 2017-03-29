﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Converter : MonoBehaviour {

    private int health;
    private bool active;
    private string type;

    public int Health
    {
        get { return health; }
    }

    public bool Active
    {
        get { return active; }
    }

    public string Type
    {
        get { return type; }
    }

    // Use this for initialization
    void Start()
    {
        health = 100;
        active = true;
        type = this.gameObject.tag;     // Question voodoo. . .?
    }

    // Update is called once per frame
    void Update()
    {

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
        if (health == 0 && regen > 0)
        {
            active = true;
        }

        health += regen;
        if (health > 100)
        {
            health = 100;
        }
    }
    
}