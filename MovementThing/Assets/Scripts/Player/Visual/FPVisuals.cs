using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPVisuals : MonoBehaviour {
	public float speedlineMaxSpeed = 0.1f;
	public float speedlineMinimumSpeed = 15;
	public float armSpeedMaxReaction = 15;
	public float mainCamMaxFOV = 15;
	public float grappleArmReach = 1.0f;

	private float camStartFOV;
	private Camera startCam;
	public Grapple grapple;
	public FPIKControl fpIKControl;

	public Transform grappleUpPoint;
	public Transform grappleHeldPoint;
	public Parkour parkour;
	public Animator armAnimator;

	Speedlines sl;
	private PlayerMovement pm;
	// Use this for initialization
	void Start () {
		pm = GetComponentInParent<PlayerMovement> ();
		sl = GetComponentInChildren<Speedlines> ();
		startCam = pm.camObj.GetComponent<Camera> ();
		camStartFOV = startCam.fieldOfView;
		pm.JumpEvent += JumpEffect;
	}

	void JumpEffect(bool midAir){
		if (midAir) {
			armAnimator.CrossFade ("jumpBIG", 0.007f);
		} else {
			armAnimator.CrossFade ("jump", 0.007f);
		}

	}

	// Update is called once per frame
	void Update () {
		SpeedEffect ();
		GrappleEffect ();

	}
	void SpeedEffect(){
		float speedReaction = (float)pm.PlayerHSpeed();
		sl.SetAmount((speedReaction - speedlineMinimumSpeed) / speedlineMaxSpeed);
		startCam.fieldOfView = camStartFOV + (Mathf.Clamp((speedReaction / speedlineMaxSpeed),0.0f,1.0f) * mainCamMaxFOV);

		//Arms
		bool onGround = pm.VisualGrounded() || parkour.isHoldingWall();
		armAnimator.SetBool("PlayerInAir", !onGround);
		armAnimator.SetBool ("OnWall", parkour.isHoldingWall ());

		if (pm.cc.isGrounded) {
			armAnimator.SetFloat ("PlayerForwardSpeed", speedReaction / armSpeedMaxReaction);
		}
		if (parkour.isHoldingWall ()) {
			float wallMovement = (pm.cc.velocity.sqrMagnitude);
			armAnimator.SetFloat ("PlayerForwardSpeed", wallMovement / armSpeedMaxReaction);
		}
	}

	void GrappleEffect(){
		if (grapple && fpIKControl) {
			if(grapple.held && grapple.grapplePoint){
				Vector3 gHeld = ClampedPosition (transform.position, grapple.grapplePoint.transform.position, distance: grappleArmReach);
				grappleHeldPoint.transform.position = gHeld;
				grappleHeldPoint.transform.LookAt (gHeld);
				fpIKControl.leftHandObj = grappleHeldPoint;
			}else if (grapple.validGrapple) {
				fpIKControl.leftHandObj = grappleUpPoint;
			}else{
				fpIKControl.leftHandObj = fpIKControl.defaultLeftHandObj;
			}
		}
	}

	//TODO: Expand this later to fix the visual look of the clamped position
	Vector3 ClampedPosition(Vector3 a,Vector3 b, float maxAngle = 0.0f, float distance = 0.0f){
		//return  ((b-a)*distance) + a;
		return b;
	}
}
