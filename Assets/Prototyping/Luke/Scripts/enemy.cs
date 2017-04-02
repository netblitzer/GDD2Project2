using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { triangle, square, circle };

public class enemy : MonoBehaviour {


    // public variables to change
    [Tooltip("The max health of the enemy.")]
	public int maxHealth = 10;

	[Tooltip("The max speed of the enemy.")]
	public int maxSpeed = 4;

	[Tooltip("The max force of the enemy controller.")]
	public int maxForce = 10;

		// weights for controls
	public float seekWeight = 1f;
	public float pursueWeight = 0f;
	public float fleeWeight = 1f;
	public float evadeWeight = 0f;
	public float wanderWeight = 2f;
	public float wanderRadius = 2f;
	public float wanderOffset = 2f;
	public float separationWeight = 2f;
	public float avoidWeight = 2f;
    public float friction = 0.1f;

    // physics variables
    private Vector3 forces;
	private Vector3 acceleration;
	public Vector3 velocity;
	private Vector3 position;

	// physics status variables
	private float speed;
	private Vector3 direction;
	private Vector2 directionXY;
	private Vector2 positionXY;
    private Vector3 wanderPos;
    public GameObject target;

	// enemy variables
	public float health;
	public float damage;
	public EnemyType type;

	// constructor
	public void init(int _maxH, int _maxS, int _maxF, float _damage, int _type, Vector3 _startPos, GameObject _target) {
		maxHealth = _maxH;
		maxSpeed = _maxS;
		maxForce = _maxF;

        health = maxHealth;
        damage = _damage;

        wanderPos = Vector3.zero;
        speed = 0;
        direction = Vector3.forward;
        directionXY = Vector3.forward;
        positionXY = Vector3.zero;

        switch (_type) {
		    default:
		    case 1:
			    type = EnemyType.triangle;
                //gameObject.GetComponent<Material>().color = Color.red;
			    break;
		    case 2:
			    type = EnemyType.square;
                //gameObject.GetComponent<Material>().color = Color.green;
                break;
		    case 3:
			    type = EnemyType.circle;
                //gameObject.GetComponent<Material>().color = Color.blue;
                break;
		}

		if (_startPos != null) {
			position = _startPos;
		} else {
            position = Vector3.zero;

        }

		if (_target != null) {
			target = _target;
		}

        resetPhysicVars();
	}

	// Use this for initialization
	void Start () {
        //switch (type) {
        //    default:
        //    case EnemyType.triangle:
        //        gameObject.GetComponent<Material>().color = Color.red;
        //        break;
        //    case EnemyType.square:
        //        gameObject.GetComponent<Material>().color = Color.green;
        //        break;
        //    case EnemyType.circle:
        //        gameObject.GetComponent<Material>().color = Color.blue;
        //        break;
        //}
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	// generic function to add a force to the enemy
	public bool addForce(Vector3 _force) {
		if (_force == null) {
			return false;
		}

		forces += _force;

		return true;
	}

    // basic friction method to slow the velocity
    private void calcFriction() {
        forces += (velocity * -friction);
    }

    // basic seek method with the target being what we're seeking
    private Vector3 calcSeek() {
        Vector3 force = Vector3.zero;

        if (target != null) {
            Vector3 desiredVel = target.transform.position - position;
            force += desiredVel - velocity;
        }

        return force;
    }

    /*  Needs to know player info before it can be implemented
    private Vector3 calcPursue(float _timeAhead) {
        Vector3 force = Vector3.zero;

        if (target != null) {
            Player ply = target.getComponent<Player>();

            if (ply != null) {
                Vector3 desiredVel = (target.transform.position + ply.velocity * _timeAhead) - position;
                force += desiredVel - velocity;
            } else {
                Vector3 desiredVel = target.transform.position - position;
                force += desiredVel - velocity;
            }
        }

        return force;
    }
    */

    // basic flee method with the target being what we're fleeing from
    private Vector3 calcFlee() {
        Vector3 force = Vector3.zero;

        if (target != null) {
            Vector3 desiredVel = target.transform.position - position;
            force += velocity - desiredVel;
        }

        return force;
    }


    /*  Needs to know player info before it can be implemented
    private Vector3 calcEvade(float _timeAhead) {
        Vector3 force = Vector3.zero;

        if (target != null) {
            Player ply = target.getComponent<Player>();

            if (ply != null) {
                Vector3 desiredVel = (target.transform.position + ply.velocity * _timeAhead) - position;
                force += velocity - desiredVel;
            } else {
                Vector3 desiredVel = target.transform.position - position;
                force +=  velocity - desiredVel;
            }
        }

        return force;
    }
    */

    // offset circle method of wandering
    private Vector3 calcWanderCircle() {
        Vector3 force = Vector3.zero;

        Vector3 wanderTarget = position + (direction * wanderOffset);

        float randomAng = Random.Range(0, Mathf.PI * 2f);

        wanderTarget += new Vector3(Mathf.Cos(randomAng), 0, Mathf.Sin(randomAng)) * wanderRadius;

        Vector3 desiredVel = wanderTarget - position;
        force += desiredVel - velocity;

        return force;
    }

    // wander to a point and find a new random point method
    private Vector3 calcWanderToLoc() {
        Vector3 force = Vector3.zero;

        float distTo = (wanderPos - position).sqrMagnitude;

        // get new point to 'wander' to
        if (distTo < 1) {
            wanderPos = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        }

        // seek the wanderPos point
        Vector3 desiredVel = wanderPos - position;
        force += desiredVel - velocity;

        return force;
    }

    public bool calcMovement() {

        // calucate forces
        calcFriction();
        forces += calcSeek() * seekWeight;
        //forces += calcPursue() * pursueWeight;
        forces += calcFlee() * fleeWeight;
        //forces += calcEvade() * evadeWeight;
        forces += calcWanderCircle() * wanderWeight;
        //forces += calcWanderToLoc() * wanderWeight;

        // add forces to velocity
        if (forces != Vector3.zero) {
            // scale forces
            float forceScalar = Mathf.Min(forces.sqrMagnitude, maxForce);

            forces.Normalize();
            forces *= forceScalar;

            acceleration += forces;

            velocity += acceleration * Time.deltaTime;
        }

        // zero out velocity if it's low enough
        if (velocity.sqrMagnitude < 0.001) {
            velocity = Vector3.zero;
        } else {    // scale velocity
            float velScalar = Mathf.Min(velocity.sqrMagnitude, maxSpeed);

            velocity.Normalize();
            velocity *= velScalar;
            velocity.y = 0;
        }

        // add velocity to position
        position += velocity * Time.deltaTime;

        positionXY.Set(position.x, position.z);

        gameObject.transform.position = position;

        // set status variables
        direction = velocity.normalized;
        directionXY.Set(direction.x, direction.z);

        // reset the forces and acceleration
        resetPhysicVars();

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
}
