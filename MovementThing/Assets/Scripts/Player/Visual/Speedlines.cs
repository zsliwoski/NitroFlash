using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedlines : MonoBehaviour {
	Animator psAnim;
	public float amount = 0.0f;

	void Start(){
		psAnim = GetComponent<Animator> ();
	}
	public void SetAmount(float amount){
		this.amount =  Mathf.Clamp (amount,0.0f,1.0f);
	}
	void Update(){
		psAnim.SetFloat ("Alpha", amount);
	}
}
