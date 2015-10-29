using UnityEngine;
using System.Collections;

public class MouseOverScript : MonoBehaviour {

	void OnMouseDown () {
		SquareGenerator sg = GameObject.Find ("MapGenerator").GetComponent<SquareGenerator>();
		sg.AddToMap ((int)transform.position.x, (int)transform.position.y);
		this.gameObject.SetActive(false);
	}
}
