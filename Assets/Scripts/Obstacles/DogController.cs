using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(GrimeAffector))]
public class DogController : MonoBehaviour
{
	private NavMeshAgent agent;
	private GrimeAffector grimeAffector;
	private Transform player;

	private enum State {Patrol, Chase, Bark, Return, Flee, Eat, Die};
	[SerializeField]
	private State state;

	[SerializeField]
	private Transform goal;
	[SerializeField]
	private Transform foodBowl;
	[SerializeField]
	private Vector3[] patrolPoints;
	[SerializeField]
	private int patrolIndex;
	[SerializeField]
	private float patrolDistance;
	[SerializeField]
	private float nearChaseDistance;
	[SerializeField]
	private float farChaseDistance;
	[SerializeField]
	private float chaseDuration;
	private float chaseTimer;
	[SerializeField]
	private float chaseAngle;
	[SerializeField]
	private float barkDistance;
	[SerializeField]
	private float eatDistance;
	[SerializeField]
	private float eatDuration;
	private float eatTimer;

	private Quaternion startRotation;
	private Quaternion endRotation;
	
    // Start is called before the first frame update
    void Start()
    {
		startRotation = new Quaternion();
		endRotation = new Quaternion();
		agent = GetComponent<NavMeshAgent>();
		grimeAffector = GetComponent<GrimeAffector>();
		//agent.updateRotation = false;
		if (!player)
			player = GameObject.FindGameObjectWithTag("Player").transform;
		if (!goal)
			goal = GameObject.FindGameObjectWithTag("DogHouse").transform;
		if (!foodBowl)
			foodBowl = GameObject.FindGameObjectWithTag("FoodBowl").transform;
		if (patrolPoints.Length <= 0)
		{
			patrolPoints = new Vector3[1];
			patrolPoints[0] = transform.position;
		}
    }

    // Update is called once per frame
    void Update()
    {
		switch (state)
		{
			case (State.Patrol):	Patrol(); break;
			case (State.Chase):	Chase(); break;
			case (State.Bark):	Bark(); break;
			case (State.Return):	Return(); break;
			case (State.Flee):	Flee(); break;
			case (State.Eat):	Eat(); break;
			case (State.Die):	Die(); break;
		}
    }

	void Patrol()
	{
		if (Mathf.Pow(transform.position.x - patrolPoints[patrolIndex].x, 2) + Mathf.Pow(transform.position.z - patrolPoints[patrolIndex].z, 2) < Mathf.Pow(patrolDistance, 2))
		{
			patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
			agent.destination = patrolPoints[patrolIndex];
		}
		
		RaycastHit hit;
        Vector3 playerDir = player.position - transform.position;

		if (Physics.Raycast(transform.position, playerDir.normalized, out hit, farChaseDistance))
		{
			if (hit.collider.CompareTag("Player") && hit.distance < farChaseDistance)
			{
        		float angle = Vector3.Angle(playerDir.normalized, transform.forward);
				if (angle < chaseAngle)
				{
					state = State.Chase;
					chaseTimer = 0.0f;
				}
			}
		}
		if (Mathf.Pow(transform.position.x - player.position.x, 2) + Mathf.Pow(transform.position.z - player.position.z, 2) < Mathf.Pow(nearChaseDistance, 2))
		{
			state = State.Chase;
			chaseTimer = 0.0f;
		}
		
		Vector3 deltaPos = foodBowl.position - transform.position;
		float distToFoodSquared = Mathf.Pow(deltaPos.x, 2) + Mathf.Pow(deltaPos.z, 2);
		if (distToFoodSquared <= Mathf.Pow(eatDistance, 2))
		{
			PrepEat();
		}
	}
	void Chase()
	{
		agent.destination = player.position;
		chaseTimer = Mathf.Min(chaseTimer + Time.deltaTime, chaseDuration);

		Vector3 deltaPos = player.position - transform.position;
		float playerDistSquared = Mathf.Pow(deltaPos.x, 2) + Mathf.Pow(deltaPos.z, 2);
		if (playerDistSquared < Mathf.Pow(barkDistance, 2))
			state = State.Bark;

		if (playerDistSquared > Mathf.Pow(farChaseDistance, 2) || chaseTimer >= chaseDuration)
		{
			agent.destination = patrolPoints[patrolIndex];
			state = State.Return;
		}
			
		
		deltaPos = foodBowl.position - transform.position;
		float distToFoodSquared = Mathf.Pow(deltaPos.x, 2) + Mathf.Pow(deltaPos.z, 2);
		if (distToFoodSquared <= Mathf.Pow(eatDistance, 2))
		{
			PrepEat();
		}

	}
	void Bark()
	{
		Vector3 deltaPos = player.position - transform.position;
		float playerDistSquared = Mathf.Pow(deltaPos.x, 2) + Mathf.Pow(deltaPos.z, 2);
		if (playerDistSquared > Mathf.Pow(nearChaseDistance, 2))
			state = State.Chase;
	}
	void Return()
	{
		Vector3 deltaPos = patrolPoints[patrolIndex] - transform.position;
		float distToPatrolSquared = Mathf.Pow(deltaPos.x, 2) + Mathf.Pow(deltaPos.z, 2);
		if (distToPatrolSquared <= patrolDistance)
			state = State.Patrol;
		if (Mathf.Pow(transform.position.x - player.position.x, 2) + Mathf.Pow(transform.position.z - player.position.z, 2) < Mathf.Pow(nearChaseDistance, 2))
		{
			state = State.Chase;
			chaseTimer = 0.0f;
		}
		
		deltaPos = foodBowl.position - transform.position;
		float distToFoodSquared = Mathf.Pow(deltaPos.x, 2) + Mathf.Pow(deltaPos.z, 2);
		if (distToFoodSquared <= Mathf.Pow(eatDistance, 2))
		{
			PrepEat();
		}
	}
	void Flee()
	{
	}
	void Eat()
	{
		agent.isStopped = true;
		eatTimer = Mathf.Min(eatTimer + Time.deltaTime, eatDuration);
		if (eatTimer == eatDuration)
		{
			agent.destination = goal.position;
			agent.isStopped = false;
			state = State.Die;
			grimeAffector.SetActive(false);
			agent.speed *= 0.5f;
		}
	}
	void Die()
	{
		Vector3 deltaPos = foodBowl.position - transform.position;
		float distToBed = Mathf.Pow(deltaPos.x, 2) + Mathf.Pow(deltaPos.z, 2);
	}

	
	void PrepEat()
	{
			state = State.Eat;
			startRotation = transform.rotation;
			//Find Angle towards target;
			Vector3 toFood = (foodBowl.position - transform.position);
        	float angle = Vector3.Angle(toFood.normalized, Vector3.forward);
			endRotation = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
			eatTimer = 0.0f;
	}
	/*	Art Used:
	 *	https://poly.google.com/view/bIG-wEodRRS	//Rottweiler
	 *	https://poly.google.com/view/eDmSVe4TKwF	//Weiner Dog
	 *	https://poly.google.com/view/3l7jDlX2jcy	//Cushion
	 */
}