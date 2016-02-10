using UnityEngine;
using System.Collections;

public class AITestMapGenerator : MonoBehaviour {

	public SquareGenerator sg;
	TileType[,] map;

	void Start () {

		map = new TileType[30, 30];

		for (int i = 0; i < 30; i++) {
			for (int j = 0; j < 30; j++) {

				if (i == 0 || j == 0 || i == 29 || j == 29) {
					map [i, j] = TileType.LIMITS;
				} else if (j > 15 || (j > 13 && j < 15 && i > 13 && i < 17)) {
					map [i, j] = TileType.NONE;
				} else {
					map [i, j] = TileType.DIRT;
				}

			}
		}

		sg.GenerateSquares (map);
	}

}
