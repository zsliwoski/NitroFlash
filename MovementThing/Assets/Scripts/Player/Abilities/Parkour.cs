using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class Parkour : MonoBehaviour {
	public delegate void WallJumpDelegate(bool forcedOff);
	public event WallJumpDelegate WallJumpEvent;


	public float forwardForce;
	public float jumpoffForce;

	public float timeToStick;
	public float wallSpeed;
	public float wallGravity;
	public float wallFriction;
	public float camWallAngle;

	public float maxDistanceAway;

	public LayerMask runnables;
	public AudioClip climbupsfx;
	public AudioClip dismountsfx;
	public AudioSource aud;
	public AudioSource audLoop;

	RaycastHit hit;

	ReceiveImpact phys;
	PlayerMovement playermove;

	float defaultGravity;
	float wishCamAngle;
	float walltimer = 0.0f;
	float defaultMoveSpeed;
	//float defaultFriction;
	float defaultYOffset;

	bool midrun = false;
	//bool hasJump = true;
	bool hasDoubleJump = false;
	//bool latchingtoWall = false;
	bool sprinting = false;
	bool crouched = false;
	bool sliding = false;
	bool alreadyclimbing = false;

	[SerializeField]
	private float overlapBoxYTranslation;
	[SerializeField]
	private float overlapBoxForwardTranslation;

	Camera cammove;
	Vector3 camMovePos = Vector3.zero;
	bool camshouldmove = false;

	Vector2 input = Vector2.zero;

	public bool isHoldingWall(){
		return midrun;
	}

	// Use this for initialization
	void Start () {
		
		phys = GetComponentInParent<ReceiveImpact> ();
		playermove = GetComponentInParent<PlayerMovement> ();
		defaultGravity = playermove.gravity;
		defaultMoveSpeed = playermove.moveSpeed;
		//defaultFriction = playermove.friction;
		defaultYOffset = playermove.playerViewYOffset;
	}
	void Update(){
		if (cammove != null && camshouldmove) {
			cammove.transform.position = Vector3.Lerp (cammove.transform.position, camMovePos, Time.deltaTime * 20);
		}
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (hasDoubleJump) {
				if (!midrun) {
					if (!Physics.Raycast (playermove.cc.transform.position, -playermove.cc.transform.up, 2.2f)) {
						playermove.Jump (true);
						hasDoubleJump = false;
					}
				}
			}
		}
		if (midrun) {
			if (Input.GetKeyDown (KeyCode.Space) && !playermove.cc.isGrounded) {
				//phys.AddImpact (transform.forward, forwardForce, true);
				phys.AddImpact (hit.normal, jumpoffForce, true);
				aud.clip = dismountsfx;
				aud.Play ();
				if (WallJumpEvent != null) {
					WallJumpEvent.Invoke (false);
				}
				//hasJump = false;
			}
			if (walltimer > timeToStick) {
				phys.AddImpact (hit.normal, jumpoffForce / 2, true);
				if (WallJumpEvent != null) {
					WallJumpEvent.Invoke (true);
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			if (!crouched && !sliding) {
				if (Physics.Raycast (playermove.transform.position, -playermove.transform.up, 1.2f)) {
					sprinting = true;
					playermove.moveSpeed = defaultMoveSpeed * 1.5f;
				}
			}
		}
		if (sprinting) {
			if (!Input.GetKey (KeyCode.LeftShift)) {
				sprinting = false;
				playermove.moveSpeed = defaultMoveSpeed;
			}
		}
		if (Input.GetKeyDown (KeyCode.LeftControl)) {
			if (sprinting && !crouched) {
				if (Physics.Raycast(playermove.transform.position,-playermove.transform.up,1.2f)) {
					//print ("Slide boy");
					Slide ();
				}
			}
			else if (!crouched && !sprinting) {
				Crouch ();
			}
		}
		if (sliding) {
			if (playermove.playerVelocity.sqrMagnitude < 10) {
				Crouch ();
			}
		}if (crouched) {
			if (!Input.GetKey (KeyCode.LeftControl)) {
				Stand ();
			}
		}
		//print (IsClimbingObstructed());
	}

	// Update is called once per frame
	void FixedUpdate () {
		
		if (!playermove.cc.isGrounded) {
			input = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
			midrun = false;
			if (!crouched && !sliding) {
				if (input.x > 0.1f) {
					WallHold (1);
				} else if (input.x < -0.1f) {
					WallHold (2);
				}
				if (input.y > 0.1f) {
					WallHold (0);
				}
			}
		} else {
			playermove.gravity = defaultGravity;
			wishCamAngle = 0;
			hasDoubleJump = true;
		}
		if (!midrun) {
			playermove.gravity = defaultGravity;
			//hasJump = true;
			walltimer = 0;
			wishCamAngle = 0;
			if (audLoop.isPlaying) {
				audLoop.Pause ();
			}
		}
		playermove.rotZ = Mathf.Lerp (playermove.rotZ, wishCamAngle, Time.deltaTime * 4);
		//print ("IsGrounded? : " + playermove.cc.isGrounded + ", Has double jump? : " + hasDoubleJump);
	}
	void Slide(){
		playermove.moveSpeed = 0;
		playermove.friction = 1;
		playermove.cc.height = 1;
		//playermove.runAcceleration = 0;
		Vector3 newpos = playermove.transform.position;
		newpos.y -= 0.45f;
		playermove.transform.position = newpos;

		playermove.playerViewYOffset = 0;
		phys.AddImpact (playermove.transform.forward, 10, true);
		sliding = true;
	}
	void Crouch(){
		playermove.friction = 6;
		playermove.moveSpeed = defaultMoveSpeed / 1.5f;
		playermove.cc.height = 1;
		if (!sliding) {
			Vector3 newpos = playermove.transform.position;
			newpos.y -= 0.45f;
			playermove.transform.position = newpos;
		}
		playermove.playerViewYOffset = 0;
		sliding = false;
		crouched = true;
	}
	void Stand(){
		playermove.friction = 6;
		playermove.moveSpeed = defaultMoveSpeed ;
		playermove.cc.height = 2;
		playermove.playerViewYOffset = defaultYOffset;
		sliding = false;
		crouched = false;
	}
	void WallHold(int dir){
		if (dir == 0) {
			if (Physics.Raycast (transform.position, playermove.transform.forward, out hit, maxDistanceAway, runnables)) {
				midrun = true;
				playermove.gravity = wallGravity;
				phys.AddImpact (transform.up, (forwardForce/2) * input.y, true);
				playermove.ApplyFriction (wallFriction);
				walltimer += Time.deltaTime;
				phys.AddImpact (-hit.normal, 0.1f * input.sqrMagnitude, true);
				playermove.playerVelocity.x = 0;
				wishCamAngle = 0;
				if (!audLoop.isPlaying) {
					audLoop.Play ();
				}
				hasDoubleJump = true;
				if (!IsClimbingObstructed () && !alreadyclimbing) {
					if (Vector3.Angle (-hit.normal, playermove.transform.forward) <= 30f) {
						alreadyclimbing = true;
						StartCoroutine (ClimbLedge ());
					}
				}
			}
		} else if (dir == 1) {
			if (Physics.Raycast (transform.position, playermove.transform.right, out hit, maxDistanceAway, runnables)) {
				midrun = true;
				playermove.gravity = wallGravity;
				phys.AddImpact (transform.forward, forwardForce * input.y, true);
				playermove.ApplyFriction (wallFriction);
				wishCamAngle = camWallAngle;
				walltimer += Time.deltaTime;
				phys.AddImpact (-hit.normal, 0.1f * input.sqrMagnitude, true);
				playermove.playerVelocity.y = 0;
				if (!audLoop.isPlaying) {
					audLoop.Play();
				}
				hasDoubleJump = true;
			}
		} else if (dir == 2) {
			if (Physics.Raycast (transform.position, -playermove.transform.right, out hit, maxDistanceAway, runnables)) {
				midrun = true;
				playermove.gravity = wallGravity;
				phys.AddImpact (transform.forward, forwardForce * input.y, true);
				playermove.ApplyFriction (wallFriction);
				wishCamAngle = -camWallAngle;
				walltimer += Time.deltaTime;
				phys.AddImpact (-hit.normal, 0.1f * input.sqrMagnitude, true);
				playermove.playerVelocity.y = 0;
				if (!audLoop.isPlaying) {
					audLoop.Play ();
				}
				hasDoubleJump = true;
			}
		} else {
			midrun = false;
		}
	}
	private IEnumerator ClimbLedge(){
		playermove.canMove = false;
		playermove.playerVelocity = Vector3.zero;
		Vector3 referencespace = playermove.transform.position;
		referencespace.y += playermove.cc.height;
		referencespace += playermove.transform.forward;
		Vector3 telespace = playermove.transform.position;
		RaycastHit hit;
		GameObject gam = new GameObject ();
		cammove =  gam.AddComponent<Camera>();
		var post = gam.AddComponent<PostProcessingBehaviour> ();
		post.profile = GetComponent<PostProcessingBehaviour> ().profile;
		cammove.tag = "MainCamera";
		cammove.transform.position = transform.position;
		cammove.transform.rotation = transform.rotation;
		GetComponent<Camera> ().enabled = false;
		if (Physics.Raycast (referencespace, -Vector3.up, out hit,playermove.cc.height)) {
			telespace = hit.point;
			telespace.y += playermove.cc.height/2;
		}
		camMovePos = telespace;
		camMovePos.y += playermove.playerViewYOffset;
		camshouldmove = true;
		aud.clip = climbupsfx;
		aud.Play ();
		yield return new WaitForSeconds (0.2f);
		playermove.transform.position = telespace;
		camshouldmove = false;
		GetComponent<Camera> ().enabled = true;
		Destroy(gam);
		playermove.playerVelocity = Vector3.zero;
		playermove.canMove = true;
		alreadyclimbing = false;
	}
	private bool IsClimbingObstructed()
	{
		Collider[] results = new Collider[3];

		return Physics.OverlapBoxNonAlloc(new Vector3(playermove.transform.position.x,
			playermove.transform.position.y + overlapBoxYTranslation, playermove.transform.position.z) +
			playermove.transform.forward * overlapBoxForwardTranslation,
			Vector3.one, results, playermove.transform.rotation, runnables) != 0;
	}
	private void OnDrawGizmosSelected()
	{
		if (transform != null)
		{
			Gizmos.color = Color.red;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube(new Vector3(0f, overlapBoxYTranslation,
				overlapBoxForwardTranslation), new Vector3(2f, 2f, 2f));
		}
	}
}
