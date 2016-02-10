using UnityEngine;
using System.Collections;

/// <summary>
/// Test mob. An ant, actually.
/// </summary>

[RequireComponent(typeof(Controller))]
public class TestMob : MonoBehaviour {

	Controller controller;
	SpriteRenderer sr;
	public Characteristics stats;
	SquareGenerator sg;

	float gravity;
	float velocityXSmoothing;

	float jumpVelocity;

	Vector3 velocity;

	int targetDirection;
	public Vector2 direction;

	void Start () {
		stats = new Characteristics();
		sr = GetComponent<SpriteRenderer> ();
		sg = GameObject.Find ("MapGenerator").GetComponent<SquareGenerator>();
		controller = GetComponent<Controller> ();
		gravity = -(2 * stats.jumpHeight) / Mathf.Pow (stats.timeToJumpApex, 2);
		targetDirection = (int)UnityEngine.Random.Range(0, 1) == 0 ? 1 : -1;
		jumpVelocity = Mathf.Abs (gravity) * stats.timeToJumpApex;
	}

	void RandomMovement () {

		if (targetDirection == 0)
			targetDirection = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;

		if (controller.collisions.left && controller.collisions.below)
			targetDirection = 1;
		else if (controller.collisions.right && controller.collisions.below)
			targetDirection = -1;
		else if ((int)UnityEngine.Random.Range (0, 100) == 1)
			targetDirection = -targetDirection;
		
		if ((int)UnityEngine.Random.Range (0, 100) == 1 && controller.collisions.below)
			velocity.y = jumpVelocity;
	}

	bool LookForPlayer () {
		direction = sg.LookForPlayer ((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y));

		return (direction != Vector2.zero);
	}

	void GoToPlayer () {

		if (direction.y != 0) {
			if (direction.x != 0) {
				direction.y = 0;
			} else {
				targetDirection = -(int)Mathf.Sign (transform.position.x - Mathf.Round (transform.position.x));
			}
		} else {
			targetDirection = (int)direction.x;
		}
			
		if (direction.y == 1 && controller.collisions.below)
			velocity.y = jumpVelocity;
	}

	void Update () {

		if (controller.collisions.above || controller.collisions.below)
			velocity.y = 0;

		if (LookForPlayer()) {
			GoToPlayer ();
		} else {
			RandomMovement ();
		}

		velocity.x = Mathf.SmoothDamp(velocity.x, stats.moveSpeed * targetDirection, ref velocityXSmoothing, controller.collisions.below ? stats.groundedAcceleration : stats.airborneAcceleration);
		velocity.y += gravity * Time.deltaTime;

		if (targetDirection == 1)
			sr.flipX = true;
		else
			sr.flipX = false;
	}

	void FixedUpdate ()
	{
		controller.Move (velocity * Time.deltaTime);
	}
}
