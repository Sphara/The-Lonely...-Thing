using UnityEngine;
using System.Collections;

public class MobSpawnerScript : MonoBehaviour {

	Teleporter tp;
	public GameObject antMob;
	int nbAnts = 0;
	int[,] _map;
	public bool isSpawningStuff = false;

	void Start () {
		tp = GameObject.Find ("Teleporter").GetComponent<Teleporter> ();
	}

	public void setStuff(bool stuff) {
		isSpawningStuff = stuff;
	}

	void SpawnAnt () {
		GameObject ant = (GameObject)Instantiate(antMob, new Vector3(0, 0, 0), Quaternion.identity);
		ant.name = "ant" + nbAnts;
		ant.transform.parent = transform;
		nbAnts++;
		tp.TeleportPlayerInSquareMap(ant);
	}

	void Update () {

		if (nbAnts == 0) {
			SpawnAnt();
		}

		if ((UnityEngine.Random.Range (0, 1000) == 1) && isSpawningStuff) {
			SpawnAnt();
		}
	}
}