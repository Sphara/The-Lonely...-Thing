using UnityEngine;
using System.Collections;

public class CollisionController : Controller {

	protected float maxSlopeAngle = 70;

	protected override void DispatchVerticalCollision(RaycastHit2D hit, ref Vector3 velocity, ref float YDirection, ref float rayLength) {

		switch (hit.transform.gameObject.layer) { // I don't like switches
		case 9: // CollisionLayer
			velocity.y = (hit.distance - skinWidth) * YDirection;
			rayLength = hit.distance;

			if (collisions.climbingASlope) {
				velocity.x = velocity.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
			}

			collisions.above = YDirection == 1;
			collisions.below = YDirection == -1;
			break;
		case 10:
			break;
		default:
			Debug.Log (this.name + " collided with " + hit.transform.gameObject.name + " from layer " + hit.transform.gameObject.layer + " and failed to react");
			break;
		}

	}

	protected override void DispatchHorizontalCollision(RaycastHit2D hit, ref Vector3 velocity, ref float XDirection, ref float rayLength, int i) {

		switch (hit.transform.gameObject.layer) { // I don't like switches
		case 9: // CollisionLayer
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

			if (i == 0 && slopeAngle <= maxSlopeAngle) {
				float distanceToSlope = 0;

				if (slopeAngle != collisions.slopeAngleOld) {
					distanceToSlope = hit.distance - skinWidth;
					velocity.x -= distanceToSlope * XDirection;
				}

				ClimbSlope (ref velocity, slopeAngle);
				velocity.x += distanceToSlope * XDirection;

			}

			if (!collisions.climbingASlope || slopeAngle > maxSlopeAngle) {
				velocity.x = (hit.distance - skinWidth) * XDirection;
				rayLength = hit.distance;

				if (collisions.climbingASlope) {
					velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x);
				}

				collisions.left = XDirection == -1;
				collisions.right = XDirection == 1;
			}
			break;
		case 10:
			break;
		default:
			Debug.Log (this.name + " collided with " + hit.transform.gameObject.name + " from layer " + hit.transform.gameObject.layer + " and failed to react");
			break;
		}

	}

	protected override void VerticalCollision (ref Vector3 velocity) {
		float YDirection = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (YDirection == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * YDirection, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * YDirection * rayLength, Color.red);

			if (hit) {
				DispatchVerticalCollision (hit, ref velocity, ref YDirection, ref rayLength);
			}

		}

		if (collisions.climbingASlope) {
			float XDirection = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((XDirection == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * XDirection, rayLength, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (slopeAngle != collisions.slopeAngle) {
					velocity.x = (hit.distance - skinWidth) * XDirection;
					collisions.slopeAngle = slopeAngle;
				}

			}
		}

	}

	protected override void HorizontalCollision (ref Vector3 velocity) {
		float XDirection = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (XDirection == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * XDirection, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * XDirection * rayLength, Color.red);

			if (hit)
				DispatchHorizontalCollision (hit, ref velocity, ref XDirection, ref rayLength, i);

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
}
