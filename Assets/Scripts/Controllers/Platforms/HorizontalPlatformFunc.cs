using UnityEngine;
using System.Collections;

public class HorizontalPlatformFunc : MonoBehaviour, IPlatformGuideFunction {
	
	public int maxX = 15;
	public int minX = -15;

	Vector3 movement = new Vector3(1, 0, 0);

	public Vector3 GetMovement() {
		if (transform.position.x < minX)
			movement.x = 1;
		if (transform.position.x > maxX)
			movement.x = -1;

		return movement;
	}
}
