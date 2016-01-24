using UnityEngine;
using System.Collections;

public class GeneralMobController : CollisionController {

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
		case 8: // P-layer
			hit.transform.gameObject.GetComponent<Player>().stats.TakeContactHit(this.GetComponent<TestMob>().stats);
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
		case 8: // P-layer
			hit.transform.gameObject.GetComponent<Player>().stats.TakeContactHit(this.GetComponent<TestMob>().stats);
			break;
		default:
			Debug.Log (this.name + " collided with " + hit.transform.gameObject.name + " from layer " + hit.transform.gameObject.layer + " and failed to react");
			break;
		}

	}
}
