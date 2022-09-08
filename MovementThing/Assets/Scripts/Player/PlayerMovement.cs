using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.UI;
using AssemblyCSharp;

public class PlayerMovement : NetworkPlayerBehavior{

	//EVENTS//
	public delegate void RespawnDelegate ();
	public event RespawnDelegate RespawnEvent;

	public delegate void GetKillDelegate (string instigator, string playerKilledName);
	public event GetKillDelegate GetKillEvent;

	public delegate void DeathDelegate (string instigator, Vector3 position, Quaternion rotation);
	public event DeathDelegate DeathEvent;

	public delegate void DisconnectDelegate();
	public event DisconnectDelegate DisconnectEvent;

	public delegate void JumpDelegate(bool midAir);
	public event JumpDelegate JumpEvent;

	GamemodeBase gamemodeRef;

	//network stuff
	public string playerName = "";
	public bool SERVER_isDead = false;
	public PlayerScoreStruct playerScoreStruct;

	//VARIABLES//
	public bool localDebug = false;
	public Renderer head;
	public GameController gam;
	public GameObject camPref;
	public GameObject ui;
	UIHandler uH;
	private Canvas can;

	public Transform playerView; //the players camera

	//MOVEMENT
	public float playerViewYOffset 			= 0.6f; // the camera's height
	public float xMouseSensitivity 			= 30.0f;
	public float yMouseSensitivity 			= 30.0f;

	public float gravity 					= 20.0f;
	public float friction 					= 6.0f; //ground friction

	public float moveSpeed 					= 7.0f;	// ground move speed
	public float runAcceleration 			= 14f; 	// ground acceleration
	public float runDeacceleration 			= 10f; 	// deaccelertion that occurs on ground
	public float airAcceleration 			= 2.0f;	// air acceleration
	public float airDeacceleration 			= 2.0f;	// deacceleration when moving opposite
	public float airControl 				= 0.3f; // precision of air control
	public float sideStrafeAcceleration		= 50f; 	// speed of acceleration to get to sidestrafe speed
	public float sideStrafeSpeed			= 1f; 	// the max speed of side strafing
	public float jumpSpeed					= 8.0f; // speed the player moves upward
	public float moveScale					= 1.0f;
	public float floorAngle					= 0.0f;
	//style of debug hud
	public GUIStyle style;

	//array of sounds to play randomly when player jumps
	public AudioClip[] jumpSounds;

	public CharacterController cc;

	public bool canMove = true;
	bool wantLockCursor = true;
	//Camera Rotation
	private float rotX = 0.0f;
	private float rotY = 0.0f;
	public float rotZ = 0.0f;

	//private Vector3 moveDirection = Vector3.zero;
	//private Vector3 moveDirectionNorm = Vector3.zero;
	public Vector3 playerVelocity = Vector3.zero;

	Vector2 showVel;

	private float playerTopVelocity = 0.0f;

	//if the player is touching the ground
	//private bool grounded = false;

	//if the player is held space while midair
	private bool wishJump = false;

	public GameObject camObj;
	
	//displays realtime friction
	//private float playerFriction = 0.0f;

	public class Cmd{
		public float forwardmove;
		public float rightmove;
		public float upmove;
	}

	private Cmd cmd; //stores player movement requests

	private bool mouseMovementFrozen = false;

	//controls where the player spawns
	private Vector3 playerSpawnPos;
	private Quaternion playerSpawnRot;

	public void FreezeMouseInput (bool shouldFreezeMouse){
		mouseMovementFrozen = shouldFreezeMouse;
	}

	public bool GetMouseInputFrozen(){
		return mouseMovementFrozen;
	}

	public void SetCursorLock(bool wantLock){
		wantLockCursor = wantLock;
		if (wantLock) {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

	public bool VisualGrounded(){
		RaycastHit hit;
		Ray ray = new Ray (transform.position, -(transform.up));
		Debug.DrawRay (ray.origin, ray.direction, Color.red);
		bool grounded = false;
		if (Physics.Raycast (ray.origin, ray.direction, out hit, cc.height + 0.1f)) {
			grounded = hit.collider != null;
		}
		return grounded;
	}

	//TODO: this method is evolving, use it to clean up other things as well
	protected void OnNetworkDisconnect(){
		if (DisconnectEvent != null){
			DisconnectEvent.Invoke();
		}

		gam.RemovePlayerFromRegistry (this);
		
		networkObject.ClearRpcBuffer ();
		if (networkObject != null){
			networkObject.Destroy ();
		}
	}

	protected override void NetworkStart ()
	{
		base.NetworkStart ();

		cc = GetComponent<CharacterController> ();
		playerScoreStruct.kills = 0;
		playerScoreStruct.deaths = 0;
		playerScoreStruct.score = 0;
		gam = FindObjectOfType<GameController> ();
		gam.AddPlayerToRegistry (this);
		if (networkObject.IsOwner) {
			
			camObj = Instantiate (camPref,playerView);
			GameObject u = Instantiate (ui);

			UI_MatchEndCard card = FindObjectOfType<UI_MatchEndCard>();
			card.SetOwner (this);

			uH = u.GetComponent<UIHandler> ();
			uH.ph = GetComponent<PlayerHealth> ();
			uH.sh = camObj.GetComponent<Shotgun> ();
			uH.pm = this;
			uH.gr = GetComponentInChildren<Grapple>();
			head.enabled = false;

			gameObject.tag = "Player";
			gameObject.layer = LayerMask.NameToLayer ("Self");
			//puts camera into capsule collider
			playerView.position = new Vector3 (transform.position.x,
				transform.position.y + playerViewYOffset,
				transform.position.z);

			cmd = new Cmd ();

			playerName = PlayerPrefs.GetString ("PlayerName");
			if (playerName == "") {
				string newName = NameGenerator.GenerateName ();
				playerName = newName;
				PlayerPrefs.SetString ("PlayerName", newName);
			}

			networkObject.SendRpc (NetworkPlayerBehavior.RPC_SERVER__SET_NAME, Receivers.Server, playerName);

			//clears scores
			gamemodeRef = FindObjectOfType<GamemodeBase>();
			gamemodeRef.RoundStartEvent += () => {
				networkObject.score = 0;
				networkObject.SendRpc(NetworkPlayerBehavior.RPC_MULTICAST__RESPAWN, true, Receivers.Owner);
			};
			gamemodeRef.RoundEndEvent += (winningTeam) => {print(string.Format("{0} wins!!!",winningTeam));};
		} else {
			//TODO:Change based on team
			gameObject.tag = "Enemy";
			CapsuleCollider c = gameObject.AddComponent<CapsuleCollider> ();
			c.height = cc.height;
			c.radius = cc.radius;
		}

	}

	void Update(){
		//this is a little networking block, nothing too fancy
		if (networkObject.Owner.Disconnected) {
			OnNetworkDisconnect();
		}

		if (!networkObject.IsOwner) {
			transform.rotation = networkObject.rotation;
			transform.position = networkObject.position;
			return;
		}
		if (Input.GetKeyDown (KeyCode.Tab)) {
			uH.ToggleScoreboard (true);
		}
		if (Input.GetKeyUp (KeyCode.Tab)) {
			uH.ToggleScoreboard (false);
		}

		if (Input.GetKeyDown (KeyCode.P)) {
			networkObject.Networker.Disconnect (false);
		}
		//make sure that the cursor is locked onto the screen
		if (Cursor.lockState != CursorLockMode.Locked && wantLockCursor) {
			if (Input.GetMouseButtonDown (0)) {
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
		if (canMove) {
			if (mouseMovementFrozen == false){
				//camera movement
				rotX -= Input.GetAxisRaw ("Mouse Y") * xMouseSensitivity * 0.02f;
				rotY += Input.GetAxisRaw ("Mouse X") * yMouseSensitivity * 0.02f;

				// Clamp the X rotation
				if (rotX < -90)
					rotX = -90;
				else if (rotX > 90)
					rotX = 90;
				this.transform.rotation = Quaternion.Euler (0, rotY, 0); // Rotates the collider
				playerView.rotation = Quaternion.Euler (rotX, rotY, rotZ); // Rotates the camera
			}
			// Set the camera's position to the transform
			playerView.position = new Vector3 (transform.position.x,
				transform.position.y + playerViewYOffset,
				transform.position.z);

			/* Movement, here's the important part */
			QueueJump ();
			if (cc.isGrounded)
				GroundMove ();
			else if (!cc.isGrounded)
				AirMove ();

			//move the controller
			cc.Move (playerVelocity * Time.deltaTime);

			//calculate top velocity
			Vector3 udp = playerVelocity;
			udp.y = 0.0f;
			if (playerVelocity.magnitude > playerTopVelocity) {
				playerTopVelocity = playerVelocity.magnitude;
			}
		}
		if (Input.GetKeyDown (KeyCode.M)) {
			transform.position = Vector3.zero;
		}
		//assigning our network variables our final position and rotation at the end of the frame
		networkObject.position = transform.position;
		networkObject.rotation = transform.rotation;
	}

	public int PlayerHSpeed(){
		showVel.x = playerVelocity.x;
		showVel.y = playerVelocity.z;
		return (int)Mathf.Floor(showVel.magnitude);
	}

	//movement

	//Set movement direction based on player input
	void SetMovementDir(){
		cmd.forwardmove = Input.GetAxisRaw ("Vertical");
		cmd.rightmove 	= Input.GetAxisRaw ("Horizontal");
	}

	//Queueing next jump
	void QueueJump(){
		if (Input.GetKeyDown (KeyCode.Space) && !wishJump) {
			wishJump = true;
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			wishJump = false;
		}
	}

	void AirMove(){
		//Pretty sure accelerate is supposed to fuckin work, it doesn't apply midair movement though
		Vector3 wishdir;
		float accel;

		float scale = CmdScale();

		SetMovementDir ();

		wishdir = new Vector3 (cmd.rightmove, 0, cmd.forwardmove);
		wishdir = transform.TransformDirection (wishdir);

		float wishspeed = wishdir.magnitude;
		wishspeed *= moveSpeed;

		wishdir.Normalize ();
		//moveDirectionNorm = wishdir;
		wishspeed *= scale;

		//aux air control
		float wishspeed2 = wishspeed;
		if (Vector3.Dot (playerVelocity, wishdir) < 0) {
			accel = airDeacceleration;
		} else {
			accel = airAcceleration;
		}

		// left or right speed
		if (cmd.forwardmove == 0 && cmd.rightmove != 0) {
			if (wishspeed > sideStrafeSpeed) {
				wishspeed = sideStrafeSpeed;
			}
			accel = sideStrafeAcceleration;
		}

		//Accelerate(wishdir, wishspeed, accel);
		if (airControl > 0) {
			AirControl(wishdir, wishspeed2);
		}

		//apply gravity
		playerVelocity.y -= gravity * Time.deltaTime;

	}

	//air control occurs when in air it lets players move side to side much faster
	void AirControl(Vector3 wishdir, float wishspeed){
		float	zspeed;
		float	speed;
		float	dot;
		float	k;

		//cant control movement if not forward or backwards
		if (cmd.forwardmove == 0 || wishspeed == 0) {
			return;
		}

		zspeed = playerVelocity.y;
		playerVelocity.y = 0;

		//ported vector normalize
		speed = playerVelocity.magnitude;
		playerVelocity.Normalize ();

		dot = Vector3.Dot (playerVelocity, wishdir);
		k = 32;
		k *= airControl * dot * dot * Time.deltaTime;

		if (dot > 0) {
			playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
			playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
			playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;

			playerVelocity.Normalize();
			//moveDirectionNorm = playerVelocity;
		}

		playerVelocity.x *= speed;
		playerVelocity.y = zspeed; // Note this line
		playerVelocity.z *= speed;

	}

	//called every frame players on the ground
	void GroundMove(){
		Vector3 wishdir;
		//Vector3 wishvel;

		if (!wishJump) {
			ApplyFriction(1.0f);
		} else {
			ApplyFriction(0);
		}

		//float scale = CmdScale();
	
		SetMovementDir();

		wishdir = new Vector3 (cmd.rightmove, 0, cmd.forwardmove);
		wishdir = transform.TransformDirection (wishdir);
		wishdir.Normalize ();
		//moveDirectionNorm = wishdir;

		float wishspeed = wishdir.magnitude;
		wishspeed *= moveSpeed;

		Accelerate(wishdir, wishspeed, runAcceleration);

		// Reset gravity velocity
		playerVelocity.y = 0;

		if (wishJump) {
			Jump (false);
		}
	}

	public void Jump(bool midAir){
		playerVelocity.y = jumpSpeed;
		wishJump = false;
		PlayJumpSound ();
		if (JumpEvent != null) {
			JumpEvent.Invoke (midAir);
		}
	}

	//calculates friction to add
	public void ApplyFriction(float t){
		Vector3 vec = playerVelocity;
		//float vel;
		float speed;
		float newspeed;
		float control;
		float drop;

		vec.y = 0.0f;
		speed = vec.magnitude;
		drop = 0.0f;

		//if player is on ground, apply
		if (cc.isGrounded) {
			control = speed < runDeacceleration ? runDeacceleration : speed;
			drop = control * friction * Time.deltaTime * t;
		}
		newspeed = speed - drop;
		//playerFriction = newspeed;
		if (newspeed < 0) {
			newspeed = 0;
		}
		if (speed > 0) {
			newspeed /= speed;
		}

		playerVelocity.x *= newspeed;
		//playerVelocity.y *= newspeed;
		playerVelocity.z *= newspeed;
	}

	//calculates acceleration based on cmd wish
	void Accelerate(Vector3 wishdir, float wishspeed, float accel){
		float addspeed;
		float accelspeed;
		float currentspeed;

		currentspeed = Vector3.Dot (playerVelocity, wishdir);
		addspeed = wishspeed - currentspeed;
		if (addspeed <= 0) {
			return;
		}
		accelspeed = accel * Time.deltaTime * wishspeed;
		if (accelspeed > addspeed) {
			accelspeed = addspeed;
		}

		playerVelocity.x += accelspeed * wishdir.x;
		playerVelocity.z += accelspeed * wishdir.z;
	}

	//messes with speed or something
	float CmdScale()
	{
		int max;
		float total;
		float scale;

		max = Mathf.Abs((int)cmd.forwardmove);
		if(Mathf.Abs(cmd.rightmove) > max)
			max = Mathf.Abs((int)cmd.rightmove);
		if(max == 0)
			return 0;

		total = Mathf.Sqrt(cmd.forwardmove * cmd.forwardmove + cmd.rightmove * cmd.rightmove);
		scale = moveSpeed * max / (moveScale * total);

		return scale;
	}

	//plays a jump sound
	void PlayJumpSound()
	{
		if (GetComponent<AudioSource> ().isPlaying)
			return;
		GetComponent<AudioSource> ().clip = jumpSounds [Random.Range (0, jumpSounds.Length)];
		GetComponent<AudioSource> ().Play ();
	}
	void OnControllerColliderHit(ControllerColliderHit hit) {
		floorAngle = Vector3.Angle (Vector3.up, hit.normal);
		playerVelocity = Vector3.ProjectOnPlane (playerVelocity, hit.normal);
	}

	//RPC Functions
	public override void Server_TakeDamage(RpcArgs args){
		//if (!networkObject.IsServer) {
		//	return;
		//}
		int dam = args.GetNext<int> ();
		string instigator = args.GetNext<string> ();
		uint instigatorID = args.GetNext<uint> ();
		GetComponent<PlayerHealth> ().TakeDamage (dam);
		networkObject.health = GetComponent<PlayerHealth> ().curHealth;
		if (!SERVER_isDead) {
			if (GetComponent<PlayerHealth> ().curHealth <= 0) {
				SERVER_isDead = true;
				object[] rpcArgs = { instigator, instigatorID };
				networkObject.SendRpc (NetworkPlayerBehavior.RPC_SERVER__DEATH, BeardedManStudios.Forge.Networking.Receivers.Owner, rpcArgs);
			} else {
				networkObject.SendRpc (NetworkPlayerBehavior.RPC_MULTICAST__TAKE_DAMAGE, BeardedManStudios.Forge.Networking.Receivers.All);
			}
		}
	}
	public override void Server_Death(RpcArgs args){
		if (networkObject.IsOwner) {	
			playerScoreStruct.deaths += 1;
			networkObject.death = playerScoreStruct.deaths;
		}

		//if (!networkObject.IsServer) {
		//	return;
		//}

		string instigator = args.GetNext<string> ();
		uint instigatorID = args.GetNext<uint> ();
		print(System.String.Format("{0}",instigatorID));
		if (instigatorID != 0) {
			NetworkPlayerBehavior killer = gam.GetNetworkPlayerFromID (instigatorID);
			if (killer) {
				killer.networkObject.SendRpc (NetworkPlayerBehavior.RPC_SERVER__GET_KILL, BeardedManStudios.Forge.Networking.Receivers.Owner, playerName);
			}
		}
		object[] multicastArgs = {networkObject.position, networkObject.rotation, instigator};
		networkObject.SendRpc (NetworkPlayerBehavior.RPC_MULTICAST__DEATH, BeardedManStudios.Forge.Networking.Receivers.All, multicastArgs);

		//TODO: TEMPORARY MAKE SPAWNING RELIANT ON PLAYER RESPAWN REQUEST
		networkObject.SendRpc (NetworkPlayerBehavior.RPC_SERVER__RESPAWN, BeardedManStudios.Forge.Networking.Receivers.Owner);
	}
	public override void Server_GetKill(RpcArgs args){
		playerScoreStruct.kills += 1;
		networkObject.kills = playerScoreStruct.kills;
		if (gamemodeRef.killsAreScore){
			networkObject.SendRpc(NetworkPlayerBehavior.RPC_CLIENT__ADD_SCORE, Receivers.Owner, gamemodeRef.scorePerKill);
		}
	}
	public override void Server_Respawn(RpcArgs args){
		SERVER_isDead = false;
		networkObject.SendRpc (NetworkPlayerBehavior.RPC_MULTICAST__RESPAWN, BeardedManStudios.Forge.Networking.Receivers.All);
	}

	public override void Server_SetName(RpcArgs args){
		string netName = args.GetNext<string> ();
		playerName = netName;

		Dictionary<uint,string> nameInfos = new Dictionary<uint,string>();

		foreach (NetworkPlayerBehavior npb in gam.playerObjects){
			PlayerMovement pm = (PlayerMovement)npb;
			nameInfos.Add (npb.networkObject.NetworkId, pm.playerName);
		}

		networkObject.SendRpc(NetworkPlayerBehavior.RPC_CLIENT__GET_NAMES, true, Receivers.AllBuffered, Tools.JSON.SerializeNameDict(nameInfos));	
	}
	public override void Client_GetNames(RpcArgs args){
		string namesStr = args.GetNext<string> ();
		print (namesStr);
		Dictionary<uint,string> names =  Tools.JSON.DeserializeNameDict(namesStr);
		gam.ApplyNames (names);
	}
	//MULTICASTS
	public override void Multicast_TakeDamage(RpcArgs args){
		print ("DAMAGE?????");
	}
	
	public override void Multicast_Death(RpcArgs args){
		Vector3 deathPos = args.GetNext<Vector3> ();
		Quaternion deathRot = args.GetNext<Quaternion> ();
		string instigator = args.GetNext<string> ();

		print ("DIED TO : " + instigator);

		playerScoreStruct.deaths = networkObject.death;
		playerScoreStruct.kills = networkObject.kills;
		playerScoreStruct.score = networkObject.score;

		//print (System.String.Format("deaths: {0} | kills: {1}| score: {2}",networkObject.death,networkObject.kills,networkObject.score));
		if (DeathEvent != null) DeathEvent.Invoke((string)instigator,deathPos,deathRot);
	}
	public override void Client_GetKill(RpcArgs args){
		print ("KILL GOT?");
		string playerKilled = args.GetNext<string> ();

		print (System.String.Format("deaths: {0} | kills: {1}| score: {2}",playerScoreStruct.deaths, playerScoreStruct.kills, playerScoreStruct.score));

		if (GetKillEvent != null) GetKillEvent.Invoke((string)playerName, playerKilled);

		//TODO: Remove circular dependcies
		if (gamemodeRef.killsAreScore) {
			int scoreAmount = gamemodeRef.scorePerKill;
			networkObject.SendRpc (NetworkPlayerBehavior.RPC_CLIENT__ADD_SCORE, Receivers.Owner, scoreAmount);
		}
	}
	public override void Multicast_GunFired(RpcArgs args){
		print ("Shotgun visuals??");
	}
	public override void Multicast_Respawn(RpcArgs args){
		print ("RESPAWN");
		if (networkObject.IsOwner) {
			Transform spawnPoint = gam.GetSpawn ();
			playerVelocity = Vector3.zero;
			transform.position = spawnPoint.position;
			transform.rotation = spawnPoint.rotation;
			GetComponent<PlayerHealth> ().FillHealth ();
			networkObject.health = GetComponent<PlayerHealth> ().curHealth;
		} else {
			transform.rotation = networkObject.rotation;
			transform.position = networkObject.position;
		}
		GetComponent<PlayerHealth> ().curHealth = networkObject.health;
		if (RespawnEvent != null) RespawnEvent.Invoke();
	}

	public override void Client_ClearValues(RpcArgs args){
		bool clearScore = args.GetNext<bool> ();
		bool clearKills = args.GetNext<bool> ();
		bool clearDeath = args.GetNext<bool> ();

		if(clearDeath)
			networkObject.death = 0;

		if (clearKills)
			networkObject.kills = 0;

		if (clearScore)
			networkObject.score = 0;
	}

	public override void Client_AddScore(RpcArgs args){
		int amount = args.GetNext<int> ();
		object[] scoreArgs = {amount, networkObject.team}; 
		gamemodeRef.networkObject.SendRpc (NetworkGamemodeObjectBehavior.RPC_CLIENT__ADD_SCORE, Receivers.Owner, scoreArgs);
	}

	public float GetKDRatio(){
		//prevent divide by zero, no NaNa's
		int deathVisual = networkObject.death > 1 ? networkObject.death : 1;
		return (float)networkObject.kills / deathVisual;
	}

	void OnTriggerEnter(Collider other){
		switch (other.tag) {
		case "DeathVolume":
			if (networkObject.IsServer) {
				int damage = 2000;
				print ("DAMAGE");
				object[] multicastArgs= {damage, "<<Environment>>", 0u};
				networkObject.SendRpc (NetworkPlayerBehavior.RPC_SERVER__TAKE_DAMAGE, BeardedManStudios.Forge.Networking.Receivers.ServerAndOwner, multicastArgs);
			}
			break;
		}
	}
}
