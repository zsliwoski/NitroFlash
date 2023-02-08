using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This UI class controls all the server hosting options
/// </summary>
public class UI_HostWindow : MonoBehaviour {
	[System.Serializable]
	public struct MapOption
	{
		public string name;
		public string mapID;
	}
	public MultiplayerMenu mulm;
	public List<MapOption> mapOptions;
	public int maxPlayers = 16;
	public int minPlayers = 2;
	public UI_OptionCycler mapChooser;
	public UI_OptionCycler playerCount;
	public InputField portInput;
	public UI_CheckBox lanCheck;

	// Use this for initialization
	void Start () {
		foreach (MapOption mo in mapOptions) {
			mapChooser.AddOption (mo.mapID, mo.name);
		}
		for (int i = minPlayers; i <= maxPlayers; i++) {
			playerCount.AddOption (i.ToString(), i.ToString ());
		}
		mapChooser.SetCurrentOption (0);
		playerCount.SetCurrentOption (0);
	}

	/// <summary>
	/// Attempt to host the server given all current options
	/// </summary>	
	public void TryHost(){
		mulm.Host (int.Parse(playerCount.currentOptionID), portInput.text);
	}
}
