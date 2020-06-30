using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField]
	private float accelerationRate;	//w/s
	[SerializeField]
	private float decelerationModifier;
	[SerializeField]
	private float maxVelocity;

	private float velocity;
	[SerializeField]
	private float stopSpeed;

	[SerializeField]
	private float angVelocity;

	Rigidbody rb;

	//w/s for forward/backward
	//a/d for rotate left/right

	/* 	How does movement work?
	 * 		a/d rotate (can rotate on the spot or while moving)
	 * 		w/s control acceleration forward/backwards. 
	 * 		Can go 100 to 0 quickly, takes a while to get up to 100.
	 * 			i.e. takes a half sec or so to get up to speed, release/press opposite key to brake quickly, then hold opposite key to accelerate in reverse
	 * 		Direction always == GetForward();
	 */

	 //Want to be able to handle::
	 	//	Some forced rotation
		//	
	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		//Operations I want to happen without jitter:
		//Rotation	
		float rotationDirection = Input.GetAxisRaw("Horizontal");
		float deltaAngle = rotationDirection * angVelocity * Time.deltaTime;
		//to rotate my shit, need to do: (Current Quaternion * (Rotation Amount Quaternion) ) Normalized
		transform.rotation *= Quaternion.Euler(0, deltaAngle, 0);

		//Movement forward and back
		float moveDirection = Input.GetAxisRaw("Vertical");
		velocity = Mathf.Clamp(velocity + (accelerationRate * moveDirection) * Time.deltaTime, -maxVelocity, maxVelocity);

		if (moveDirection == 0 || velocity * moveDirection < 0)
		{
			//if accelerating in opposite dir. to vel. or not moving anymore, should rapidly decelerate
			//TODO:: Add Particle + Sound effects centered on this position

			if (velocity != 0)
			{
				velocity *= decelerationModifier;
				velocity = (velocity / Mathf.Abs(velocity)) * ((Mathf.Abs(velocity) > stopSpeed) ? velocity : 0.0f);
			}
		}
		rb.MovePosition(transform.position + transform.forward * velocity * Time.deltaTime);
	}
}