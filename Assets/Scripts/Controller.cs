using UnityEngine;
using System.Collections;

/// <summary>
///  Controller made by Sebastian Lague (https://www.youtube.com/watch?v=MbWK8bCAU2w) tweaked a bit
/// </summary>

[RequireComponent(typeof(BoxCollider2D))]
public class Controller : MonoBehaviour {

	BoxCollider2D boxCollider;

	public LayerMask collisionMask;

	const float skinWidth = 0.002f;
	public int verticalRayCount = 4;
	public int horizontalRayCount = 4;

	float maxSlopeAngle = 70;

	float horizontalRaySpacing;
	float verticalRaySpacing;

	RaycastOrigins raycastOrigins;
	public CollisionsInfo collisions;
	
	void Start() {
		boxCollider = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
		
	}

	/// <summary>
	/// Move the object, checking for collisions with raycasts.
	/// Handles only ascending slopes for now (gonna implement descending slopes/double jump/wall jump & other stuff later)
	/// </summary>
	/// <param name="velocity">Actual velocity of the object</param>

	public void Move (Vector3 velocity) {
		UpdateRaycastOrigins ();
		collisions.Reset ();

		if (velocity.x != 0)
			HorizontalCollision (ref velocity);

		if (velocity.y != 0)
			VerticalCollision (ref velocity);

		transform.Translate (velocity);
	}
	
	void VerticalCollision (ref Vector3 velocity) {
		float YDirection = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (YDirection == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * YDirection, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * YDirection * rayLength, Color.red);

			if (hit) {
				velocity.y = (hit.distance - skinWidth) * YDirection;
				rayLength = hit.distance;
			
				if (collisions.climbingASlope) {
					velocity.x = velocity.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}

				collisions.above = YDirection == 1;
				collisions.below = YDirection == -1;
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
	
	void HorizontalCollision (ref Vector3 velocity) {
		float XDirection = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;
		
		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (XDirection == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * XDirection, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * XDirection * rayLength, Color.red);

			if (hit) {

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
			}
			
		}
	}
	
	void ClimbSlope (ref Vector3 velocity, float slopeAngle) {
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
	
	void UpdateRaycastOrigins() {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (skinWidth * -2);
	
		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);

	}
	
	void CalculateRaySpacing () {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);
	
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
	
	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	};
}
