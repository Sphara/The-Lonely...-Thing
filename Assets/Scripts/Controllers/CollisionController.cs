using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// A controller for basic moving objects like the player & the mobs. Supports slope climbing & collisions with stuff
/// </summary>
public class CollisionController : Controller {

	protected float maxSlopeAngle = 70;
	protected float maxDescendAngle = 80;

	protected Dictionary<int, CollisionEventFunction> CollisionHandleDic;

	public override void Start () {
		base.Start ();

		CollisionEventFunction obstacle = ObstacleCollision;
		CollisionEventFunction enemy = EnemyCollision;
		CollisionEventFunction buff = BuffCollision;
		CollisionEventFunction player = PlayerCollision;

		CollisionHandleDic = new Dictionary<int, CollisionEventFunction> ();
		CollisionHandleDic.Add (LayerMask.NameToLayer("CollisionLayer"), obstacle);
		CollisionHandleDic.Add (LayerMask.NameToLayer("EnemyLayer"), enemy);
		CollisionHandleDic.Add (LayerMask.NameToLayer("BuffLayer"), buff);
		CollisionHandleDic.Add (LayerMask.NameToLayer("P-layer"), player);
	}

	public override void Move (Vector3 velocity, bool standingOnPlatform = false) {
		UpdateRaycastOrigins ();
		collisions.Reset ();
		collisions.oldVelocity = velocity;

		if (velocity.y < 0) {
			DescendSlope(ref velocity);
		}

		if (velocity.x != 0)
			HorizontalCollision (ref velocity);

		if (velocity.y != 0)
			VerticalCollision (ref velocity);

		transform.Translate (velocity);

		if (standingOnPlatform) {
			collisions.below = true;
		}
	}

	protected virtual void ObstacleCollision(RaycastHit2D hit, ref Vector3 velocity, ref float Direction, ref float rayLength, bool isVertical) {

		if (isVertical) {
			velocity.y = (hit.distance - skinWidth) * Direction;
			rayLength = hit.distance;

			if (collisions.climbingASlope) {
				velocity.x = velocity.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (velocity.x);
			}

			collisions.above = Direction == 1;
			collisions.below = Direction == -1;

		} else {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

			if (i == 0 && slopeAngle <= maxSlopeAngle) {
				float distanceToSlope = 0;

				if (slopeAngle != collisions.slopeAngleOld) {
					distanceToSlope = hit.distance - skinWidth;
					velocity.x -= distanceToSlope * Direction;
				}

				ClimbSlope (ref velocity, slopeAngle);
				velocity.x += distanceToSlope * Direction;

			}

			if (!collisions.climbingASlope || slopeAngle > maxSlopeAngle) {
				velocity.x = (hit.distance - skinWidth) * Direction;
				rayLength = hit.distance;

				if (collisions.climbingASlope) {
					velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x);
				}

				collisions.left = Direction == -1;
				collisions.right = Direction == 1;
			}
		}
	}

	protected virtual void EnemyCollision(RaycastHit2D hit, ref Vector3 velocity, ref float Direction, ref float rayLength, bool isVertical) {

	}

	protected virtual void BuffCollision(RaycastHit2D hit, ref Vector3 velocity, ref float Direction, ref float rayLength, bool isVertical) {

	}

	protected virtual void PlayerCollision(RaycastHit2D hit, ref Vector3 velocity, ref float Direction, ref float rayLength, bool isVertical) {

	}

	protected override void DispatchVerticalCollision(RaycastHit2D hit, ref Vector3 velocity, ref float YDirection, ref float rayLength) {
		CollisionHandleDic [hit.transform.gameObject.layer] (hit, ref velocity, ref YDirection, ref rayLength, true);
	}

	protected override void DispatchHorizontalCollision(RaycastHit2D hit, ref Vector3 velocity, ref float XDirection, ref float rayLength, int i) {
		CollisionHandleDic [hit.transform.gameObject.layer] (hit, ref velocity, ref XDirection, ref rayLength, false);

	}

	protected override void VerticalCollision (ref Vector3 velocity) {
		float YDirection = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (YDirection == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * YDirection, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * YDirection * rayLength, Color.red);

			if (hit) {
				DispatchVerticalCollision (hit, ref velocity, ref YDirection, ref rayLength);
			}

		}
			
		if (collisions.climbingASlope) {
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D newHit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			Debug.DrawRay (rayOrigin, Vector2.right * directionX * rayLength, Color.blue);

			if (newHit) {
				float slopeAngle = Vector2.Angle(newHit.normal, Vector2.up);

				if (slopeAngle != collisions.slopeAngle) {
					velocity.x = (newHit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	protected virtual void ClimbSlope (ref Vector3 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocity = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocity) {
			velocity.y = climbVelocity;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisions.below = true;
			collisions.climbingASlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}


	protected virtual void DescendSlope(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign(hit.normal.x) == directionX) {
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingASlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}
}
