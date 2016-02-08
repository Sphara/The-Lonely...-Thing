using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class used to transform map into gameobjects
/// </summary>
/// 
public class SquareGenerator : MonoBehaviour {

	GameObject tileCollection;
	Dictionary<Coord, GameObject> tileMap;
	Dictionary<TileType, GameObject> tilesToGO;
	Dictionary<Coord, Diffusion> AIMap;
	Queue<MouseOver> linkables;

	[Header("Tiles")]
	public GameObject DIRT;
	public GameObject DIRT_GRAVEL;
	public GameObject GRASS;
	public GameObject GREYSAND;
	public GameObject GREYSTONE;
	public GameObject GREYSTONE_RUBY;
	public GameObject GREYSTONE_RUBY_ALT;
	public GameObject GREYSTONE_SAND;
	public GameObject REDSAND;
	public GameObject REDSTONE;
	public GameObject REDSTONE_EMERALD;
	public GameObject REDSTONE_EMERALD_ALT;
	public GameObject REDSTONE_SAND;
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
	public GameObject GROWNGRASS_1;
	public GameObject GROWNGRASS_2;
	public GameObject GROWNGRASS_3;
	public GameObject GROWNGRASS_4;

	void Start () {
		tileCollection = new GameObject ();
		tileCollection.name = "Tiles";

		AIMap = new Dictionary<Coord, Diffusion> ();
		tilesToGO = new Dictionary<TileType, GameObject> ();
		linkables = new Queue<MouseOver> ();
		tileMap = new Dictionary<Coord, GameObject> ();

		tilesToGO.Add (TileType.DIRT, DIRT);
		tilesToGO.Add (TileType.GRASS, GRASS);
		tilesToGO.Add (TileType.GREYSAND, GREYSAND);
		tilesToGO.Add (TileType.GREYSTONE, GREYSTONE);
		tilesToGO.Add (TileType.GREYSTONE_RUBY, GREYSTONE_RUBY);
		tilesToGO.Add (TileType.GREYSTONE_RUBY_ALT, GREYSTONE_RUBY_ALT);
		tilesToGO.Add (TileType.GREYSTONE_SAND, GREYSTONE_SAND);
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
		tilesToGO.Add (TileType.STONE_DIAMOND_ALT, STONE_DIAMOND_ALT);
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
		tilesToGO.Add (TileType.REDSAND, REDSAND);
		tilesToGO.Add (TileType.REDSTONE, REDSTONE);
		tilesToGO.Add (TileType.REDSTONE_EMERALD, REDSTONE_EMERALD);
		tilesToGO.Add (TileType.REDSTONE_EMERALD_ALT, REDSTONE_EMERALD_ALT);
		tilesToGO.Add (TileType.REDSTONE_SAND, REDSTONE_SAND);
		tilesToGO.Add (TileType.DIRT_GRAVEL, DIRT_GRAVEL);
		tilesToGO.Add (TileType.GROW_GRASS_1, GROWNGRASS_1);
		tilesToGO.Add (TileType.GROW_GRASS_2, GROWNGRASS_2);
		tilesToGO.Add (TileType.GROW_GRASS_3, GROWNGRASS_3);
		tilesToGO.Add (TileType.GROW_GRASS_4, GROWNGRASS_4);

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

	/// <summary>
	/// Generates the gameobjects.
	/// </summary>
	/// <param name="map">Map.</param>

	public void GenerateSquares(TileType[,] map) {

		GameObject tile;

		for (int i = 0; i < map.GetLength(0); i++) {
			for (int j = 0; j < map.GetLength(1); j++) {
			
				tile = (GameObject)Instantiate(getTile(map[i, j]), new Vector3(i, j, 0), Quaternion.identity);
				tile.name = "Tile[" + i + "][" + j + "]";
				tile.transform.parent = tileCollection.transform;
				tileMap.Add (new Coord(i, j), tile);

				Diffusion d = tile.GetComponent<Diffusion> ();
				if (d)
					AIMap.Add(new Coord(i, j), d);

				MouseOver m = tile.GetComponent<MouseOver> ();
				if (m && m.isLinkable) {
					linkables.Enqueue (m);
				}

			}
		}

		foreach (KeyValuePair<Coord, Diffusion> kvp in AIMap) {
			AttachNeighbours(kvp.Key.x, kvp.Key.y);
		}

		LinkLinkables ();
	}

	/// <summary>
	/// Links the linkables tiles to be able to destroy multiple codependant tiles in one dig.
	/// </summary>

	public void LinkLinkables () {

		while (linkables.Count != 0) {

			MouseOver m = linkables.Dequeue();
			MouseOver o = tileMap [new Coord ((int)m.transform.position.x + m.xLinked, (int)m.transform.position.y + m.yLinked)].GetComponent<MouseOver> ();

			if (o)
				o.LinkTile (m);
		}
	}

	/// <summary>
	/// Adds a tile of air and attach it to the pathfinding grid.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>

	public void AddToMap(int x, int y) {
		Diffusion ds;
		Coord coord = new Coord (x, y);

		AIMap.TryGetValue (coord, out ds);
		
		if (ds)
			return;

		GameObject tile = (GameObject)Instantiate(getTile(TileType.NONE), new Vector3(x, y, 0), Quaternion.identity);
		tile.name = "Tile[" + x + "][" + y + "]";
		tile.transform.parent = tileCollection.transform;
		AIMap.Add (coord, tile.GetComponent<Diffusion>());
		AttachNeighbours (x, y);
		AttachNeighbours (x + 1, y);
		AttachNeighbours (x, y + 1);
		AttachNeighbours (x, y - 1);
		AttachNeighbours (x - 1, y);
	}

	public void DeleteFromMap(int x, int y) {
		Coord c = new Coord (x, y);

		if (AIMap.ContainsKey (c))
			AIMap.Remove (c);
	}

	void AttachNeighbours(int x, int y) {
		Diffusion right;
		Diffusion left;
		Diffusion up;
		Diffusion down;

		Diffusion ds;

		AIMap.TryGetValue(new Coord(x, y), out ds);

		if (!ds)
			return;

		AIMap.TryGetValue(new Coord(x + 1, y), out right);
		AIMap.TryGetValue(new Coord(x - 1, y), out left);
		AIMap.TryGetValue(new Coord(x, y + 1), out up);
		AIMap.TryGetValue(new Coord(x, y - 1), out down);

		ds.neighbours.setNeighbouhrs(up, down, left, right);
	}

	/// <summary>
	/// Search for player from the X/Y tile.
	/// </summary>
	/// <returns>The for player.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Vector2 LookForPlayer (int x, int y) {
		Coord coord = new Coord (x, y);
		Diffusion ds;
		
		AIMap.TryGetValue (coord, out ds);

		return ds ? ds.getDirection () : Vector2.zero;
	}

	/// <summary>
	/// Diffuses the player position to help mobs finding it.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="value">Value.</param>

	public void DiffuseValue(int x, int y, int value) {
		Coord coord = new Coord (x, y);
		Diffusion ds;

		AIMap.TryGetValue (coord, out ds);

		if (ds)
			ds.DiffuseValue (value, Vector2.zero);
	}

	public void DisableSquare(int x, int y) {
		Coord coord = new Coord(x, y);
		Diffusion ds;

		AIMap.TryGetValue (coord, out ds);

		if (ds)
			ds.VoidSquare();
	}


	/// <summary>
	/// Reset the map
	/// </summary>
	public void deleteSquares() {
		Destroy (tileCollection);
		tileCollection = new GameObject ();
		tileCollection.name = "Tiles";
		AIMap.Clear ();
	}
	
}
