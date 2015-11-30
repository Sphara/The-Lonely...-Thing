using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class (Maybe it should be a struct ?) regroups informations about minerals spawn. It is used by the mineralFarm to grow minerals on the map
/// </summary>
public class MineralInfo {
	public List<TileType> minerals;
	public List<BiomeType> compatibleBiomes;
	public int rarity;
	public int minHeight;
	public int maxHeight;

	/// <summary>
	/// Initializes a new instance of the <see cref="MineralInfo"/> class.
	/// </summary>
	/// <param name="_minerals">A list of the minerals to spawn</param>
	/// <param name="_compatibleBiomes">The biomes where these minerals spawn</param>
	/// <param name="_rarity">The rarity of these minerals (Should approximately be between 60 and 100)</param>
	/// <param name="min">The minimum height for these minerals to spawn</param>
	/// <param name="max">The maximum height for these minerals to spawn</param>
	public MineralInfo (List<TileType> _minerals, List<BiomeType> _compatibleBiomes, int _rarity, int min, int max) {
		minerals = _minerals;
		compatibleBiomes = _compatibleBiomes;
		rarity = _rarity;
		minHeight = min;
		maxHeight = max;
	}

};

/// <summary>
/// This is the class used to grow minerals on the map. It uses perlin noise and the MineralInfo struct to determine where they spawn.
/// 
/// To add a new mineral to the map, create a new MineralInfo in the start method, fill the class and add it to the availableMinerals queue. The last mineral added overwrites the already spawned minerals
/// 
/// I am still working on the values used by the perlin noise and the rarity of minerals, these might change a bit.
/// </summary>
public class MineralFarm : MonoBehaviour {

	Queue<MineralInfo> availableMinerals;

	void Start () {
		availableMinerals = new Queue<MineralInfo> ();

		availableMinerals.Enqueue (new MineralInfo (new List<TileType> () {TileType.GREYSTONE_RUBY, TileType.GREYSTONE_RUBY_ALT}, new List<BiomeType>() {BiomeType.GREYSTONE}, 78, 0, 50));
		availableMinerals.Enqueue (new MineralInfo (new List<TileType> () {TileType.STONE_COAL, TileType.STONE_COAL_ALT}, new List<BiomeType>() {BiomeType.STONE, BiomeType.SAND, BiomeType.SNOW}, 75, 0, 100));
		availableMinerals.Enqueue (new MineralInfo (new List<TileType> () {TileType.STONE_COPPER, TileType.STONE_COPPER_ALT}, new List<BiomeType> () {BiomeType.STONE, BiomeType.SAND, BiomeType.SNOW}, 82, 0, 75));
		availableMinerals.Enqueue (new MineralInfo (new List<TileType> () {TileType.STONE_SILVER, TileType.STONE_SILVER_ALT}, new List<BiomeType> () {BiomeType.STONE, BiomeType.SAND, BiomeType.SNOW}, 82, 0, 100));
		availableMinerals.Enqueue (new MineralInfo (new List<TileType> () {TileType.STONE_IRON, TileType.STONE_IRON_ALT}, new List<BiomeType> () {BiomeType.STONE, BiomeType.SAND, BiomeType.SNOW}, 82, 0, 100));
		availableMinerals.Enqueue (new MineralInfo (new List<TileType> () {TileType.STONE_GOLD, TileType.STONE_GOLD_ALT}, new List<BiomeType> () {BiomeType.STONE, BiomeType.SAND, BiomeType.SNOW}, 82, 0, 50));
		availableMinerals.Enqueue (new MineralInfo (new List<TileType> () {TileType.STONE_DIAMOND, TileType.STONE_DIAMOND_ALT}, new List<BiomeType> () {BiomeType.STONE, BiomeType.SAND, BiomeType.SNOW}, 90, 0, 25));
		availableMinerals.Enqueue (new MineralInfo (new List<TileType> () {TileType.REDSTONE_EMERALD, TileType.REDSTONE_EMERALD_ALT}, new List<BiomeType> () {BiomeType.REDSTONE}, 65, 0, 100));
		availableMinerals.Enqueue (new MineralInfo (new List<TileType> () {TileType.DIRT_GRAVEL}, new List<BiomeType> () {BiomeType.DEFAULT}, 70, 0, 100));
	}

	/// <summary>
	/// Grows the minerals on the map.
	/// </summary>
	/// <param name="map">The map where the minerals spawn</param>
	/// <param name="biomeMap">The biome map attached to the previous map.</param>
	public void GrowAll (TileType[, ] map, BiomeType[, ] biomeMap) {

		NoiseGenerator gen = new NoiseGenerator ();

		while (availableMinerals.Count > 0) {

			MineralInfo mineral = availableMinerals.Dequeue();

			gen.Reset();

			for (int x = 0; x < map.GetLength(0); x ++) {

				int y = (mineral.minHeight > 0 ? mineral.minHeight : 0);
				int maxY = (mineral.maxHeight < map.GetLength(1) ? mineral.maxHeight : map.GetLength(1));

				for (; y < maxY; y ++) {

					if (IsTileCompatible(biomeMap, x, y, mineral.compatibleBiomes) && (mineral.rarity < (int) gen.PerlinNoise(x, y, 10, 100, 1)))
						PlaceMinerals(map, x, y, mineral.minerals);

				}
			}
		
		}

	}

	void PlaceMinerals (TileType[, ] map, int x, int y, List<TileType> minerals)
	{
		if (map [x, y] != TileType.NONE && (y + 1) < map.GetLength(1) && map[x, y + 1] != TileType.NONE) {

			int i = Random.Range(0, minerals.Count);

			map[x, y] = minerals[i];
		
		}
	}

	bool IsTileCompatible (BiomeType[, ] bMap, int x, int y, List<BiomeType> biomes)
	{
		foreach (BiomeType biome in biomes) {
			if (bMap[x, y] == biome)
				return true;
		}

		return false;
	}
}
