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
	LIMITS = 29,
	REDSTONE = 30,
	REDSTONE_SAND = 31,
	REDSTONE_EMERALD = 32,
	REDSTONE_EMERALD_ALT = 33,
	REDSAND = 34,
	DIRT_GRAVEL = 35,
	GROW_GRASS_1 = 36,
	GROW_GRASS_2 = 37,
	GROW_GRASS_3 = 38,
	GROW_GRASS_4 = 39
};

public enum BiomeType {
	NONE = 0,
	STONE = 1,
	SNOW = 2,
	GREYSTONE = 3,
	SAND = 4,
	DEFAULT = 5,
	REDSTONE = 6
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
/// TODO: Replace random generation of surface with some noise, prob. perlin
/// 
/// Yeah, i want to change everything
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

	[Range(0, 10)]
	public int smoothingFactor = 2;
	[Range(0,100)]
	public int randomFillPercent;
	public int groundLevel = 50;
	public int crustHeight = 5;

	private int SEED_SIZE = 32;
	private const string seedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz123456789";
	TileType[,] map;
	TileType[,] borderedMap;
	BiomeType[,] biomesMap;
	
	SquareGenerator squareGenerator;
	public Teleporter teleporter;
	public MobSpawner mobSpawner;
	public GameObject player;
	public MineralFarm mineralFarm;

	List<TileType> grassList = new List<TileType> ();
	
	void Start() {
		squareGenerator = GetComponent<SquareGenerator> ();
		grassList.Add (TileType.GROW_GRASS_1);
		grassList.Add (TileType.GROW_GRASS_2);
		grassList.Add (TileType.GROW_GRASS_3);
		grassList.Add (TileType.GROW_GRASS_4);

		GenerateMap();
		teleporter.TeleportPlayerInSquareMap (player);
		mobSpawner.setSpawn (true);
	}
	
	void Update() {

		if (Input.GetKeyDown (KeyCode.R)) {
			GenerateMap ();
			teleporter.TeleportPlayerInSquareMap (player);
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

		FancifyMap ();

		UpdateBorderedMap ();
		GenerateSquares();
	}

	void FancifyMap () {

		GrowMinerals ();
		ApplyBiomes ();
		SetSurface ();
	}

	void GrowMinerals() {
		mineralFarm.GrowAll (map, biomesMap);
	}

	void RandomFillMap() {

		if (useRandomSeed) {
			seed = RandomString (SEED_SIZE);
		}

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		NoiseGenerator gen = new NoiseGenerator ();

		for (int x = 0; x < width; x ++) {

			int groundHeight = (int)gen.PerlinNoise (x, 0, 80, 15, 1);
			groundHeight += (int)gen.PerlinNoise (x, 0, 50, 30, 1);
			groundHeight += groundLevel;

			for (int y = 0; y < height; y ++) {

				if (y < (groundHeight - crustHeight)) {
					map[x,y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? TileType.DIRT : TileType.NONE;
				} else if (y < groundHeight) {
					map[x, y] = TileType.DIRT;
				} else {
					map[x,y] = TileType.NONE;
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
		
		UpdateBorderedMap ();
	}

	/// <summary>
	/// Generate biomes.
	/// 
	/// At first i was using floodfill but the biomes were unnatural and ugly, so i modified it a bit and even though it's not perfect it's waaaaay better
	/// </summary>

	void GenerateBiomes () {
		biomesMap = new BiomeType[width, height];
		List<Coord> tileList = new List<Coord> ();
		BiomeType actualType;

		for (int i = 0; i < (int)((height * width) / 500); i++) {
			int x = UnityEngine.Random.Range (1, map.GetLength(0) - 1);
			int y = UnityEngine.Random.Range (1, map.GetLength(1) - 1);

			biomesMap[x, y] = GetBiomeType(x, y);
			tileList.Add(new Coord(x, y));
		}

		while (tileList.Count != 0) {
			Coord dequeued = tileList[0];
			tileList.RemoveAt(0);
			actualType = biomesMap[dequeued.x, dequeued.y];

			if (dequeued.x < width - 1 && biomesMap[dequeued.x + 1, dequeued.y] == BiomeType.NONE) {
				biomesMap[dequeued.x + 1, dequeued.y] = actualType;
				tileList.Insert(UnityEngine.Random.Range(0, tileList.Count), new Coord(dequeued.x + 1, dequeued.y));
			}

			if (dequeued.x > 0 && biomesMap[dequeued.x - 1, dequeued.y] == BiomeType.NONE) {
				biomesMap[dequeued.x - 1, dequeued.y] = actualType;
				tileList.Insert(UnityEngine.Random.Range(0, tileList.Count), new Coord(dequeued.x - 1, dequeued.y));
			}

			if (dequeued.y > 0 && biomesMap[dequeued.x, dequeued.y - 1] == BiomeType.NONE) {
				biomesMap[dequeued.x, dequeued.y - 1] = actualType;
				tileList.Insert(UnityEngine.Random.Range(0, tileList.Count), new Coord(dequeued.x, dequeued.y - 1));
			}

			if (dequeued.y < height - 1 && biomesMap[dequeued.x, dequeued.y + 1] == BiomeType.NONE) {
				biomesMap[dequeued.x, dequeued.y + 1] = actualType;
				tileList.Insert(UnityEngine.Random.Range(0, tileList.Count), new Coord(dequeued.x, dequeued.y + 1));
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

					case BiomeType.REDSTONE:
						map[x, y] = TileType.REDSTONE;
						break;
					}
				}

			}
		}

	}

	void SetSurface () {

		Queue<Coord> stoneQueue = new Queue<Coord> ();
		Queue<Coord> grassQueue = new Queue<Coord> ();

		for (int x = 0; x < map.GetLength(0); x ++) {
			for (int y = 0; y < map.GetLength(1); y ++) {
				
				if (IsInMapRange (x, y + 1)) {

					if (map [x, y] == TileType.DIRT && map [x, y + 1] == TileType.NONE) {
						map [x, y] = TileType.GRASS;
						grassQueue.Enqueue (new Coord(x, y));
					}

					if (map [x, y] == TileType.STONE && map [x, y + 1] == TileType.NONE) {
						if (biomesMap [x, y] == BiomeType.SAND)
							map [x, y] = TileType.STONE_SAND;
						else if (biomesMap [x, y] == BiomeType.SNOW)
							map [x, y] = TileType.STONE_SNOW;
						else
							map [x, y] = TileType.STONE_DIRT;

						stoneQueue.Enqueue (new Coord (x, y));
					}

					if (map[x, y] == TileType.GREYSTONE && map[x, y + 1] == TileType.NONE) {
						map [x, y] = TileType.GREYSTONE_SAND;
					}

					if (map[x, y] == TileType.REDSTONE && map[x, y + 1] == TileType.NONE) {
						map[x, y] = TileType.REDSTONE_SAND;
					}
				}
			}
		}

		/* Sets the surface of the stone biomes */
		while (stoneQueue.Count != 0) {
			Coord tile = stoneQueue.Dequeue();
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
					grassQueue.Enqueue (new Coord(tile.x, tile.y + 1));
				}
			}
		}
			
		/* Grow grass */
		NoiseGenerator grassGenerator = new NoiseGenerator ();

		while (grassQueue.Count != 0) {

			Coord tile = grassQueue.Dequeue ();

			if (grassGenerator.PerlinNoise(tile.x, tile.y, 10, 1, 1) > 0.5f && IsInMapRange (tile.x, tile.y + 1) && map[tile.x, tile.y + 1] == TileType.NONE) {
				map [tile.x, tile.y + 1] = grassList[UnityEngine.Random.Range (0, grassList.Count)];
			}

		}

	}
	
	void GenerateSquares() {
		squareGenerator.deleteSquares ();
		squareGenerator.GenerateSquares (borderedMap);
	}
	
	void UpdateBorderedMap ()	{
		borderedMap = new TileType[width + borderSize * 2, height + borderSize * 2];

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

	BiomeType GetBiomeType (int x, int y) {

		BiomeType biomeRet = BiomeType.NONE;

		if (y >= (groundLevel * 1.1)) {
			biomeRet = BiomeType.DEFAULT;
		} else {
			while (biomeRet == BiomeType.DEFAULT || biomeRet == BiomeType.NONE) {
				Array biomesArray = Enum.GetValues (typeof(BiomeType));
				biomeRet = ((BiomeType)biomesArray.GetValue(UnityEngine.Random.Range (1, biomesArray.Length)));
			}
		}

		return biomeRet;
	}
		
}