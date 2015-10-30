using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum TileType {
	NONE = 0,
	DIRT = 1,
	GRASS = 2,
	GREYSAND = 3,
	GREYSTONE = 4,
	GREYSTONE_RUBY = 5,
	GREYSTONE_RUBY_ALT = 6,
	GREYSTONE_SAND = 7,
	DIRT_SAND = 8,
	SAND = 9,
	DIRT_SNOW = 10,
	SNOW = 11,
	STONE = 12,
	STONE_SNOW = 13,
	STONE_SAND = 14,
	STONE_COPPER = 15,
	STONE_COPPER_ALT = 16,
	STONE_IRON = 17,
	STONE_IRON_ALT = 18,
	STONE_GOLD = 19,
	STONE_GOLD_ALT = 20,
	STONE_COAL = 21,
	STONE_COAL_ALT = 22,
	STONE_DIAMOND = 23,
	STONE_DIAMOND_ALT = 24,
	STONE_SILVER = 25,
	STONE_SILVER_ALT = 26,
	STONE_GRASS = 27,
	STONE_DIRT = 28,
	LIMITS = 29
};

enum BiomeType {
	NONE = 0,
	STONE = 1,
	SNOW = 2,
	GREYSTONE = 3,
	SAND = 4,
	DEFAULT = 5
};

/// <summary>
/// The procedural map generator. 
/// 
/// There's some basic biome generation (floodfill for now, to change) & cave generation via cellular algorithms
/// 
/// TODO: Add ore generation
/// TODO: Change Biome generation
/// TODO: Change Cave generation to have a more "real" world
/// TODO: "Fancify" the map
/// 
/// Yeah, i kinda want to change everything
/// 
/// Creates a tile per unit of terrain, not a big mesh. I'm not using Tiled2Unity because i'm gonna wait for 5.4 and native integration of tilemaps (ETA march 2016 iirc)
/// </summary>

public class MapGenerator : MonoBehaviour {

	[Header("Map Attributes")]
	public int width;
	public int height;
	public string seed;
	public bool useRandomSeed;
	public int borderSize;
	public int smoothingFactor = 2;

	[Range(0,100)]
	public int randomFillPercent;
	private int SEED_SIZE = 32;
	private const string seedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz123456789";
	TileType[,] map;
	TileType[,] borderedMap;
	BiomeType[,] biomesMap;
	
	SquareGenerator squareGenerator;
	Teleporter teleporter;
	
	void Start() {
		squareGenerator = GetComponent<SquareGenerator> ();
		squareGenerator.Initialize ();
		teleporter = GameObject.Find("Teleporter").GetComponent<Teleporter> ();
		GenerateMap();
		teleporter.TeleportPlayerInSquareMap (GameObject.Find("Player"));
		GameObject.Find ("MobSpawner").GetComponent<MobSpawner> ().setSpawn (true);
	}
	
	void Update() {

		if (Input.GetKeyDown (KeyCode.R)) {
			GenerateMap ();
			teleporter.TeleportPlayerInSquareMap (GameObject.Find ("Player"));
		}
	}
	
	void GenerateMap() {
		map = new TileType[width, height];
		borderedMap = new TileType[width + borderSize * 2, height + borderSize * 2];
		
		GenerateBiomes ();
		RandomFillMap();

		for (int i = 0; i < smoothingFactor; i++) {
			SmoothMap ();
		}

		ApplyBiomes ();
		fancifyMap ();
		updateBorderedMap ();
		generateSquares();
	}

	void fancifyMap () {
		setSurface ();
		/* I have to put some more stuff here */
	}

	void RandomFillMap() {

		if (useRandomSeed) {
			seed = RandomString (SEED_SIZE);
		}
		
		System.Random pseudoRandom = new System.Random(seed.GetHashCode());
		
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (x == 0 || x == width-1 || y == 0 || y == height -1) {
					map[x,y] = TileType.DIRT;
				}
				else {
					map[x,y] = (pseudoRandom.Next(0,100) < randomFillPercent)? TileType.DIRT : TileType.NONE;
				}
			}
		}
	}
	
	void SmoothMap() {
		
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighbourWallTiles = GetSurroundingWallCount(x,y);
				
				if (neighbourWallTiles > 4)
					map[x,y] = TileType.DIRT;
				else if (neighbourWallTiles < 4)
					map[x,y] = TileType.NONE;		
			}
		}
		
		updateBorderedMap ();
	}

	void GenerateBiomes () {
		biomesMap = new BiomeType[width, height];
		Queue<Coord> queue = new Queue<Coord> ();
		BiomeType actualType;

		for (int i = 0; i < (int)((height * width) / 500); i++) { // I'll have to find out a better way to create biomes
			int x = UnityEngine.Random.Range (1, map.GetLength(0) - 1);
			int y = UnityEngine.Random.Range (1, map.GetLength(1) - 1);

			biomesMap[x, y] = getRandomBiomeType();
			queue.Enqueue(new Coord(x, y));
		}

		while (queue.Count != 0) {
			Coord dequeued = queue.Dequeue();
			actualType = biomesMap[dequeued.x, dequeued.y];

			if (dequeued.x < width - 1 && biomesMap[dequeued.x + 1, dequeued.y] == BiomeType.NONE) {
				biomesMap[dequeued.x + 1, dequeued.y] = actualType;
				queue.Enqueue(new Coord(dequeued.x + 1, dequeued.y));
			}

			if (dequeued.x > 0 && biomesMap[dequeued.x - 1, dequeued.y] == BiomeType.NONE) {
				biomesMap[dequeued.x - 1, dequeued.y] = actualType;
				queue.Enqueue(new Coord(dequeued.x - 1, dequeued.y));
			}

			if (dequeued.y > 0 && biomesMap[dequeued.x, dequeued.y - 1] == BiomeType.NONE) {
				biomesMap[dequeued.x, dequeued.y - 1] = actualType;
				queue.Enqueue(new Coord(dequeued.x, dequeued.y - 1));
			}

			if (dequeued.y < height - 1 && biomesMap[dequeued.x, dequeued.y + 1] == BiomeType.NONE) {
				biomesMap[dequeued.x, dequeued.y + 1] = actualType;
				queue.Enqueue(new Coord(dequeued.x, dequeued.y + 1));
			}
		}
	}

	void ApplyBiomes () {

		for (int x = 0; x < map.GetLength(0); x ++) {
			for (int y = 0; y < map.GetLength(1); y ++) {

				if (map[x, y] == TileType.DIRT) {

					switch (biomesMap[x, y])
					{
					case BiomeType.GREYSTONE:
						map[x, y] = TileType.GREYSTONE;
						break;
					
					case BiomeType.SAND:
						map[x, y] = TileType.STONE;
						break;
					
					case BiomeType.SNOW:
						map[x, y] = TileType.STONE;
						break;

					case BiomeType.STONE:
						map[x, y] = TileType.STONE;
						break;
					}
				}

			}
		}

	}

	void setSurface () {

		Queue<Coord> queue = new Queue<Coord> ();

		for (int x = 0; x < map.GetLength(0); x ++) {
			for (int y = 0; y < map.GetLength(1); y ++) {
				
				if (IsInMapRange (x, y + 1)) {
					if (map [x, y] == TileType.DIRT && map [x, y + 1] == TileType.NONE) {
						map [x, y] = TileType.GRASS;
					}

					if (map [x, y] == TileType.STONE && map [x, y + 1] == TileType.NONE) {
						if (biomesMap [x, y] == BiomeType.SAND)
							map [x, y] = TileType.STONE_SAND;
						else if (biomesMap [x, y] == BiomeType.SNOW)
							map [x, y] = TileType.STONE_SNOW;
						else
							map [x, y] = TileType.STONE_DIRT;

						queue.Enqueue (new Coord (x, y));
					}
				}
			}
		}

		while (queue.Count != 0) {
			Coord tile = queue.Dequeue();
			BiomeType tileBiome = biomesMap[tile.x, tile.y];
			TileType tileToAdd;

			if (tileBiome == BiomeType.SAND)
				tileToAdd = TileType.SAND;
			else if (tileBiome == BiomeType.SNOW)
				tileToAdd = TileType.SNOW;
			else
				tileToAdd = TileType.DIRT;

			if (tile.y + 1 < height - 1 && map[tile.x, tile.y + 1] == TileType.NONE) {
				map[tile.x, tile.y + 1] = tileToAdd;

				if (tileToAdd == TileType.DIRT && tile.y + 2 < height - 1 && map[tile.x, tile.y + 2] == TileType.NONE) {
					map[tile.x, tile.y + 1] = TileType.GRASS;
				}
			}


		}
		
	}
	
	void generateSquares() {
		squareGenerator.deleteSquares ();
		squareGenerator.GenerateSquares (borderedMap);
	}
	
	void updateBorderedMap ()	{
		borderedMap = new TileType[width + borderSize * 2,height + borderSize * 2];

		for (int x = 0; x < borderedMap.GetLength(0); x ++) {
			for (int y = 0; y < borderedMap.GetLength(1); y ++) {
				if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize) {
					borderedMap[x,y] = map[x-borderSize,y-borderSize];
				}
				else {
					borderedMap[x,y] = TileType.LIMITS;
				}
			}
		}

		teleporter.setMap (borderedMap);
	}

	bool IsInMapRange(int x, int y) {
		return x >= 0 && x < width && y >= 0 && y < height;
	}

	private string RandomString(int size)
	{
		char[] buffer = new char[size];
		
		for (int i = 0; i < size; i++)
		{
			buffer[i] = seedCharacters[(int)UnityEngine.Random.Range(0, seedCharacters.Length)];
		}

		return new string(buffer);
	}
	
	int GetSurroundingWallCount(int gridX, int gridY) {
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
				if (IsInMapRange(neighbourX,neighbourY)) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += map[neighbourX,neighbourY];
					}
				}
				else {
					wallCount ++;
				}
			}
		}
		
		return wallCount;
	}

	BiomeType getRandomBiomeType () {
		Array biomesArray = Enum.GetValues (typeof(BiomeType));
		return ((BiomeType)biomesArray.GetValue(UnityEngine.Random.Range (1, biomesArray.Length)));
	}

}