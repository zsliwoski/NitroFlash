using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.UI;
using AssemblyCSharp;

/// <summary>
/// Class containing main player movement as well as networking logic
/// </summary>
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

	//Networking
	public string playerName = "";
	public bool SERVER_isDead = false;
	public PlayerScoreStruct playerScoreStruct;

	//VARIABLES//
	GamemodeBase gamemodeRef;
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

	//Array of sounds to play randomly when player jumps
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
	private bool queueJump = false;

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

	/// <summary>
	/// Handles stopping mouse input
	/// </summary>
	/// <param name="shouldFreezeMouse">Whether or not input is stopped</param>
	public void FreezeMouseInput (bool shouldFreezeMouse){
		mouseMovementFrozen = shouldFreezeMouse;
	}

	/// <summary>
	/// Handles returning mouse input state
	/// </summary>
	/// <returns>False if we aren't receiving input</returns>
	public bool GetMouseInputFrozen(){
		return mouseMovementFrozen;
	}

	/// <summary>
	/// Locks/Unlocks the mouse cursor
	/// </summary>
	/// <param name="wantLock">If True, the cursor is locked, else the cursor is free</param>
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

	/// <summary>
	/// If the attached character is "visually" touching the ground
	/// </summary>
	/// <returns>True if the character is close enough to be grounded</returns>
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
	/// <summary>
	/// Called whenever Client disconnects
	/// </summary>
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

	/// <summary>
	/// Network state begins
	/// </summary>
	protected override void NetworkStart (){
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

			string cust = GetComponentInChildren<CharacterPaint> (true).Serialize ();
			object[] customizationArgs = { cust, networkObject.NetworkId };
			networkObject.SendRpc (NetworkPlayerBehavior.RPC_MULTICAST__SEND_CUSTOMIZATION, Receivers.AllBuffered, customizationArgs);

			//clears scores
			gamemodeRef = FindObjectOfType<GamemodeBase>();
			gamemodeRef.RoundStartEvent += () => {
				networkObject.score = 0;
				networkObject.SendRpc(NetworkPlayerBehavior.RPC_MULTICAST__RESPAWN, true, Receivers.Owner);
			};
			gamemodeRef.RoundEndEvent += (winner) => {
				//TODO: this is bloated, break into smaller functions
				print(string.Format("{0} wins!!!",winner));
				int winState = 0; //default to lose

				//check win with team, then as a lone star
				if (gamemodeRef.usingTeams){
					if (winner == (int)networkObject.team){
						winState = 2;	
					}
				}else{
					if (winner == (int)networkObject.NetworkId){
						winState = 2;
					}
				}

				//tie
				if (winner == -1){
					winState = 1;
				}

				if (winState == 2){
					int winRecord = PlayerPrefs.GetInt("TotalWins");
					winRecord += 1;
					PlayerPrefs.SetInt("TotalWins",winRecord);
				}
			};
		} else {
			
			//TODO:Change based on team
			gameObject.tag = "Enemy";
			CapsuleCollider c = gameObject.AddComponent<CapsuleCollider> ();
			c.height = cc.height;
			c.radius = cc.radius;
		}

	}

	// Update is called once per frame
	void Update(){
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
				// camera movement
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

			// Player state machine
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

	/// <summary>
	/// Calculate attached character's horizontal speed
	/// </summary>
	/// <returns>Integer representing attached character's horizontal speed</returns>
	public int PlayerHSpeed(){
		showVel.x = playerVelocity.x;
		showVel.y = playerVelocity.z;
		return (int)Mathf.Floor(showVel.magnitude);
	}
	
	//////////////////////
	//**** Movement ****//
	//////////////////////

	/// <summary>
	/// Gets input direction from local player
	/// </summary>
	void SetMovementDir(){
		cmd.forwardmove = Input.GetAxisRaw ("Vertical");
		cmd.rightmove 	= Input.GetAxisRaw ("Horizontal");
	}

	/// <summary>
	/// Handles queueing next jump while in air
	/// </summary>
	void QueueJump(){
		if (Input.GetKeyDown (KeyCode.Space) && !queueJump) {
			queueJump = true;
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			queueJump = false;
		}
	}

	/// <summary>
	/// Calculates mid air velocity
	/// </summary>
	void AirMove(){
		Vector3 targetdir;
		float accel;

		float scale = CmdScale();

		SetMovementDir ();

		targetdir = new Vector3 (cmd.rightmove, 0, cmd.forwardmove);
		targetdir = transform.TransformDirection (targetdir);

		float targetspeed = targetdir.magnitude;
		targetspeed *= moveSpeed;

		targetdir.Normalize ();
		//moveDirectionNorm = targetdir;
		targetspeed *= scale;

		//aux air control
		float targetspeed2 = targetspeed;
		if (Vector3.Dot (playerVelocity, targetdir) < 0) {
			accel = airDeacceleration;
		} else {
			accel = airAcceleration;
		}

		// left or right speed
		if (cmd.forwardmove == 0 && cmd.rightmove != 0) {
			if (targetspeed > sideStrafeSpeed) {
				targetspeed = sideStrafeSpeed;
			}
			accel = sideStrafeAcceleration;
		}

		//Accelerate(targetdir, targetspeed, accel);
		if (airControl > 0) {
			AirControl(targetdir, targetspeed2);
		}

		//apply gravity
		playerVelocity.y -= gravity * Time.deltaTime;

	}

	/// <summary>
	/// Handles character aerial control
	/// </summary>
	/// <param name="targetdir">Character's requested direction</param>
	/// <param name="targetspeed">Character's requested speed</param>
	void AirControl(Vector3 targetdir, float targetspeed){
		float	zspeed;
		float	speed;
		float	dot;
		float	k;

		//cant control movement if not forward or backwards
		if (cmd.forwardmove == 0 || targetspeed == 0) {
			return;
		}

		zspeed = playerVelocity.y;
		playerVelocity.y = 0;

		//ported vector normalize
		speed = playerVelocity.magnitude;
		playerVelocity.Normalize ();

		dot = Vector3.Dot (playerVelocity, targetdir);
		k = 32;
		k *= airControl * dot * dot * Time.deltaTime;

		if (dot > 0) {
			playerVelocity.x = playerVelocity.x * speed + targetdir.x * k;
			playerVelocity.y = playerVelocity.y * speed + targetdir.y * k;
			playerVelocity.z = playerVelocity.z * speed + targetdir.z * k;

			playerVelocity.Normalize();
			//moveDirectionNorm = playerVelocity;
		}

		playerVelocity.x *= speed;
		playerVelocity.y = zspeed; // Note this line
		playerVelocity.z *= speed;

	}

	/// <summary>
	/// Calculates ground based movement, 
	/// applys character input and speed, 
	/// uses friction to decelerate character
	/// </summary>
	void GroundMove(){
		Vector3 targetdir;
		//Vector3 targetvel;

		if (!queueJump) {
			ApplyFriction(1.0f);
		} else {
			ApplyFriction(0);
		}

		//float scale = CmdScale();
	
		SetMovementDir();

		targetdir = new Vector3 (cmd.rightmove, 0, cmd.forwardmove);
		targetdir = transform.TransformDirection (targetdir);
		targetdir.Normalize ();
		//moveDirectionNorm = targetdir;

		float targetspeed = targetdir.magnitude;
		targetspeed *= moveSpeed;

		Accelerate(targetdir, targetspeed, runAcceleration);

		// Reset gravity velocity
		playerVelocity.y = 0;

		if (queueJump) {
			Jump (false);
		}
	}

	/// <summary>
	/// Handles movement as well as visual logic for character jumping
	/// </summary>
	/// <param name="midAir">Whether or not the jump occurred in mid-air</param>
	public void Jump(bool midAir){
		playerVelocity.y = jumpSpeed;
		queueJump = false;
		PlayJumpSound ();
		if (JumpEvent != null) {
			JumpEvent.Invoke (midAir);
		}
		networkObject.SendRpc (NetworkPlayerBehavior.RPC_MULTICAST__JUMP, BeardedManStudios.Forge.Networking.Receivers.All, midAir);
	}

	/// <summary>
	/// Applies friction to character velocity
	/// </summary>
	/// <param name="t">Friction scale</param>
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

	/// <summary>
	/// Accelerates character in a given direction with to a desired speed
	/// </summary>
	/// <param name="targetdir">The direction to apply the acceleration</param>
	/// <param name="targetspeed">The target speed to reach</param>
	/// <param name="accel">How quickly velocity is added</param>
	void Accelerate(Vector3 targetdir, float targetspeed, float accel){
		float addspeed;
		float accelspeed;
		float currentspeed;

		currentspeed = Vector3.Dot (playerVelocity, targetdir);
		addspeed = targetspeed - currentspeed;
		if (addspeed <= 0) {
			return;
		}
		accelspeed = accel * Time.deltaTime * targetspeed;
		if (accelspeed > addspeed) {
			accelspeed = addspeed;
		}

		playerVelocity.x += accelspeed * targetdir.x;
		playerVelocity.z += accelspeed * targetdir.z;
	}

	/// <summary>
	/// Gets total scale of max speed
	/// </summary>
	/// <returns>Percentage of max speed reached from 0 to 1</returns>
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

	/// <summary>
	/// Plays jumping sound
	/// </summary>
	void PlayJumpSound()
	{
		if (GetComponent<AudioSource> ().isPlaying)
			return;
		GetComponent<AudioSource> ().clip = jumpSounds [Random.Range (0, jumpSounds.Length)];
		GetComponent<AudioSource> ().Play ();
	}

	/// <summary>
	/// Handles character controller impacting another collider
	/// </summary>
	/// <param name="hit">Information on the collision</param>
	void OnControllerColliderHit(ControllerColliderHit hit) {
		floorAngle = Vector3.Angle (Vector3.up, hit.normal);
		playerVelocity = Vector3.ProjectOnPlane (playerVelocity, hit.normal);
	}

	////////////////////////////////////////
	///**** RPC Functions (Generated) ****//
	////////////////////////////////////////
	
	/// <summary>
	/// Notifies server that attached character that has taken damage
	/// </summary>
	/// <param name="args">RPC arguments to send to server for taking damage</param>
	public override void Server_TakeDamage(RpcArgs args){
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

	/// <summary>
	/// Notifies server that attached character has "Died"
	/// </summary>
	/// <param name="args">RPC arguments for the "Death" event</param>
	public override void Server_Death(RpcArgs args){
		if (networkObject.IsOwner) {	
			playerScoreStruct.deaths += 1;
			networkObject.death = playerScoreStruct.deaths;
		}

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
	
	/// <summary>
	/// Notifies server that attached character has gotten a "Kill"
	/// </summary>
	/// <param name="args">RPC arguments for "Kill" event</param>
	public override void Server_GetKill(RpcArgs args){
		playerScoreStruct.kills += 1;
		networkObject.kills = playerScoreStruct.kills;

		//Stat logging
		int killRecord = PlayerPrefs.GetInt ("HighestKills");
		float kdRecord = PlayerPrefs.GetFloat ("HighestKD");
		float curKD = GetKDRatio ();
		if (curKD > kdRecord) {
			PlayerPrefs.SetFloat ("HighestKD", curKD);
		}
		if (networkObject.kills > killRecord) {
			PlayerPrefs.SetInt ("HighestKills", networkObject.kills);
		}

		if (gamemodeRef.killsAreScore){
			networkObject.SendRpc(NetworkPlayerBehavior.RPC_CLIENT__ADD_SCORE, Receivers.Owner, gamemodeRef.scorePerKill);
		}
	}

	/// <summary>
	/// Notifies server to respawn attached player
	/// </summary>
	/// <param name="args">RPC arguments for "Respawn" event</param>
	public override void Server_Respawn(RpcArgs args){
		SERVER_isDead = false;
		networkObject.SendRpc (NetworkPlayerBehavior.RPC_MULTICAST__RESPAWN, BeardedManStudios.Forge.Networking.Receivers.All);
	}

	/// <summary>
	/// Notifies server to set the display name of the attached character
	/// </summary>
	/// <param name="args">RPC arguments for "Set Name" event, i.e. new display name</param>
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

	/// <summary>
	/// Notifies client of all other character names in the server
	/// </summary>
	/// <param name="args"></param>
	public override void Client_GetNames(RpcArgs args){
		string namesStr = args.GetNext<string> ();
		print (namesStr);
		Dictionary<uint,string> names =  Tools.JSON.DeserializeNameDict(namesStr);
		gam.ApplyNames (names);
	}


	/////////////////////////
	///**** Multicasts ****//
	/////////////////////////
	
	/// <summary>
	/// Notifies all clients that the attached player has taken damage
	/// </summary>
	/// <param name="args">RPC arguments for "On Damaged" event</param>
	public override void Multicast_TakeDamage(RpcArgs args){
		print ("Apply damage visuals");
	}
	
	/// <summary>
	/// Notifies all clients that the attached player has "Died"
	/// </summary>
	/// <param name="args">RPC arguments for "On Death" event</param>
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

	/// <summary>
	/// Notifies client of "Kill" received
	/// </summary>
	/// <param name="args">RPC arguments for "Kill Received" event</param>
	public override void Client_GetKill(RpcArgs args){
		string playerKilled = args.GetNext<string> ();

		print (System.String.Format("deaths: {0} | kills: {1}| score: {2}",playerScoreStruct.deaths, playerScoreStruct.kills, playerScoreStruct.score));

		if (GetKillEvent != null) GetKillEvent.Invoke((string)playerName, playerKilled);

		//TODO: Remove circular dependcies
		if (gamemodeRef.killsAreScore) {
			int scoreAmount = gamemodeRef.scorePerKill;
			networkObject.SendRpc (NetworkPlayerBehavior.RPC_CLIENT__ADD_SCORE, Receivers.Owner, scoreAmount);
		}
	}

	/// <summary>
	/// Notifies all clients of the attached character's gun firing
	/// </summary>
	/// <param name="args">RPC arguments for "Gun Fired" event</param>
	public override void Multicast_GunFired(RpcArgs args){
		print ("Apply Shotgun visual");
	}

	/// <summary>
	/// Notifies all clients of character respawning
	/// </summary>
	/// <param name="args">RPC arguments for "On Respawn" event</param>
	public override void Multicast_Respawn(RpcArgs args){
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

	/// <summary>
	/// Notifies client of cleared scoreboard values
	/// </summary>
	/// <param name="args">RPC arguments for "Scoreboard Cleared" event</param>
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

	/// <summary>
	/// Notifies client that "Score" has been added to the attached character
	/// </summary>
	/// <param name="args">RPC arguments for "Score Added" event</param>
	public override void Client_AddScore(RpcArgs args){
		int amount = args.GetNext<int> ();
		object[] scoreArgs = {amount, networkObject.team}; 
		gamemodeRef.networkObject.SendRpc (NetworkGamemodeObjectBehavior.RPC_CLIENT__ADD_SCORE, Receivers.Owner, scoreArgs);
	}

	/// <summary>
	/// Notifies all clients of the attached character jumping
	/// </summary>
	/// <param name="args">RPC arguments for "On Jumped" event</param>
	public override void Multicast_Jump(RpcArgs args){
		if (networkObject.IsOwner) {
		}
		bool midAir = args.GetNext<bool> ();
		if (JumpEvent != null) {
			JumpEvent.Invoke (midAir);
		}
	}

	/// <summary>
	/// Notifies Client of a received character's customization
	/// </summary>
	/// <param name="args">RPC arguments for "Customization Received" event</param>
	public override void Client_ReceiveCustomization(RpcArgs args){
		string data = args.GetNext<string>();
		uint playerID = args.GetNext<uint> ();
		print(data + ":  " + playerID.ToString());
		PlayerMovement target = gam.GetNetworkPlayerFromID (playerID);
		target.GetComponentInChildren<CharacterPaint> (true).Deserialize (data);
	}

	/// <summary>
	/// Notifies all clients of attached character's customization
	/// </summary>
	/// <param name="args">RPC arguments for "Customization Sent" event</param>
	public override void Multicast_SendCustomization(RpcArgs args){
		string data = args.GetNext<string>();
		uint playerID = args.GetNext<uint> ();

		print(data + ":  " + playerID.ToString());

		GetComponentInChildren<CharacterPaint> (true).Deserialize (data);

		PlayerMovement us = gam.GetOwnedPlayer ();
		string cust = us.GetComponentInChildren<CharacterPaint> (true).Serialize ();
		object[] customizationArgs = { cust, us.networkObject.NetworkId };
		networkObject.SendRpc (NetworkPlayerBehavior.RPC_CLIENT__RECEIVE_CUSTOMIZATION, Receivers.Target, customizationArgs);
		
	}

	/// <summary>
	/// Notifies all clients of attached character's grappling hook being fired
	/// </summary>
	/// <param name="args">RPC arguments for "On Grapple Fire" event</param>
	public override void Multicast_GrappleFired (RpcArgs args)
	{
		throw new System.NotImplementedException ();
	}

	/// <summary>
	/// Notifies Client of received impact force
	/// </summary>
	/// <param name="args">RPC arguments for "Receive Impact" event</param>
	public override void Client_AddForce (RpcArgs args)
	{
		Vector3 dir = args.GetNext<Vector3> ();
		float magnitude = args.GetNext<float> ();
		bool isImpulse = args.GetNext<bool> ();
		GetComponent<ReceiveImpact> ().AddImpact(dir,magnitude,true);
	}

	/// <summary>
	/// Calculates "Kills" to "Deaths" ratio
	/// </summary>
	/// <returns>Kill/Death ratio</returns>
	public float GetKDRatio(){
		//prevent divide by zero, no NaNa's
		int deathVisual = networkObject.death > 1 ? networkObject.death : 1;
		return (float)networkObject.kills / deathVisual;
	}

	/// <summary>
	/// Handles attached character entering trigger volumes
	/// </summary>
	/// <param name="other">Trigger volume that was entered</param>
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
