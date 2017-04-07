using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyTypes { square, circle, triangle };

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
    private GameManager gm;

    public GameObject target;

    // enemy variables
    public float health;
    public float damage;
    public EnemyTypes type;

    // reference to the game's DebugLines script
    private DebugLines lines;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="_maxH">Max Health</param>
    /// <param name="_maxS">Max Speed</param>
    /// <param name="_maxF">Max Force</param>
    /// <param name="_damage">Damage</param>
    /// <param name="_type">Enemy Type</param>
    /// <param name="_startPos">Starting Position</param>
    /// <param name="_target">Target to Seek</param>
    public void init(int _maxH, int _maxS, int _maxF, float _damage, EnemyTypes _type, Vector3 _startPos, GameObject _target) {
        gm = GameObject.FindObjectOfType<GameManager>();

        type = _type;
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

    /// <summary>
    /// wander for navmesh
    /// </summary>
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

    /// <summary>
    /// new wander method to use with the navmesh
    /// </summary>
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

    /// <summary>
    /// Calculate Movement
    /// </summary>
    /// <returns></returns>
    public bool calcMovement() {
        
        if (target != null) {
            agent.SetDestination(target.transform.position);

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

        return true;
    }

    /// <summary>
    /// function to reset the forces and acceleration
    /// </summary>
    private void resetPhysicVars() {
        forces = Vector3.zero;
        acceleration = Vector3.zero;
    }

    /// <summary>
    /// function to damage the enemy by an amount
    /// </summary>
    /// <param name="_amount">Amount to damage enemy by</param>
    /// <returns>True if valid</returns>
    public bool damageEnemy(float _amount) {
        if (_amount < 0) {
            return healEnemy(_amount);
        }

        health -= _amount;

        return true;
    }

    /// <summary>
    /// function to heal the enemy
    /// </summary>
    /// <param name="_amount">Amount to heal enemy by</param>
    /// <returns>True if valid</returns>
    public bool healEnemy(float _amount) {
        if (_amount > 0) {
            return damageEnemy(_amount);
        }

        health -= _amount;

        return true;
    }

    /// <summary>
    /// function to see if the enemy is below 0 health
    /// </summary>
    /// <returns>True if dead</returns>
    public bool isDead() {
        if (health <= 0) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Detect collision for bullet
    /// </summary>
    /// <param name="collision">Collision object</param>
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Bullet")
        {
            damageEnemy(gm.TakeDamage(type));
        }

    }

    /// <summary>
    /// Dectect if hitting player and staying on player
    /// </summary>
    /// <param name="collision">Collision object</param>
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

    }

    /// <summary>
    /// If not colliding with player, move again
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    /// <summary>
    /// function to set the target of each enemy
    /// even if the target is null, this will be fine since then the enemy will just wnader
    /// </summary>
    /// <param name="_target">seek target</param>
    void setTarget(GameObject _target) {
        target = _target;
    }
}
