using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;

public class MainMenuUIHandler : MonoBehaviour {
	public Text nameDisplay;

	// Use this for initialization
	void Start () {
		string savedName = PlayerPrefs.GetString ("PlayerName");
		if (savedName == "") {
			savedName = NameGenerator.GenerateName ();
			PlayerPrefs.SetString ("PlayerName", savedName);
		}
		nameDisplay.text = savedName;
	}

	public void GetNewName(){
		string funName = NameGenerator.GenerateName ();
		PlayerPrefs.SetString ("PlayerName", funName);
		nameDisplay.text = funName;
	}

	public void QuitPressed(){
		Application.Quit ();
	}
}
