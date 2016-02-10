using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mob spawner.
/// </summary>

public class MobSpawner : MonoBehaviour {

	public Teleporter tp;
	public GameObject BasicMob;
	public GameObject FlyingMob;

	public int mobsMaximumCount = 10;
	public float mobsPopCooldown = 1;
	public bool isSpawningMobs = false;
	public int chanceToPopEachFrame = 10;

	float timeOfLastPop;

	List<GameObject> mobList;
	List<GameObject> potentialMobs;


	/// <summary>
	/// Toggle the spawn of mobs
	/// </summary>
	/// <param name="value">If set to <c>true</c>, spawn mobs.</param>
	public void setSpawn(bool value) {
		isSpawningMobs = value;
	}

	/// <summary>
	/// Spawns a mob of type specified.
	/// </summary>
	/// <param name="g">The type of mob spawned</param>
	void SpawnMob (GameObject g) {
		GameObject mob = (GameObject)Instantiate(g, new Vector3(0, 0, 0), Quaternion.identity);
		mob.name = g.name + mobList.Count;
		mob.transform.parent = transform;
		mobList.Add (g);
		tp.TeleportPlayerInSquareMap(mob);
	}

	void Start () {
		mobList = new List<GameObject> ();
		potentialMobs = new List<GameObject> ();

		potentialMobs.Add (BasicMob);
		potentialMobs.Add (FlyingMob);
	}

	void Update () {

		if (UnityEngine.Random.Range(0, 101) < chanceToPopEachFrame && (Time.timeSinceLevelLoad - timeOfLastPop) > mobsPopCooldown && mobList.Count < mobsMaximumCount && isSpawningMobs) {

			timeOfLastPop = Time.timeSinceLevelLoad;

			GameObject g = potentialMobs[UnityEngine.Random.Range(0, potentialMobs.Count)];

			SpawnMob(g);
		}
	}
}