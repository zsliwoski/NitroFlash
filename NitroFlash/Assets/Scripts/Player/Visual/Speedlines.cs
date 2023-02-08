using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles updating First Person speedlines effect
/// </summary>
public class Speedlines : MonoBehaviour {
	Animator psAnim;
	public float amount = 0.0f;

	// Use this for initialization
	void Start(){
		psAnim = GetComponent<Animator> ();
	}
	
	/// <summary>
	/// Sets intensity of the effect
	/// </summary>
	/// <param name="amount">Intensity from 0 to 1</param>
	public void SetAmount(float amount){
		this.amount =  Mathf.Clamp (amount,0.0f,1.0f);
	}

	// Update is called once per frame
	void Update(){
		psAnim.SetFloat ("Alpha", amount);
	}
}
