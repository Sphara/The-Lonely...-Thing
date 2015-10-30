using UnityEngine;
using System.Collections;

/// <summary>
/// This is only a "temporary" fix while the terrain is made of tiles. Obviously when i switch to a mesh i'll have to change that
/// 
/// Makes terrain disappar
/// </summary>

public class MouseOver : MonoBehaviour {

	void OnMouseDown () {
		SquareGenerator sg = GameObject.Find ("MapGenerator").GetComponent<SquareGenerator>();
		sg.AddToMap ((int)transform.position.x, (int)transform.position.y);
		this.gameObject.SetActive(false);
	}
}
