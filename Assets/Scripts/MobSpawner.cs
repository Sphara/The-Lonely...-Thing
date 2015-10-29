using UnityEngine;
using System.Collections;


/// <summary>
/// Mob spawner.
/// </summary>

public class MobSpawner : MonoBehaviour {

	Teleporter tp;
	public GameObject antMob;
	int antNumber = 0;
	int[,] _map;
	public bool isSpawningMobs = false;

	void Start () {
		tp = GameObject.Find ("Teleporter").GetComponent<Teleporter> ();
	}

	/// <summary>
	/// Sets the stuff.
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

		if (antNumber == 0) {
			SpawnAnt();
		}

		if ((UnityEngine.Random.Range (0, 1000) == 1) && isSpawningMobs) {
			SpawnAnt();
		}
	}
}