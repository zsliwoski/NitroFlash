using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack : MonoBehaviour {

	public float packforce;

	public int jumpsinair;

	public AudioClip jumpsound;
	public AudioSource aud;

	ReceiveImpact phys;
	PlayerMovement playermove;
	int numberOfJumps;
	// Use this for initialization
	void Start () {
		phys = GetComponentInParent<ReceiveImpact> ();
		playermove = GetComponentInParent<PlayerMovement> ();
	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetKeyDown (KeyCode.Space)) {
//			if (!playermove.cc.isGrounded)
//				if (numberOfJumps > 0)
//					BlastOff (transform.up, packforce);
//		}
		if (playermove.cc.collisionFlags == CollisionFlags.Below) {
			numberOfJumps = jumpsinair;
		} else if (playermove.cc.collisionFlags != CollisionFlags.Below) {
			if (Input.GetKeyDown (KeyCode.Space))
				if (numberOfJumps > 0)
					BlastOff (transform.up, packforce);
			if (Input.GetKeyDown (KeyCode.LeftShift))
				if (numberOfJumps > 0)
					BlastOff (transform.forward, packforce);
		}
	}
	void BlastOff(Vector3 direction, float force){
		aud.clip = jumpsound;
		aud.Play ();
		phys.AddImpact (direction, packforce, true);
		numberOfJumps -= 1;
	}
}
