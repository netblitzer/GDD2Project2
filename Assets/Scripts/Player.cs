using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool debug;
    public float health;
    public float maxSpeed;
    public GameObject bulletPrefab;

    private Vector3 velocity;
    private float speed;
    private Vector3 prevMousePosition;
    private Vector3 currMousePosition;
    private float shootFreezeTime;

    private DebugLines lines;

    private int energy;         // Available energy
    private int[] allocation;   // 0 == Square, 1 == Circle, 2 == Triangle

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

    // Use this for initialization
    void Start()
    {
        allocation = new int[3];

        health = 100.0f;
        speed = 0f;
        shootFreezeTime = 0f;
        velocity = Vector3.zero;
        prevMousePosition = Input.mousePosition;
        currMousePosition = Input.mousePosition;
        charControl = gameObject.GetComponent<CharacterController>();
        lines = FindObjectOfType<DebugLines>();

        allocation[0] = 3;
        allocation[1] = 3;
        allocation[2] = 3;
    }

    // Update is called once per frame
    void Update()
    {
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
            shootFreezeTime = 0.25f;
        }

        // lastly regen health over time
        health = validateHealth(health += Time.deltaTime);

    }//end of Update

    // validate health value, keep between 0 and 100 (like, duh)
    private float validateHealth(float value)
    {
        return value >= 0 && value <= 100 ? value : value < 0 ? 0 : 100;
    }

    //Take damage from enemy
    public void TakeDamage(float dmg)
    {
        if (dmg > 0.0f && dmg < 10.0f)
        {
            health -= dmg;
        }
    }

    // Could refactor into RemoveEnergy & AddEnergy

    public bool SetPlayerEnergy(int _energy)
    {
        if (_energy < 0)
            return false;

        if (energy < _energy)
        {
            energy = _energy;
        }

        if (energy > _energy)
        {
            int diff = energy - _energy;
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
                energy--;
                diff--;
            }
        }

        return true;
    }

    // For when player removes energy
    public void SubStrength(int index)
    {
        if (index >= 0 && index < allocation.Length)
            if (allocation[index] > 0)
            {
                allocation[index]--;
                energy++;
            }
    }

    // For when player adds energy
    public void AddStrength(int index)
    {
        if (index >= 0 && index < allocation.Length)
        {
            if (energy >= 0)
            {
                allocation[index]++;
                energy--;
            }
        }
    }

    // create a bullet yo!
    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab) as GameObject;
        bullet.transform.position = transform.position + transform.forward;
        Bullet bulletBehavior = bullet.GetComponent<Bullet>();
        bulletBehavior.Fire(transform.forward);
    }
}//end of player