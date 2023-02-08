using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Small class that manages an attached character's health.
/// </summary>
public class PlayerHealth : MonoBehaviour {
	public int maxHealth = 100;
	public int curHealth = 100;
	public GameObject hurtParticle;
	public GameObject deathParticle;

	/// <summary>
	/// Handles applying damage to the attached character
	/// </summary>
	/// <param name="damage"></param>
	public void TakeDamage(int damage){
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

	/// <summary>
	/// Resets attached character's health
	/// </summary>
	public void FillHealth(){
		curHealth = maxHealth;
	}
}
