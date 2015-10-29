using UnityEngine;
using System.Collections;

public class DiffusionScript : MonoBehaviour {

	public Neighbours neighbours;
	float Diffusion = 0f;
	float TimeSinceDiffusion = 0f;

	void Update () {
		if (Diffusion > 0f)
			Diffusion -= Time.deltaTime * 3;
	}

	public void DiffuseStuff (float diffuseValue, Vector2 direction) {

		if (diffuseValue > Diffusion && diffuseValue > 0) {

			Diffusion = diffuseValue;

			if (neighbours._up && direction != Vector2.up)
				neighbours._up.DiffuseStuff(Diffusion - 1f, Vector2.down);
			if (neighbours._down && direction != Vector2.down)
				neighbours._down.DiffuseStuff(Diffusion - 1f, Vector2.up);
			if (neighbours._left && direction != Vector2.left)
				neighbours._left.DiffuseStuff(Diffusion - 1f, Vector2.right);
			if (neighbours._right && direction != Vector2.right)
				neighbours._right.DiffuseStuff(Diffusion - 1f, Vector2.left);

		}

	}

	public Vector2 getDirection() { // TO CHANGE BECAUSE ITS 9 AM AND I CAN SMELL SOUNDS
		int x = 0;
		int y = 0;
		float up = 0f;
		float down = 0f;
		float left = 0f;
		float right = 0f;
		float greater;

		if (neighbours._left)
			left = neighbours._left.Diffusion;
		if (neighbours._right)
			right = neighbours._right.Diffusion;
		if (neighbours._up)
			up = neighbours._up.Diffusion;
		if (neighbours._down)
			down = neighbours._down.Diffusion;

		greater = Mathf.Max (up, Mathf.Max(down, Mathf.Max(left, right)));

		Vector2 newv = new Vector2 (0,0);

		if (greater == 0)
			return Vector2.zero;
		if (greater == up)
			newv.y = 1f;
		if (greater == down)
			newv.y = -1f;
		if (greater == left)
			newv.x = -1f;
		if (greater == right)
			newv.x = 1f;

		return newv;

	}

	public struct Neighbours {

		public DiffusionScript _up, _down, _left, _right;
	
		public void setNeighbouhrs(DiffusionScript up, DiffusionScript down, DiffusionScript left, DiffusionScript right) {
			_up = up;
			_down = down;
			_left = left;
			_right = right;
		}
	
	}
}
