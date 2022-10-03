using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPVisuals : MonoBehaviour {
	public Animator tpAnimator;
	private PlayerMovement pm;
	Vector3 localVelocity;
	Vector3 lastPosition = Vector3.zero;
	// Use this for initialization
	void Start () {
		pm = GetComponent<PlayerMovement> ();
		pm.JumpEvent += JumpEffect;
	}
	
	// Update is called once per frame
	void Update () {
		
		localVelocity = transform.InverseTransformDirection (pm.networkObject.position - lastPosition);
		//localVelocity /= 5;
		SpeedEffect ();	
		lastPosition = pm.networkObject.position;
	}
	void JumpEffect(bool midAir){
		if (midAir) {
			tpAnimator.CrossFade ("jump_additive", 0.007f);
		} else {
			tpAnimator.CrossFade ("jump_additive", 0.007f);
		}

	}
	void SpeedEffect(){
		tpAnimator.SetFloat("WalkY",localVelocity.z);
		tpAnimator.SetFloat ("WalkX", localVelocity.x);
	}
}
