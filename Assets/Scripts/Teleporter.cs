using UnityEngine;
using System.Collections;

/// <summary>
/// TODO: Teleport to ground level, setup teleport points (home & stuff)
/// </summary>
public class Teleporter : MonoBehaviour {

	TileType[,] _map;

	void Start () {
			
	}

	public void setMap (TileType[,] map) {
		_map = map;
	}

	public void TeleportPlayerInSquareMap (GameObject player) {
		int x = UnityEngine.Random.Range (0, _map.GetLength(0));
		int y = UnityEngine.Random.Range (0, _map.GetLength (1));

		while (_map[x,y] != TileType.NONE) {
			x = UnityEngine.Random.Range (0, _map.GetLength(0));
			y = UnityEngine.Random.Range (0, _map.GetLength (1));
		}

		Vector3 pos = new Vector3 (x, y, 0);

		player.transform.position = pos;
	}

}
