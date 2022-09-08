using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;
using UnityEngine.UI;
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
	void ClearCards(){
		
	}
	public void AddCard(PlayerMovement observedPlayer){
		GameObject newCard = Instantiate (scoreCardPrefab, contentWindow.transform);
		UI_PlayerScoreItem cardInfo = newCard.GetComponent<UI_PlayerScoreItem> ();
		cardInfo.SetObservedPlayer (observedPlayer);
		scoreCardItems.Add(cardInfo);
	}
	void SortScores(){
	
	}
}
