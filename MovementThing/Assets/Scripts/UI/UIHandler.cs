using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	void UpdateGamemodeText(){
		if (gameMode != null) {
			gameModeType.text = gameMode.gamemodeTypes [gameMode.networkObject.gamemodeType];
			gameModeTypeShadow.text = gameModeType.text;
		}
	}

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

	void UpdatePlayerStatus(){
		healthnumber.text = "" + ph.curHealth;
		ammoNumber.text = "" + sh.curAmmo;
		healthShadow.text = healthnumber.text;
		ammoShadow.text = ammoNumber.text;
		healthBar.value = ((float)ph.curHealth/ph.maxHealth);
		mph.text = "" + pm.PlayerHSpeed();
		mphShadow.text = mph.text;	
	}
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

	public void ToggleScoreboard (bool showBoard){
		sb.ShowBoard(showBoard);
	}
	public void SetGroupsActive(bool matchInfo = true, bool playerStatus = true, bool speedGroup = true, bool gunCursor = true){
		matchInfoGroup.SetActive (matchInfo);
		playerStatusGroup.SetActive (playerStatus);
		playerSpeedGroup.SetActive(speedGroup);
		gunCursorGroup.SetActive (gunCursor);
	}
}
