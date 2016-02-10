using UnityEngine;
using System.Collections;

public class FlyingBasicMob : MonoBehaviour {

	Controller c;
	SpriteRenderer sr;
	SquareGenerator sg;
	Vector3 velocity;
	public Vector2 direction;

	public Characteristics stats;

	float velocityXSmoothing;
	float velocityYSmoothing;
	int targetXDirection;
	int targetYDirection;

	void Start () {
		c = GetComponent<Controller> ();
		sr = GetComponent<SpriteRenderer> ();
		sg = GameObject.Find("MapGenerator").GetComponent<SquareGenerator> ();
		velocity = new Vector3 ();
		direction = new Vector2 ();
		stats = new Characteristics ();
	}
		
	void Update () {

		if (c.collisions.above || c.collisions.below)
			velocity.y = 0;

		if (LookForPlayer()) {
			GoToPlayer ();
		} else {
			RandomMovement ();
		}

		if (targetXDirection != 1)
			sr.flipX = true;
		else
			sr.flipX = false;

		velocity.x = Mathf.SmoothDamp (velocity.x, stats.moveSpeed * targetXDirection, ref velocityXSmoothing, stats.airborneAcceleration);
		velocity.y = Mathf.SmoothDamp (velocity.y, stats.moveSpeed * targetYDirection, ref velocityYSmoothing, stats.airborneAcceleration);

	}

	void GoToPlayer () {

		if (direction.x == 0) {
			targetXDirection = -(int)Mathf.Sign (transform.position.x - Mathf.Round (transform.position.x));
		} else {
			targetXDirection = (int)direction.x;
		}

		targetYDirection = (int)direction.y;

	}

	void RandomMovement () {

		if (targetXDirection == 0)
			targetXDirection = 1;

		targetYDirection = (int)Mathf.Sign(Mathf.Cos(Time.timeSinceLevelLoad));
		targetXDirection = UnityEngine.Random.Range(0, 10000) == 1 ? -targetXDirection : targetXDirection;

		RaycastHit2D hit = c.ManualRayCast (c.collisionMask, -1f, 1, 5f);
		if (hit) {
			targetYDirection = 1;
		}

		hit = c.ManualRayCast (c.collisionMask, 1f, 1, 5f);
		if (hit) {
			targetYDirection = -1;
		}

		hit = c.ManualRayCast (c.collisionMask, 1f, 0, 3f);
		if (hit) {
			targetXDirection = -1;
		}

		hit = c.ManualRayCast (c.collisionMask, -1f, 0, 3f);
		if (hit) {
			targetXDirection = 1;
		}
	}

	bool LookForPlayer () {
		direction = sg.LookForPlayer ((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y));

		return (direction != Vector2.zero);
	}

	void FixedUpdate () {
		c.Move (velocity * Time.deltaTime);
	}
}
