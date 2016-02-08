using UnityEngine;
using System.Collections;


/// <summary>
/// Mob spawner.
/// </summary>

public class MobSpawner : MonoBehaviour {

	public Teleporter tp;
	public GameObject antMob;
	int antNumber = 0;
	int[,] _map;
	public bool isSpawningMobs = false;
	
	/// <summary>
	/// Toggle the spawn of mobs
	/// </summary>
	/// <param name="stuff">If set to <c>true</c> stuff.</param>
	public void setSpawn(bool value) {
		isSpawningMobs = value;
	}

	void SpawnAnt () {
		GameObject ant = (GameObject)Instantiate(antMob, new Vector3(0, 0, 0), Quaternion.identity);
		ant.name = "ant" + antNumber;
		ant.transform.parent = transform;
		antNumber++;
		tp.TeleportPlayerInSquareMap(ant);
	}

	void Update () {

		if (antNumber == 0 && isSpawningMobs) {
			SpawnAnt();
		}

		if ((UnityEngine.Random.Range (0, 100) == 1) && isSpawningMobs) {
			SpawnAnt();
		}
	}
}