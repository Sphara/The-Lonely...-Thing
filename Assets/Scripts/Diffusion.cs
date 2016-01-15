﻿using UnityEngine;
using System.Collections;

/// <summary>
/// The diffusion script used by the non-terrain tiles.
/// 
/// TODO: replace float values by some shorter thing representing the direction of the player and observe performance impact
/// TODO: replace the value by a dictionary of values to allow multiple tracking (watch for performance drop)
/// </summary>

public class Diffusion : MonoBehaviour {

	public Neighbours neighbours;
	float tileDiffusionValue = 0f;

	void Update () {
		if (tileDiffusionValue > 0f)
			tileDiffusionValue -= Time.deltaTime * 300;
	}
	
	/// <summary>
	/// Diffuses the value on the grid. Call this on the actual tile from items that are to be chased by mobs
	/// </summary>
	/// <param name="valueDiffused">Value diffused.</param>
	/// <param name="direction">Direction.</param>

	public void DiffuseValue (float valueDiffused, Vector2 direction) {

		if (valueDiffused > tileDiffusionValue && valueDiffused > 0) {

			tileDiffusionValue = valueDiffused;

			if (neighbours._up && direction != Vector2.up)
				neighbours._up.DiffuseValue(tileDiffusionValue - 1f, Vector2.down);
			if (neighbours._down && direction != Vector2.down)
				neighbours._down.DiffuseValue(tileDiffusionValue - 1f, Vector2.up);
			if (neighbours._left && direction != Vector2.left)
				neighbours._left.DiffuseValue(tileDiffusionValue - 1f, Vector2.right);
			if (neighbours._right && direction != Vector2.right)
				neighbours._right.DiffuseValue(tileDiffusionValue - 1f, Vector2.left);

		}

	}

	/// <summary>
	/// Gets the direction.
	/// TODO: add a parameter to get the direction to a particular entity
	/// </summary>

	public Vector2 getDirection() {
		float up = 0f;
		float down = 0f;
		float left = 0f;
		float right = 0f;
		float greater;
		Vector2 newv = new Vector2 (0,0);

		if (neighbours._left)
			left = neighbours._left.tileDiffusionValue;
		if (neighbours._right)
			right = neighbours._right.tileDiffusionValue;
		if (neighbours._up)
			up = neighbours._up.tileDiffusionValue;
		if (neighbours._down)
			down = neighbours._down.tileDiffusionValue;

		greater = Mathf.Max (up, Mathf.Max(down, Mathf.Max(left, right)));

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

		public Diffusion _up, _down, _left, _right;
	
		public void setNeighbouhrs(Diffusion up, Diffusion down, Diffusion left, Diffusion right) {
			_up = up;
			_down = down;
			_left = left;
			_right = right;
		}
	
	}
}
