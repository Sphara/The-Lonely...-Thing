using UnityEngine;
using System.Collections;

/// <summary>
/// A Shovel. Shovels stuff, using the controller attached to the object. I'll have to change it to be a bit more general & to work on all destructible objects, not only terrain
/// </summary>

[RequireComponent(typeof(Controller))]
[RequireComponent(typeof(Player))]
public class Shovel : MonoBehaviour {

	Controller controller;
	Player player;
	int axisToDig = 0; // 0 for horizontal dig

	public LayerMask layerToDig;

	void Start () {
		controller = GetComponent<Controller> ();
		player = GetComponent<Player> ();
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

		if (hit = controller.ManualRayCast (layerToDig, dir, axisToDig, 1.0f)) {

			MouseOver mo = hit.transform.GetComponent<MouseOver> ();
			if (mo)
				mo.DestroyTile ();
		}
	}
}
