using UnityEngine;
using System.Collections;

/// <summary>
///  Controller originally made by Sebastian Lague (https://www.youtube.com/watch?v=MbWK8bCAU2w) tweaked a bit
/// 
/// It's a base for all moving objects
/// </summary>

[RequireComponent(typeof(BoxCollider2D))]
public class Controller : MonoBehaviour {

	public delegate void CollisionEventFunction (RaycastHit2D hit, ref Vector3 velocity, ref float Direction, ref float length, bool isVertical);

	protected BoxCollider2D boxCollider;
	public LayerMask collisionMask;

	protected const float skinWidth = 0.002f;
	public int verticalRayCount = 4;
	public int horizontalRayCount = 4;

	protected float horizontalRaySpacing;
	protected float verticalRaySpacing;

	public RaycastOrigins raycastOrigins;
	public CollisionsInfo collisions;

	protected int i;

	public virtual void Start() {
		boxCollider = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
	}

	public virtual void Move (Vector3 velocity, bool standingOnPlatform = false) {
		UpdateRaycastOrigins ();
		collisions.Reset ();

		if (velocity.x != 0)
			HorizontalCollision (ref velocity);

		if (velocity.y != 0)
			VerticalCollision (ref velocity);

		transform.Translate (velocity);
	}

	protected virtual void DispatchVerticalCollision(RaycastHit2D hit, ref Vector3 velocity, ref float YDirection, ref float rayLength) {
		Debug.Log (this.name + " collided with " + hit.transform.gameObject.name + " from layer " + hit.transform.gameObject.layer + " and failed to react");
	}
	
	protected virtual void VerticalCollision (ref Vector3 velocity) {
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

	}

	protected virtual void DispatchHorizontalCollision(RaycastHit2D hit, ref Vector3 velocity, ref float XDirection, ref float rayLength, int i) {
		Debug.Log (this.name + " collided with " + hit.transform.gameObject.name + " from layer " + hit.transform.gameObject.layer + " and failed to react");
	}

	protected virtual void HorizontalCollision (ref Vector3 velocity) {
		float XDirection = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;
		
		for (i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (XDirection == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * XDirection, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * XDirection * rayLength, Color.red);

			if (hit) {

				if (hit.distance == 0) {
					continue;
				}

				DispatchHorizontalCollision (hit, ref velocity, ref XDirection, ref rayLength, i);
			}
		}
	}

	/// <summary>
	/// Do a manual raycast from the controller attached to this gameobject
	/// </summary>
	/// <returns>The raycast hit, if any.</returns>
	/// <param name="hitLayer">The layer you want to hit.</param>
	/// <param name="direction">The direction you want to cast your ray : 1/-1</param>
	/// <param name="axis">The axis you want to cast on : 0 horizontal, 1 vertical</param>
	/// <param name="length">Length.</param>
	public RaycastHit2D ManualRayCast(LayerMask hitLayer, float direction, int axis, float length) {
		if (axis == 0)
			return ManualHorizontalRayCast (hitLayer, direction, length);
		else
			return ManualVerticalRayCast (hitLayer, direction, length);
	}

	private RaycastHit2D ManualHorizontalRayCast(LayerMask hitLayer, float direction, float length) {

		RaycastHit2D hit = new RaycastHit2D();

		for (i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (direction == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);

			hit = Physics2D.Raycast(rayOrigin, Vector2.right * direction, length, hitLayer);

			Debug.DrawRay(rayOrigin, Vector2.right * direction * length, Color.green);

			if (hit)
				return (hit);
		}

		return hit;
	}

	private RaycastHit2D ManualVerticalRayCast(LayerMask hitLayer, float direction, float length) {

		RaycastHit2D hit = new RaycastHit2D();

		for (i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (direction == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i);

			hit = Physics2D.Raycast(rayOrigin, Vector2.up * direction, length, hitLayer);

			Debug.DrawRay(rayOrigin, Vector2.up * direction * length, Color.green);

			if (hit) {
				return (hit);
			}
		}

		return hit;
	}

	protected void UpdateRaycastOrigins() {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (skinWidth * -2);
	
		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);

	}
	
	protected void CalculateRaySpacing () {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);
	
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}
	
	public struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	};
}
