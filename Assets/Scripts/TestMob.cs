using UnityEngine;
using System.Collections;


/// <summary>
/// Test mob. An ant, actually.
/// </summary>

[RequireComponent(typeof(Controller))]
public class TestMob : MonoBehaviour {

	Controller controller;
	SquareGenerator sg;
	public float jumpHeight = 4;
	public float timeToJumpApex = 0.4f;
	float gravity;

	float moveSpeed = 6;
	float velocityXSmoothing;
	
	float groundedAcceleration = 0.1f;
	float airborneAcceleration = 0.3f;

	float jumpVelocity;

	Vector3 velocity;

	int targetDirection;
	Vector2 direction;

	void Start () {
		sg = GameObject.Find ("MapGenerator").GetComponent<SquareGenerator>();
		controller = GetComponent<Controller> ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		targetDirection = (int)UnityEngine.Random.Range(0, 1) == 0 ? 1 : -1;
		jumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
	}

	void RandomMovement () {
		if (controller.collisions.left && controller.collisions.below)
			targetDirection = 1;
		else if (controller.collisions.right && controller.collisions.below)
			targetDirection = -1;
		
		if ((int)UnityEngine.Random.Range (0, 100) == 1 && controller.collisions.below)
			velocity.y = jumpVelocity;
	}

	bool LookForPlayer () {
		direction = sg.LookForPlayer ((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y));

		return (direction != Vector2.zero);
	}

	void GoToPlayer () {

		// HOTFIX
		if (direction.x != 0 && direction.y != 0)
			direction.y = 0;

		if (direction.y == 1 && controller.collisions.below)
			velocity.y = jumpVelocity;

		if (direction.y != 0) {
			targetDirection = -(int)Mathf.Sign(transform.position.x - Mathf.Round(transform.position.x));
		} else {
			targetDirection = (int)direction.x;
		}
	}

	void Update () {
	
		if (controller.collisions.above || controller.collisions.below)
			velocity.y = 0;

		if (LookForPlayer()) {
			GoToPlayer ();
		} else {
			RandomMovement ();
		}

		velocity.x = Mathf.SmoothDamp(velocity.x, moveSpeed * targetDirection, ref velocityXSmoothing, controller.collisions.below ? groundedAcceleration : airborneAcceleration);
		velocity.y += gravity * Time.deltaTime;

	}

	void FixedUpdate ()
	{
		controller.Move (velocity * Time.deltaTime);
	}
}
