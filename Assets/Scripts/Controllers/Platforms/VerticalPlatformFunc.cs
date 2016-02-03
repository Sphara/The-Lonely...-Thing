using UnityEngine;
using System.Collections;

public class VerticalPlatformFunc : MonoBehaviour, IPlatformGuideFunction {

	public int maxY = 15;
	public int minY = 0;

	Vector3 movement = new Vector3(0, 1, 0);

	public Vector3 GetMovement() {
		if (transform.position.y < minY)
			movement.y = 1;
		if (transform.position.y > maxY)
			movement.y = -1;

		return movement;
	}

}
