using UnityEngine;
using System.Collections;

public class CirclePlatformFunc : MonoBehaviour, IPlatformGuideFunction {

	public Vector3 center = new Vector3();
	public float rad = 1f;

	Vector3 movement = new Vector3();
	float time = 0f;

	public Vector3 GetMovement() {

		time = time + (Time.deltaTime);

		movement.x = (center.x + Mathf.Sin(time) * rad) - transform.position.x;
		movement.y = (center.y + Mathf.Cos(time) * rad) - transform.position.y;

		return movement;
	}
}
