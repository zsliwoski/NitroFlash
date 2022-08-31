using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
	public int maxHealth = 100;
	public int curHealth = 100;
	public GameObject hurtParticle;
	public GameObject deathParticle;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Z)) {
			TakeDamage (20);
		}
	}

	public void TakeDamage(int damage){
		//really simple way of handling damage that doesn't let you underflow or overflow the health
		int newHealth = curHealth - damage;
		if (newHealth < curHealth) {
			GameObject g = Instantiate (hurtParticle, transform.position, transform.rotation,this.transform);
			Destroy (g, 2f);
		}
		if (newHealth < 1) {
			curHealth = 0;
		} else if (newHealth > maxHealth) {
			curHealth = maxHealth;
		} else {
			curHealth = newHealth;
		}

	}
	public void FillHealth(){
		curHealth = maxHealth;
	}
}
