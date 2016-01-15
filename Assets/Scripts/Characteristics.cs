using UnityEngine;
using System.Collections;

public class Characteristics {
	
	public float jumpHeight;
	public float timeToJumpApex;
	public float groundedAcceleration;
	public  float airborneAcceleration;
	public float moveSpeed;
	public int health;
	public int armor;
	public int contactDamage;
	public float recoverTime;

	public bool isAlive;

	float lastHitTime = float.MinValue;

	public Characteristics () {
		jumpHeight = 4;
		timeToJumpApex = 0.4f;
		groundedAcceleration = 0.1f;
		airborneAcceleration = 0.3f;
		moveSpeed = 6;
		health = 10;
		armor = 1;
		contactDamage = 2;
		recoverTime = 0.5f;
		isAlive = true;
	}


	void TakeDamage (int damage) {
		health -= damage;
		lastHitTime = Time.time;

		if (health < 0)
			isAlive = false;
	}

	public void TakeContactHit (Characteristics other) {
		if (armor <= other.contactDamage && (lastHitTime + recoverTime) < Time.time) {
			TakeDamage (other.contactDamage - armor);
		}
	}
}
