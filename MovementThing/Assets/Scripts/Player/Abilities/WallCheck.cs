using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour {

	public bool colliding;

	void Update(){
		print (name + " : colliding = " + colliding);
	}

	void OnCollisionEnter(){
		colliding = true;
	}
	void OnCollisionExit(){
		colliding = false;
	}
}
