using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyTypes { triangle, square, circle };

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour {



    // public variables to change
    [Tooltip("The max health of the enemy.")]
    public int maxHealth = 9;

    [Tooltip("The max speed of the enemy.")]
    public int maxSpeed = 4;

    [Tooltip("The max force of the enemy controller.")]
    public int maxForce = 10;

    public bool debug;

    // weights for controls
    public float wanderOffset = 2f;
    public float wanderRefresh = 3f;

    // physics variables
    private Vector3 forces;
    private Vector3 acceleration;
    private Vector3 velocity;
    private Vector3 position;
    private NavMeshAgent agent;

    // physics status variables
    private float speed;
    private Vector3 direction;
    private Vector2 directionXY;
    private Vector2 positionXY;
    private Vector3 wanderPos;
    private float wanderTimer = 0;
    public GameObject target;

    // enemy variables
    public float health;
    public float damage;
    public EnemyTypes type;

    // reference to the game's DebugLines script
    private DebugLines lines;

    // constructor
    public void init(int _maxH, int _maxS, int _maxF, float _damage, EnemyTypes _type, Vector3 _startPos, GameObject _target) {
        maxHealth = _maxH;
        maxSpeed = _maxS;
        maxForce = _maxF;

        health = maxHealth;
        damage = _damage;

        agent = gameObject.GetComponent<NavMeshAgent>();
        lines = FindObjectOfType<DebugLines>();

        gameObject.transform.position = _startPos;
        findNewWanderLocation();
        agent.SetDestination(wanderPos);
        speed = 0;
        direction = Vector3.forward;
        directionXY = Vector3.forward;
        positionXY = Vector3.zero;

        //Debug.Log(_type);
        type = _type;

        /*
        switch (_type) {
            default:
            case EnemyTypes.triangle:
                gameObject.GetComponent<Renderer>().material.color = Color.red;
                break;
            case EnemyTypes.square:
                gameObject.GetComponent<Renderer>().material.color = Color.green;
                break;
            case EnemyTypes.circle:
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
                break;
        }
        */

        if (_startPos != null) {
            position = _startPos;
        }
        else {
            position = Vector3.zero;

        }

        if (_target != null) {
            target = _target;
        }
        
    }

    // wander for navmesh
    private void calcNavMeshWander() {
        float distTo = (wanderPos - position).magnitude;

        if (debug)
        {
            lines.addLine(wanderPos, position, 5);
        }

        // get new point to 'wander' to
        if (distTo < 2f || wanderTimer > wanderRefresh) {
            findNewWanderLocation();
            wanderTimer = 0f;
        } else {
            wanderTimer += Time.deltaTime;
        }
    }
    
    // new wander method to use with the navmesh
    private void findNewWanderLocation() {
        wanderPos = gameObject.transform.position + new Vector3(Random.Range(-6f, 6f), 0, Random.Range(-6f, 6f));
        wanderPos = Vector3.Max(new Vector3(-35f, 0, -25), Vector3.Min(new Vector3(35, 1, 25), wanderPos));

        // find the closest point on the navmesh
        NavMeshHit navHit;
        int tries = 0;
        do {
            if (NavMesh.SamplePosition(wanderPos, out navHit, wanderOffset, 0)) {
                wanderPos = navHit.position;
                break;
            }
        } while (++tries < 2);

        agent.SetDestination(wanderPos);
    }

    public bool calcMovement() {
        
        if (target != null) {
            agent.SetDestination(target.transform.position);
            //Debug.Log("Seeking player");
        } else {
            calcNavMeshWander();
        }

        // add velocity to position
        positionXY.Set(position.x, position.z);

        // move the enemy

        position = gameObject.transform.position;

        if (debug)
        {
            lines.addLine(position, position + gameObject.transform.forward * 2, 2);
            lines.addLine(position, position + gameObject.transform.right * 2, 3);
        }

        // reset the forces and acceleration
        //resetPhysicVars();

        return true;
    }

    // function to reset the forces and acceleration
    private void resetPhysicVars() {
        forces = Vector3.zero;
        acceleration = Vector3.zero;
    }

    // function to damage the enemy by an amount
    public bool damageEnemy(float _amount) {
        if (_amount < 0) {
            return healEnemy(_amount);
        }

        health -= _amount;

        return true;
    }

    // function to heal the enemy
    public bool healEnemy(float _amount) {
        if (_amount > 0) {
            return damageEnemy(_amount);
        }

        health -= _amount;

        return true;
    }

    // function to see if the enemy is below 0 health
    public bool isDead() {
        if (health <= 0) {
            return true;
        }

        return false;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Bullet") {
            Debug.Log("Bullet Hit Enemy");
            damageEnemy(1.0f);
        }

    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

    }


    private void OnCollisionExit(Collision collision)
    {
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    // function to set the target of each enemy
    // even if the target is null, this will be fine since then the enemy will just wnader
    void setTarget(GameObject _target) {
        target = _target;
    }
}
