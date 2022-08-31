using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;

public class UI_PlayerScoreItem : MonoBehaviour {
	public Text nameText;
	public Text scoreText;
	public Text killsText;
	public Text deathsText;
	PlayerMovement pm;

	// Update is called once per frame
	void Update () {
		if (pm != null) {
			nameText.text = pm.playerName;
			scoreText.text = pm.networkObject.score.ToString();
			killsText.text = pm.networkObject.kills.ToString();
			deathsText.text = pm.networkObject.death.ToString();
		}
	}
	public void SetObservedPlayer (PlayerMovement playerObserved){
		pm = playerObserved;
	}
	public PlayerMovement GetObservedPlayer(){
		return pm;
	}
}
