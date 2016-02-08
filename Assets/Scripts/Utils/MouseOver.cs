using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This is only a "temporary" fix while the terrain is made of tiles. Obviously when i switch to a mesh i'll have to change that
/// 
/// Makes terrain disappar
/// </summary>

public class MouseOver : MonoBehaviour {

	public bool isLinkable;
	public int xLinked;
	public int yLinked;
	List<MouseOver> linkedTiles = new List<MouseOver> ();

	void OnMouseDown () {
		DestroyTile ();
	}

	public void LinkTile (MouseOver m) {
		linkedTiles.Add (m);
	}

	public void DestroyTile() {
		if (transform.gameObject.activeSelf) {
			SquareGenerator sg = GameObject.Find ("MapGenerator").GetComponent<SquareGenerator> ();
			Diffusion d = GetComponent<Diffusion> ();

			if (d)
				sg.DeleteFromMap ((int)transform.position.x, (int)transform.position.y);

			sg.AddToMap ((int)transform.position.x, (int)transform.position.y);
			this.gameObject.SetActive (false);

			foreach (MouseOver m in linkedTiles) {
				m.DestroyTile ();
			}
		}
	}
}