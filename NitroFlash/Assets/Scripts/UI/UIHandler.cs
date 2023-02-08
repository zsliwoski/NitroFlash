using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class controls the active gameplay UI and player HUD
/// </summary>
public class UIHandler : MonoBehaviour {

	public PlayerHealth ph;
	public PlayerMovement pm;

	public Shotgun sh;
	public Grapple gr;
	public Animator grappleCursor;

	public Text ammoNumber;
	public Text ammoShadow;
	public Text healthnumber;
	public Text healthShadow;

	public Text mph;
	public Text mphShadow;

	public Slider healthBar;
	public ScoreBoard sb;

	public Text timer;
	public Text timerShadow;
	public Text gameModeType;
	public Text gameModeTypeShadow;

	public GameObject matchInfoGroup;
	public GameObject playerStatusGroup;
	public GameObject playerSpeedGroup;
	public GameObject gunCursorGroup;

	GamemodeBase gameMode;

	Vector2 showVel;

	// Update is called once per frame
	void Update () {
		UpdatePlayerStatus ();
		UpdateGrappleIcon ();
		if (gameMode != null) {
			UpdateGamemodeText ();
			UpdateTimer ();
		} else {
			gameMode = FindObjectOfType<GamemodeBase> ();
		}
	}

	/// <summary>
	/// Updates gamemode display name at the top of the player HUD
	/// </summary>
	void UpdateGamemodeText(){
		if (gameMode != null) {
			gameModeType.text = gameMode.gamemodeTypes [gameMode.networkObject.gamemodeType];
			gameModeTypeShadow.text = gameModeType.text;
		}
	}

	/// <summary>
	/// Updates gamemode timer at the top of the player HUD
	/// </summary>
	void UpdateTimer(){
		if (gameMode != null) {
			int t = gameMode.networkObject.timeRunning;
			int minutes = t / 60;
			int seconds = t % 60;
			string pad = "";
			if (seconds < 10)
				pad = "0"; 
			timer.text = minutes + ":" + pad + seconds;
			timerShadow.text = timer.text;
		}
	}

	/// <summary>
	/// Updates all widgets related to local player's current character status
	/// </summary>
	void UpdatePlayerStatus(){
		healthnumber.text = "" + ph.curHealth;
		ammoNumber.text = "" + sh.curAmmo;
		healthShadow.text = healthnumber.text;
		ammoShadow.text = ammoNumber.text;
		healthBar.value = ((float)ph.curHealth/ph.maxHealth);
		mph.text = "" + pm.PlayerHSpeed();
		mphShadow.text = mph.text;	
	}

	/// <summary>
	/// Checks current grapple state (i.e. valid/invalid grapple, grapple held, etc...)
	/// </summary>
	void UpdateGrappleIcon(){
		int gState = 0;
		if (gr.validGrapple){
			gState = 1;
		}
		if (gr.held){
			gState = 2;
		}
		grappleCursor.SetInteger("GrappleState", gState);
	}

	/// <summary>
	/// Sets visibility of the match scoreboard
	/// </summary>
	/// <param name="showBoard">Desired visibility of match scoreboard</param>
	public void ToggleScoreboard (bool showBoard){
		sb.ShowBoard(showBoard);
	}

	/// <summary>
	/// Sets visibility of UI groups, useful for derendering certain elements during gameplay events
	/// </summary>
	/// <param name="matchInfo">Sets visibility of the "Match Info" UI group</param>
	/// <param name="playerStatus">Sets visibility of the "Player Status" UI group</param>
	/// <param name="speedGroup">Sets visibility of the "Speed" UI group</param>
	/// <param name="gunCursor">Sets visibility of the "Gun Cursor" UI group</param>
	public void SetGroupsActive(bool matchInfo = true, bool playerStatus = true, bool speedGroup = true, bool gunCursor = true){
		matchInfoGroup.SetActive (matchInfo);
		playerStatusGroup.SetActive (playerStatus);
		playerSpeedGroup.SetActive(speedGroup);
		gunCursorGroup.SetActive (gunCursor);
	}
}
