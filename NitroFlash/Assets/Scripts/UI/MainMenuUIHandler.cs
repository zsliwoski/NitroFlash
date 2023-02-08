using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;

/// <summary>
/// This class controls all UI in the main menu and manages screen state
/// </summary>
public class MainMenuUIHandler : MonoBehaviour {
	public enum MenuType{
		TITLE,
		SERVERS,
		HOSTGAME,
		CUSTOMIZE,
		SETTINGS,
		STATS,
		FINDGAME
	}

	[System.Serializable]
	public struct MenuScreen{
		public MenuType screenID;
		public GameObject screenObject;
	};
	public Stack<MenuScreen> navStack;
	public MenuType curScreen;
	public GameObject curScreenObject;

	public MenuScreen[] menuScreens;
	public Text nameDisplay;

	private Animator currentScreenAnim;

	// Use this for initialization
	void Start () {
		navStack = new Stack<MenuScreen> ();
		navStack.Push (menuScreens [0]);
		string savedName = PlayerPrefs.GetString ("PlayerName");
		if (savedName == "") {
			savedName = NameGenerator.GenerateName ();
			PlayerPrefs.SetString ("PlayerName", savedName);
		}
		nameDisplay.text = savedName;
	}

	/// <summary>
	/// Handles transitioning current screen to a new one
	/// </summary>
	/// <param name="toMenu">String representing the new screen</param>
	public void TransitionMenu(string toMenu){
		MenuType toMenuEnum = (MenuType)System.Enum.Parse (typeof(MenuType), toMenu);
		TransitionMenu (toMenuEnum);
	}

	/// <summary>
	/// Handles transitioning current screen to a new one
	/// </summary>
	/// <param name="toMenu">Enum representing the new screen</param>
	/// <param name="pushToStack">Whether or not to push the screen to the navigation stack</param>
	public void TransitionMenu(MenuType toMenu, bool pushToStack = true){
		if (toMenu == curScreen) {
			Debug.LogWarning ("Attempt to transition to current screen: " + toMenu.ToString());
			return;
		}

		GameObject toScreen = null;

		//temp, find better solution than expensive lookup
		foreach (MenuScreen ms in menuScreens) {
			if (ms.screenID == toMenu) {
				toScreen = ms.screenObject;
				if (pushToStack) {
					navStack.Push (ms);
				}
				break;
			}
		}
		if (toScreen == null) {
			Debug.LogWarning ("No screen found: " + toMenu.ToString());
			return;
		}
		//TODO: make this a pretty animation transition
		//curScreenObject.SetActive (false);
		curScreenObject.GetComponent<Animator> ().CrossFade ("TransitionFrom", 0.1f);
		toScreen.GetComponent<Animator> ().CrossFade ("TransitionTo", 0.1f);
		//toScreen.SetActive (true);
		curScreenObject = toScreen;
		curScreen = toMenu;
	}

	/// <summary>
	/// Handles transitioning the current screen to the previous one
	/// </summary>
	public void NavigateBack(){
		navStack.Pop ();
		TransitionMenu(navStack.Peek().screenID, false);
	}

	/// <summary>
	/// Generates and saves a new name for the local player
	/// </summary>
	public void GetNewName(){
		string funName = NameGenerator.GenerateName ();
		PlayerPrefs.SetString ("PlayerName", funName);
		nameDisplay.text = funName;
	}

	/// <summary>
	/// Closes the application
	/// </summary>
	public void QuitPressed(){
		Application.Quit ();
	}
}
