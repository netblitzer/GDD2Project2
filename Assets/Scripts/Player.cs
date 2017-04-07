using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameManager gm;

    private bool debug;
    private bool freeze;

    private Vector3 velocity;
    private float speed;
    private Vector3 prevMousePosition;
    private Vector3 currMousePosition;

    private float shootFreezeTime;
    private float regenMod;

    private DebugLines lines;

    private int energy;         // Available energy
    private int[] allocation;   // 0 == Square, 1 == Circle, 2 == Triangle

    [SerializeField]
    private float shootTime;

    public float health;

    public float maxSpeed;

    public GameObject bulletPrefab;

    //Properties
    public float ShootTime
    {
        get { return shootTime; }
        set { shootTime = value; }
    }

    public bool Freeze
    {
        set { freeze = value; }
    }

    public bool Debugger 
	{
		set { debug = value; }
	}

    public int Energy
    {
        get { return energy; }
    }

    public int[] Allocation
    {
        get { return allocation; }
    }

    //property for velocity
    public Vector3 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    //internal reference to the CharacterController component
    protected CharacterController charControl;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        gm = FindObjectOfType<GameManager>();

        allocation = new int[3];

        health = 100.0f;
        speed = 0f;
        shootFreezeTime = 0f;
        regenMod = 1.0f;
        velocity = Vector3.zero;
        prevMousePosition = Input.mousePosition;
        currMousePosition = Input.mousePosition;
        charControl = gameObject.GetComponent<CharacterController>();
        lines = FindObjectOfType<DebugLines>();

        allocation[0] = 3;
        allocation[1] = 3;
        allocation[2] = 3;

        freeze = false;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //Update values from game mangager for testing
        //-------------------------------------------------------------------------------------------
        shootTime = gm.shotSpeed;
        health = gm.health;
        speed = gm.speed;
        regenMod = gm.playerRegen;
        //-------------------------------------------------------------------------------------------

        if (gameObject.transform.position.y > 0.5f)
        {
            //Debug.Log(gameObject.transform.position.y);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
        }
        // rotate the player using mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit mouseToGround;
        if (Physics.Raycast(ray, out mouseToGround))
        {
            Vector3 mousePoint2D = new Vector3(mouseToGround.point.x, 0, mouseToGround.point.z);
            Vector3 player2D = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 playerToPoint = mousePoint2D - player2D;

            transform.forward = playerToPoint.normalized;

            if (debug)
            {
                lines.addLine(mouseToGround.point, mouseToGround.point + mouseToGround.normal * 2, 4);
                lines.addLine(gameObject.transform.position, gameObject.transform.position + playerToPoint * 2, 5);
                lines.addLine(gameObject.transform.position, gameObject.transform.position + gameObject.transform.forward * 2, 2);
                lines.addLine(gameObject.transform.position, gameObject.transform.position + gameObject.transform.right * 2, 3);
            }

        }

        // move
        //Get arrow key input
        if (Input.anyKey)
        {
            speed += .1f;
        }

        if (!freeze)
        {
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

            // clamp speed
            speed = Mathf.Clamp(speed, 0, maxSpeed);
            charControl.Move(velocity.normalized * speed * Time.deltaTime);
            velocity = Vector3.zero;
        }

        // shoot logic
        if (shootFreezeTime > 0f)
        {
            shootFreezeTime -= Time.deltaTime;
            if (shootFreezeTime < 0f)
            {
                shootFreezeTime = 0f;
            }
        }
        if (Input.GetMouseButton(0) && shootFreezeTime == 0f)
        {
            Shoot();
            shootFreezeTime = shootTime;
        }

        // lastly regen health over time
        health = validateHealth(health += (Time.deltaTime * regenMod));


        //Debug.Log("Update: " + allocation[0]);
    }//end of Update

    /// <summary>
    /// Validate health value, keep between 0 and 100 (like, duh)
    /// </summary>
    /// <param name="value">Current health</param>
    /// <returns>Real health</returns>
    private float validateHealth(float value)
    {
        return value >= 0 && value <= 100 ? value : value < 0 ? 0 : 100;
    }

    /// <summary>
    /// Take damage from enemy
    /// </summary>
    /// <param name="dmg">Damage amount player takes</param>
    public void TakeDamage(float dmg)
    {
        if (dmg > 0.0f && dmg < 10.0f)
        {
            health -= dmg;
        }
    }

    /// <summary>
    /// Update the available energy player has
    /// </summary>
    /// <param name="_energy">Current amount of energy available</param>
    /// <returns>Returns false if energy amount is invalid</returns>
    public bool SetPlayerEnergy(int _energy)
    {
        if (_energy < 0)
            return false;

        if (SumAllocation() < _energy)
        {
            energy = _energy - SumAllocation();
        }
        else if (SumAllocation() > _energy)
        {
            int diff = SumAllocation() - _energy;
            while (diff > 0)
            {
                // Find energy type with highest value, reduce it by 1
                int max = allocation[0];
                int index = 0;

                if (allocation[1] > max)
                {
                    max = allocation[1];
                    index = 1;
                }

                if (allocation[2] > max)
                {
                    max = allocation[2];
                    index = 2;
                }

                allocation[index]--;
                diff--;
            }
        }
        
        return true;
    }

    /// <summary>
    /// For when player removes energy
    /// </summary>
    /// <param name="index">Energy tpye to subtract from</param>
    public void SubStrength(int index)
    {
        if (index >= 0 && index < allocation.Length)
            if (allocation[index] > 0)
            {
                Debug.Log(index);
                allocation[index]--;
                energy++;
            }
    }

    /// <summary>
    /// For when player adds energy
    /// </summary>
    /// <param name="index">Energy type to add to</param>
    public void AddStrength(int index)
    {
        if (index >= 0 && index < allocation.Length)
        {
            if (energy > 0)
            {
                allocation[index]++;
                energy--;
            }
        }
    }

    /// <summary>
    /// create a bullet yo!
    /// </summary>
    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab) as GameObject;
        bullet.transform.position = transform.position + transform.forward;
        Bullet bulletBehavior = bullet.GetComponent<Bullet>();
        bulletBehavior.Fire(transform.forward);
    }

    /// <summary>
    /// Add player's allocated energy together
    /// </summary>
    /// <returns>Sum of energy allocation</returns>
    private int SumAllocation()
    {
        return allocation[0] + allocation[1] + allocation[2];
    }

    /// <summary>
    /// Check if player is dead.
    /// </summary>
    /// <returns>True if player is dead else false</returns>
    public bool IsDead()
    {
        if (health <= 0)
            return true;
        else
            return false;
    }
}//end of player