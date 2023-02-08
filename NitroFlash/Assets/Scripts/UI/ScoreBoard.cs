using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles updating the scoreboard during a match
/// </summary>
public class ScoreBoard : MonoBehaviour {

	public GameObject contentWindow;
	public GameObject visibilityWindow;
	public GameObject scoreCardPrefab;
	public GameController gc;
	public List<UI_PlayerScoreItem> scoreCardItems;

	// Use this for initialization
	void Start () {
		scoreCardItems = new List<UI_PlayerScoreItem> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (gc == null) {
			TryBindGameController ();
		}
	}

	/// <summary>
	/// Attempts to get game controller and bind events
	/// </summary>
	public void TryBindGameController(){
		gc = FindObjectOfType<GameController> ();
		if (gc != null) {
			foreach (PlayerMovement p in gc.playerObjects) {
				AddCard (p);
			}
			gc.PlayerJoinRosterEvent += (PlayerMovement player) => (AddCard(player));
			gc.PlayerLeaveRosterEvent += (PlayerMovement player) => (RemoveCard(player));
		}
	}

	/// <summary>
	/// Sets the visibility of the scoreboard
	/// </summary>
	/// <param name="visible">Whether or not to show the scoreboard</param>
	public void ShowBoard(bool visible){
		visibilityWindow.SetActive(visible);
	}
	void RemoveCard(PlayerMovement removalPlayer){
		UI_PlayerScoreItem temp = null;
		foreach (UI_PlayerScoreItem item in scoreCardItems)
		{
			if (item.GetObservedPlayer() == removalPlayer){
				temp = item;
			}
		}
		if (temp != null) {
			scoreCardItems.Remove (temp);
			Destroy (temp);
		}
	}

	/// <summary>
	/// Clears all active player cards from the scoreboard
	/// </summary>
	void ClearCards(){
		//TODO: Implement
	}

	/// <summary>
	/// Adds a player card to the scoreboard
	/// </summary>
	/// <param name="observedPlayer">The character to observe</param>
	public void AddCard(PlayerMovement observedPlayer){
		GameObject newCard = Instantiate (scoreCardPrefab, contentWindow.transform);
		UI_PlayerScoreItem cardInfo = newCard.GetComponent<UI_PlayerScoreItem> ();
		cardInfo.SetObservedPlayer (observedPlayer);
		scoreCardItems.Add(cardInfo);
	}

	/// <summary>
	/// Sorts playercards by score
	/// </summary>
	void SortScores(){
		//TODO: Implement
	}
}
