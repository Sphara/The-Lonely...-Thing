using UnityEngine;
using System.Collections;

public class MouseOver : MonoBehaviour {

	void OnMouseDown () {
		SquareGenerator sg = GameObject.Find ("MapGenerator").GetComponent<SquareGenerator>();
		sg.AddToMap ((int)transform.position.x, (int)transform.position.y);
		this.gameObject.SetActive(false);
	}
}
