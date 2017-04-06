using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The Game Manager
/// </summary>
public class GameManager : MonoBehaviour
{

    private float currentHealth;
    private float currentEnergy1 = 9;
    private float currentEnergy2 = 6;
    private float currentEnergy3 = 3;
    private float unusedEnergy = 1;

    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Image energy1;
    [SerializeField]
    private Image energy2;
    [SerializeField]
    private Image energy3;
    [SerializeField]
    private Image energyEmpty;

    private float minHealth = 0;
    private float maxHealth = 100;

    private Vector3 lastCamPos;
    //---------------------------------------------------------------------------

    public GameObject myCamera;
    public GameObject mapSpot;

    public bool shifting;

    public List<GameObject> generators;
    public Player player;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        shifting = false;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        generators = new List<GameObject>(GameObject.FindGameObjectsWithTag("Generator"));
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        UpdateNumbers();

        HandleBars();

        int currEnergy = 0;
        foreach (GameObject g in generators)
        {
            Generator gen = g.GetComponent<Generator>();    // Get generator script
            if (gen.Powering)
            {
                currEnergy++;
            }
        }

        player.SetPlayerEnergy(currEnergy);

        if(Input.GetKeyDown(KeyCode.M))
        {
            myCamera.GetComponent<Camera>().cullingMask = -257;
            myCamera.GetComponent<MainCam>().MoveCamera(mapSpot.transform.position);
            myCamera.transform.position = mapSpot.transform.position;
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            player.Freeze = true;
        }
        if (Input.GetKeyUp(KeyCode.M))
        {
            myCamera.GetComponent<Camera>().cullingMask = -1;
            myCamera.GetComponent<MainCam>().MoveCamera(lastCamPos);
            myCamera.transform.position = lastCamPos;
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            player.Freeze = false;
        }
    }

    /// <summary>
    /// Move the Camera to the room the player is in.
    /// </summary>
    /// <param name="target">Location camera will move to</param>
    public void ChangeRoom(Vector3 target)
    {
        if (!shifting)
        {
            shifting = true;
            myCamera.GetComponent<MainCam>().MoveCamera(target);
            lastCamPos = target;
        }

    }

    /// <summary>
    /// Set the bool based on if the camera is moving
    /// </summary>
    /// <param name="_shifting"></param>
    public void SetShifting(bool _shifting)
    {
        shifting = _shifting;
    }

    /// <summary>
    /// Player has finished changing rooms
    /// </summary>
    public void RoomChanged()
    {
        shifting = false;
    }

    private void HandleBars()
    {
        healthBar.fillAmount = MapResource(currentHealth, minHealth, maxHealth, 0, 1);

        energy1.fillAmount = MapResource(currentEnergy1, 0, 10, 0, 1);
        energy2.fillAmount = MapResource(currentEnergy2, 0, 10, 0, 1);
        energy3.fillAmount = MapResource(currentEnergy3, 0, 10, 0, 1);

    }

    private void UpdateNumbers()
    {
        currentHealth = player.health;

        currentEnergy1 = player.Allocation[0] + 1;
        currentEnergy2 = player.Allocation[1] + 1;
        currentEnergy3 = player.Allocation[2] + 1;
    }

    //Maps resource values to 0-1 scale for use with UI
    //(Value,inMin,inMax,outMin,outMax)
    private float MapResource(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }

}//end of Game Manager
