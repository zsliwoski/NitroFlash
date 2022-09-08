using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MatchEndCard : MonoBehaviour {
	public UI_CountUpTo killsCountUp;
	public UI_CountUpTo kdCountUp;
	public UI_CountUpTo scoreCountUp;

	public Text killsPlayerName;
	public Text kdPlayerName;
	public Text scorePlayerName;

	public Animator mvpCardAnimation;
	public Animator kdCardAnimation;
	public Animator killCardAnimation;

	public Animator winScreenAnimator;
	PlayerMovement mainPlayer;
	UIHandler playerUI;
	GameController gc;
	GamemodeBase gmb;

	// Use this for initialization
	void Start () {
		gc = FindObjectOfType<GameController> ();
		gmb = FindObjectOfType<GamemodeBase> ();
		gmb.RoundStartEvent += CloseEndScreen;
		gmb.RoundEndEvent += DisplayEndScreen;
	}

	public void CloseEndScreen (){
		if (playerUI == null) {
			playerUI = FindObjectOfType<UIHandler> ();
		}
		winScreenAnimator.SetInteger("WinState",-1);
		winScreenAnimator.Play("showNone");
		mainPlayer.SetCursorLock (true);
		mainPlayer.FreezeMouseInput (false);
		//called with no args, re-enables everything
		playerUI.SetGroupsActive ();
	}

	public void SetOwner(PlayerMovement pm){
		mainPlayer = pm;
	}

	public void DisplayEndScreen(int winner){
		if (playerUI == null) {
			playerUI = FindObjectOfType<UIHandler> ();
		}
		mainPlayer.SetCursorLock (false);
		mainPlayer.FreezeMouseInput (true);
		playerUI.SetGroupsActive (playerStatus: false, speedGroup: false, gunCursor: false);
		SetupAwardCards ();

		//TODO: add this later
		//SetupTopScoreCards ();
		int winState = 0; //0 is losing

		if (gmb.usingTeams) {
			//Team Win
			if ((int)mainPlayer.networkObject.team == winner) {
				winState = 2;
			}

		} else {
			//FFA Win
			if ((int)mainPlayer.networkObject.NetworkId == winner) {
				winState = 2;
			}
		}

		//Tied
		if (winner == -1) {
			winState = 1;
		}

		winScreenAnimator.SetInteger("WinState",winState);
	}

	/*public void SetupTopScoreCards(){
	
	}*/
	public void ShowAwardCards(){
		mvpCardAnimation.Play ("cardStepIn");
		kdCardAnimation.Play ("cardStepIn");	
		killCardAnimation.Play ("cardStepIn");	
	}

	void SetupAwardCards(){
		PlayerMovement mvp = gc.GetPlayerWithMost (GameController.PlayerOrderBy.SCORE);
		PlayerMovement kder = gc.GetPlayerWithMost (GameController.PlayerOrderBy.KD_RATIO);
		PlayerMovement killer = gc.GetPlayerWithMost (GameController.PlayerOrderBy.KILLS);

		scorePlayerName.text = mvp.playerName;
		kdPlayerName.text = kder.playerName;
		killsPlayerName.text = killer.playerName;

		scoreCountUp.SetTarget (mvp.networkObject.score);
		kdCountUp.SetTarget (kder.GetKDRatio());
		killsCountUp.SetTarget (killer.networkObject.kills);
	}
}
