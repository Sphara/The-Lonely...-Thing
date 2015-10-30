using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Controller))]
public class Player : MonoBehaviour {

	Controller controller;
	public float jumpHeight = 4;
	public float timeToJumpApex = 0.4f;

	float velocityXSmoothing;

	float groundedAcceleration = 0.1f;
	float airborneAcceleration = 0.3f;
	float moveSpeed = 6;
	float gravity;
	float jumpVelocity;
	float faceDirection = 1;
	float timeJumpWasCalled = -2f;

	Vector3 velocity;
	private Transform childTransform;
	private Animator animator;
	private SquareGenerator sg;
	
	void Start () 
	{
		controller = GetComponent<Controller> ();
		animator = GetComponentInChildren<Animator> ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
		sg = GameObject.Find ("MapGenerator").GetComponent<SquareGenerator>();

		foreach (Transform child in transform)
		{
			childTransform = child;
		}
	}

	void Update ()
	{
		Vector3 newScale = transform.localScale;

		/* Prevent y velocity to buffer while we're grounded or hitting ceiling */

		if (controller.collisions.above || controller.collisions.below)
			velocity.y = 0;

		/* Jump, controls & gravity */

		Vector2 input = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		if (Input.GetKeyDown (KeyCode.Space)) {
			timeJumpWasCalled = Time.timeSinceLevelLoad;
		}

		float TargetHorizontalVelocity = input.x * moveSpeed;

		velocity.x = Mathf.SmoothDamp(velocity.x, TargetHorizontalVelocity, ref velocityXSmoothing, controller.collisions.below ? groundedAcceleration : airborneAcceleration);
		velocity.y += gravity * Time.deltaTime;

		/* Change skin orientation */

		if (input.x == -1)
			faceDirection = 1;
		else if (input.x == 1)
			faceDirection = -1;

		newScale.x = faceDirection;
		childTransform.localScale = newScale;

		/* Set animations */

		if (controller.collisions.below && input.x != 0)
			animator.SetBool ("isWalking", true);
		else
			animator.SetBool ("isWalking", false);

		/* Update diffusion values for pathfinding */

		int diffusionWeight = 10;

		if (sg != null)
			sg.DiffuseValue((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), diffusionWeight);

	}

	void FixedUpdate () {

		if (controller.collisions.below && (Time.timeSinceLevelLoad - timeJumpWasCalled) < 0.1f) {
			velocity.y = jumpVelocity;
			timeJumpWasCalled = -2f;
		}

		controller.Move (velocity * Time.deltaTime);
	}

}
