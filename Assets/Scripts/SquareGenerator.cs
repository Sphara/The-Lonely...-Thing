using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquareGenerator : MonoBehaviour {

	GameObject tileCollection;
	Dictionary<TileType, GameObject> tilesToGO;
	Dictionary<Coord, DiffusionScript> _map;

	[Header("Tiles")]
	public GameObject DIRT;
	public GameObject GRASS;
	public GameObject GREYSAND;
	public GameObject GREYSTONE;
	public GameObject GREYSTONE_RUBY;
	public GameObject GREYSTONE_RUBY_ALT;
	public GameObject GREYSTONE_SAND;
	public GameObject DIRT_SAND;
	public GameObject SAND;
	public GameObject DIRT_SNOW;
	public GameObject SNOW;
	public GameObject STONE;
	public GameObject STONE_SNOW;
	public GameObject STONE_SAND;
	public GameObject STONE_COPPER;
	public GameObject STONE_COPPER_ALT;
	public GameObject STONE_IRON;
	public GameObject STONE_IRON_ALT;
	public GameObject STONE_GOLD;
	public GameObject STONE_GOLD_ALT;
	public GameObject STONE_COAL;
	public GameObject STONE_COAL_ALT;
	public GameObject STONE_DIAMOND;
	public GameObject STONE_DIAMOND_ALT;
	public GameObject STONE_SILVER;
	public GameObject STONE_SILVER_ALT;
	public GameObject STONE_GRASS;
	public GameObject STONE_DIRT;
	public GameObject LIMITS;
	public GameObject EMPTY;
	
	public void Initialize () {
		tileCollection = new GameObject ();
		tileCollection.name = "Tiles";

		_map = new Dictionary<Coord, DiffusionScript> ();
		tilesToGO = new Dictionary<TileType, GameObject> ();
		
		tilesToGO.Add (TileType.DIRT, DIRT);
		tilesToGO.Add (TileType.GRASS, GRASS);
		tilesToGO.Add (TileType.GREYSAND, GREYSAND);
		tilesToGO.Add (TileType.GREYSTONE, GREYSTONE);
		tilesToGO.Add (TileType.GREYSTONE_RUBY, GREYSTONE_RUBY);
		tilesToGO.Add (TileType.GREYSTONE_RUBY_ALT, GREYSTONE_RUBY_ALT);
		tilesToGO.Add (TileType.DIRT_SAND, DIRT_SAND);
		tilesToGO.Add (TileType.DIRT_SNOW, DIRT_SNOW);
		tilesToGO.Add (TileType.SAND, SAND);
		tilesToGO.Add (TileType.SNOW, SNOW);
		tilesToGO.Add (TileType.STONE, STONE);
		tilesToGO.Add (TileType.STONE_COAL, STONE_COAL);
		tilesToGO.Add (TileType.STONE_COAL_ALT, STONE_COAL_ALT);
		tilesToGO.Add (TileType.STONE_COPPER, STONE_COPPER);
		tilesToGO.Add (TileType.STONE_COPPER_ALT, STONE_COPPER_ALT);
		tilesToGO.Add (TileType.STONE_DIAMOND, STONE_DIAMOND);
		tilesToGO.Add (TileType.STONE_DIRT, STONE_DIRT);
		tilesToGO.Add (TileType.STONE_GOLD, STONE_GOLD);
		tilesToGO.Add (TileType.STONE_GOLD_ALT, STONE_GOLD_ALT);
		tilesToGO.Add (TileType.STONE_GRASS, STONE_GRASS);
		tilesToGO.Add (TileType.STONE_IRON, STONE_IRON);
		tilesToGO.Add (TileType.STONE_IRON_ALT, STONE_IRON_ALT);
		tilesToGO.Add (TileType.STONE_SAND, STONE_SAND);
		tilesToGO.Add (TileType.STONE_SILVER, STONE_SILVER);
		tilesToGO.Add (TileType.STONE_SILVER_ALT, STONE_SILVER_ALT);
		tilesToGO.Add (TileType.STONE_SNOW, STONE_SNOW);
		tilesToGO.Add (TileType.LIMITS, LIMITS);
		tilesToGO.Add (TileType.NONE, EMPTY);

	}

	GameObject getTile (TileType tile) {

		GameObject obj;
		tilesToGO.TryGetValue (tile, out obj);

		if (obj == null) {
			Debug.Log ("Was asked for tile " + tile + " and couldn't find it");
			return DIRT;
		}

		return obj;

	}

	public void GenerateSquares(TileType[,] map) {

		GameObject tile;

		tileCollection.transform.parent = this.transform;

		for (int i = 0; i < map.GetLength(0); i++) {
			for (int j = 0; j < map.GetLength(1); j++) {
			
				tile = (GameObject)Instantiate(getTile(map[i, j]), new Vector3(i, j, 0), Quaternion.identity);
				tile.name = "Tile[" + i + "][" + j + "]";
				tile.transform.parent = tileCollection.transform;

				if (map[i, j] == TileType.NONE)
					_map.Add(new Coord(i, j), tile.GetComponent<DiffusionScript>());

			}
		}

		foreach (KeyValuePair<Coord, DiffusionScript> kvp in _map) {
			AttachNeighbours(kvp.Key.x, kvp.Key.y);
		}

	}

	public void AddToMap(int x, int y) {
		DiffusionScript ds;
		Coord coord = new Coord (x, y);

		_map.TryGetValue (coord, out ds);
		
		if (ds)
			return;

		GameObject tile = (GameObject)Instantiate(getTile(TileType.NONE), new Vector3(x, y, 0), Quaternion.identity);
		tile.name = "Tile[" + x + "][" + y + "]";
		tile.transform.parent = tileCollection.transform;
		_map.Add (coord, tile.GetComponent<DiffusionScript>());
		AttachNeighbours (x, y);
		AttachNeighbours (x + 1, y);
		AttachNeighbours (x, y + 1);
		AttachNeighbours (x, y - 1);
		AttachNeighbours (x - 1, y);
	}

	public void AttachNeighbours(int x, int y) {
		DiffusionScript right;
		DiffusionScript left;
		DiffusionScript up;
		DiffusionScript down;

		DiffusionScript ds;

		_map.TryGetValue(new Coord(x, y), out ds);

		if (!ds)
			return;

		_map.TryGetValue(new Coord(x + 1, y), out right);
		_map.TryGetValue(new Coord(x - 1, y), out left);
		_map.TryGetValue(new Coord(x, y + 1), out up);
		_map.TryGetValue(new Coord(x, y - 1), out down);

		ds.neighbours.setNeighbouhrs(up, down, left, right);
	}

	public Vector2 LookForPlayer (int x, int y) {
		Coord coord = new Coord (x, y);
		DiffusionScript ds;
		
		_map.TryGetValue (coord, out ds);

		return ds ? ds.getDirection () : Vector2.zero;
	}

	public void DiffuseValue(int x, int y, int value) {
		Coord coord = new Coord (x, y);
		DiffusionScript ds;

		_map.TryGetValue (coord, out ds);

		if (ds)
			ds.DiffuseStuff (value, Vector2.zero);
	}

	public void deleteSquares() {
		Destroy (tileCollection);
		tileCollection = new GameObject ();
		tileCollection.name = "Tiles";
		_map.Clear ();
	}
	
}
