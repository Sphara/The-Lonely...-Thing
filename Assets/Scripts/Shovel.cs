using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller))]
[RequireComponent(typeof(Player))]
public class Shovel : MonoBehaviour {

	Controller controller;
	Player player;
	int axisToDig = 0; // 0 for horizontal dig
	int layer;

	void Start () {
		controller = GetComponent<Controller> ();
		player = GetComponent<Player> ();
		layer = 1 << 9;
	}

	public void dig () {
		float dir = Input.GetAxisRaw ("Horizontal");
		RaycastHit2D hit;

		axisToDig = 0;

		if (dir == 0) {
			axisToDig = 1;
			dir = Input.GetAxisRaw ("Vertical");

			if (dir == 0) {
				axisToDig = 0;
				dir = -player.faceDirection;
			}
		}

		if (hit = controller.ManualRayCast (layer, dir, axisToDig, 1.0f)) {
			if (hit.transform.gameObject.layer == LayerMask.NameToLayer("CollisionLayer"))
				hit.transform.GetComponent<MouseOver> ().DestroyTile ();
		}
	}
}
