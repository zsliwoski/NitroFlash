using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that adds an impact to the attached character
/// Note: This is necessary due to custom character controller logic
/// </summary>
public class ReceiveImpact : MonoBehaviour {

	float mass = 3.0f; // defines the character mass
	Vector3 impact = Vector3.zero; 
	float explosionAcceleration;
	bool decays;
	PlayerMovement character;

	// Use this for initialization
	void Start () {
		character = GetComponent<PlayerMovement>(); 
	}

	/// <summary>
	/// Adds an impact force to the attached character
	/// </summary>
	/// <param name="dir">Direction to apply the force</param>
	/// <param name="force">Intensity of imparted force</param>
	/// <param name="decayovertime">How quickly the force disipates</param>
	public void AddImpact(Vector3 dir, float force, bool decayovertime){ 
		decays = decayovertime;
		dir.Normalize();
		//if (dir.y < 0)
		//	dir.y = -dir.y; // reflect down force on the ground 
		impact += dir.normalized * force / mass; 
		character.playerVelocity += impact;
	}
	
	//Update on a fixed timer (physics)
	void FixedUpdate () {
		if (decays)
			if (impact.magnitude > 0.2) 
				// consumes the impact energy each cycle:
				impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime); 
	}
}
