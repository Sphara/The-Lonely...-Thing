using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Controller))]
public class Player : MonoBehaviour {

	Controller controller;
	public Characteristics stats;

	float velocityXSmoothing;

	float gravity;
	float jumpVelocity;
	public float faceDirection = 1;
	float timeJumpWasCalled = -2f;

	Vector3 velocity;
	private Transform childTransform;
	private Animator animator;
	public SquareGenerator sg;
	public Shovel shovel;
	
	void Start () 
	{
		stats = new Characteristics ();
		controller = GetComponent<Controller> ();
		animator = GetComponentInChildren<Animator> ();
		gravity = -(2 * stats.jumpHeight) / Mathf.Pow (stats.timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs (gravity) * stats.timeToJumpApex;

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

		/* Digging ! */

		if (Input.GetKeyDown (KeyCode.E)) {
			shovel.dig ();
		}

		float TargetHorizontalVelocity = input.x * stats.moveSpeed;

		velocity.x = Mathf.SmoothDamp(velocity.x, TargetHorizontalVelocity, ref velocityXSmoothing, controller.collisions.below ? stats.groundedAcceleration : stats.airborneAcceleration);
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

		int diffusionWeight = 50;

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
