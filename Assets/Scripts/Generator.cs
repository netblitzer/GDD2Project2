﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    private GameManager gm;

    [SerializeField]
    private int health;

    private int maxHealth;

    [SerializeField]
    private bool powering;

    public Material[] materials;

    //Properties
    public int Health
    {
        get { return health; }
        set { health = value; }
    }
    
    public bool Powering
    {
        get { return powering; }
        set { powering = value; }
    }


    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        health = 1000;
        powering = true;
        gm = FindObjectOfType<GameManager>();
        maxHealth = gm.generatorHealth;
        health = maxHealth;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {}

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
            gameObject.GetComponent<Renderer>().sharedMaterial = materials[1];
        }
        else
        {
            health -= damage;
        }
    }//end of OnHit

    /// <summary>
    /// Handles repair processing for generators
    /// Could also find ways to combine with onHit, if that would be simpler.
    /// </summary>
    /// <param name="regen">Integer value > 0 of how much to repair by</param>
    public void onRepair(int regen)
    {
        if (health < 1 && regen > 0)
        {
            powering = true;
            gameObject.GetComponent<Renderer>().sharedMaterial = materials[0];
        }

        health += regen;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }//end of Repair

}//end of Generator
