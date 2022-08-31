﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour {

	public float forceToGrappleWith;
	public float maxGrappleDistance;
	public float maxGrappleHold;
	public float startGrappleMultiplier;
	public float fireRate;
	public float swingLength;

	public AudioClip grappleStart;
	public AudioClip grappleHold;
	public AudioClip grappleStop;

	public AudioSource aud;

	public LayerMask grappleables;

	ReceiveImpact phys;
	PlayerMovement playermove;
	public GameObject grapplePoint;
	LineRenderer lineRend;
	public bool held;
	public bool validGrapple;
	public Transform visualGrapplePoint;
	public GameObject harpoonVisualObject;
	float nextfire = -0.1f;
	public RaycastHit hit;
	Vector3 dir = Vector3.zero;
	// Use this for initialization
	void Start () {
		playermove = GetComponentInParent<PlayerMovement> ();
		phys = GetComponentInParent<ReceiveImpact> ();
		lineRend = GetComponent<LineRenderer>();
		lineRend.sortingOrder = 1;
	}
	// Update is called once per frame
	void Update () {
		HasValidGrapple();
		if (Input.GetButtonDown ("Fire2")) {
			if (nextfire <= 0)
				ShootGrapple ();
		}
		if (Input.GetButtonUp ("Fire2")) {
			if (held)
				ReleaseGrapple ();
		}

		if (nextfire > 0)
			nextfire -= Time.deltaTime;

		if (held) {
			HoldGrappleVisual ();
		}
	}
	void FixedUpdate(){
		if (held) {
			HoldGrapplePhys ();
		}
	}
	public bool HasValidGrapple(){
		validGrapple = false;
		if (nextfire < 0){
			Ray ray = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
			if (Physics.Raycast (ray, out hit, maxGrappleDistance, grappleables)) {
				validGrapple = true;
			}
		}
		return validGrapple;
	}
	void ShootGrapple(){
		if (validGrapple) {
			if (grapplePoint == null) {
				grapplePoint = Instantiate (new GameObject ("GrapplePoint"), hit.point, Quaternion.identity, hit.transform) as GameObject;
			}
			else {
				grapplePoint.transform.position = hit.point;
				grapplePoint.transform.SetParent (hit.transform);
				aud.clip = grappleStart;
				aud.Play ();
				harpoonVisualObject.SetActive (false);
				held = true;
				lineRend.enabled = true;
				if (playermove.cc.isGrounded){
					phys.AddImpact (
						phys.transform.up, 
						forceToGrappleWith * startGrappleMultiplier, true
					);
					//phys.AddImpact (transform.forward, forceToGrappleWith * startGrappleMultiplier, true);
			}else {
					phys.AddImpact (
						grapplePoint.transform.position - transform.position, 
						forceToGrappleWith * startGrappleMultiplier, true
					);
					//phys.AddImpact (transform.forward, forceToGrappleWith * startGrappleMultiplier, true);
				}
			}
		} else {
			print ("No object to grapple onto");
		}
		nextfire = fireRate;
	}
	void HoldGrapplePhys(){
		phys.AddImpact (dir, forceToGrappleWith*3,true);
		phys.AddImpact (transform.forward, forceToGrappleWith, true);
		if (Vector3.Distance (grapplePoint.transform.position, transform.position) > maxGrappleHold) {
			ReleaseGrapple ();
		}
	}
	void HoldGrappleVisual(){
		Vector3 grapplepos = grapplePoint.transform.position;
		Vector3 thispos = transform.position;
		dir = grapplepos - thispos;

		lineRend.SetPosition (0, grapplePoint.transform.position);
		lineRend.SetPosition (1, visualGrapplePoint.position);
	}
	void ReleaseGrapple(){
		held = false;
		lineRend.enabled = false;
		harpoonVisualObject.SetActive (true);
		aud.clip = grappleStop;
		aud.Play ();
	}
}
