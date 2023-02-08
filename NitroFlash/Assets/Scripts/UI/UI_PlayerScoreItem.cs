using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;

/// <summary>
/// UI class that handles displaying a player's score card
/// </summary>
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

	/// <summary>
	/// Sets the player to observe the stats of
	/// </summary>
	/// <param name="playerObserved">The player to observe</param>
	public void SetObservedPlayer (PlayerMovement playerObserved){
		pm = playerObserved;
	}

	/// <summary>
	/// Gets the player whose stats are observed
	/// </summary>
	/// <returns>The observed player</returns>
	public PlayerMovement GetObservedPlayer(){
		return pm;
	}
}
