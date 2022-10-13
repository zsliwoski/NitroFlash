using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPaint : MonoBehaviour {
	public delegate void CharacterSerializeDelegate();
	public event CharacterSerializeDelegate CharacterLoadEvent;
	string[] bodyColPropNames = {
		"_Color",
		"_PrimaryColor",
		"_SecondaryColor",
		"_TertiaryColor"
	};
	string faceTexProp = "_OverlayTex";

	string[] faceColPropNames = {
		"_PrimaryColor",
		"_OverlayColor"
	};

	public Renderer model;
	public Texture2D[] faceTextures;
	public int curFaceTex = 0;
	public bool loadOnStart = false;
	void Start(){
		if (loadOnStart) {
			Deserialize (Serialize());
		}
	}

	public void SetFace(int faceNum){
		if (faceNum + 1 > faceTextures.Length || faceTextures.Length == 0) {
			Debug.LogWarning ("Invalid character face specified : " + gameObject.name);
		} else {
			curFaceTex = faceNum;
			model.materials [1].SetTexture (faceTexProp, faceTextures [faceNum]);
		}
	}

	void ApplyColors(Dictionary<string,string> bodyColors, Dictionary<string,string> faceColors){
		foreach (string property in bodyColPropNames) {
			SetColor (property, bodyColors [property]);
		}
		foreach (string property in faceColPropNames) {
			SetColor (property, faceColors [property], false);
		}
	}

	public void SetColor(string name, string colStr, bool isBody = true){
		SetColor (name, ParseColorString (colStr), isBody);
	}

	public void SetColor(string name, Color col, bool isBody = true){
		if (isBody) {
			model.materials [0].SetColor (name, col);
		} else {
			model.materials [1].SetColor (name, col);
		}
	}

	Color ParseColorString(string inStr){
		try{
			string colorStr = inStr;
			string[] splitter = {", "};
			string[] rgba = colorStr.Substring (5, colorStr.Length - 6).Split(splitter,System.StringSplitOptions.RemoveEmptyEntries);
			return new Color (float.Parse (rgba [0]), float.Parse (rgba [1]), float.Parse (rgba [2]), float.Parse (rgba [3]));
		}catch{
			print ("Couldn't parse");
		}
		return Color.white;
	}

	public void SaveCurrentColors(){
		foreach (string property in bodyColPropNames) {
			PlayerPrefs.SetString ("$body$" + property, model.materials [0].GetColor (property).ToString ());
			print(property + ":" + model.materials [0].GetColor (property).ToString ());
		}
		foreach (string property in faceColPropNames) {
			PlayerPrefs.SetString ("$face$" + property, model.materials [1].GetColor (property).ToString ());
		}
	}

	public Color GetChannelColor(string channel, bool isFace){
		if (isFace) {
			return model.materials [1].GetColor (channel);
		} else {
			return model.materials [0].GetColor (channel);
		}
	}

	public void LoadSavedColors(){
		Dictionary<string,string> bodyColors = new Dictionary<string, string>();
		Dictionary<string,string> faceColors = new Dictionary<string, string>();

		foreach (string property in bodyColPropNames) {
			string prop = PlayerPrefs.GetString ("$body$" + property).Replace("$body$","");
			bodyColors.Add (property, prop);
		}
		foreach (string property in faceColPropNames) {
			string prop = PlayerPrefs.GetString ("$face$" + property).Replace("$face$","");
			faceColors.Add (property, prop);
		}
		ApplyColors (bodyColors, faceColors);
		if (CharacterLoadEvent != null) {
			CharacterLoadEvent.Invoke ();
		}
	}

	public string Serialize(){
		string serial = "";

		foreach (string property in bodyColPropNames) {
			string prop = PlayerPrefs.GetString ("$body$" + property).Replace("$body$","");
			serial += prop + '|';
		}

		foreach (string property in faceColPropNames) {
			string prop = PlayerPrefs.GetString ("$face$" + property).Replace ("$face$", "");
			serial += prop + '|';
		}

		serial += PlayerPrefs.GetInt ("$face$_OverlayTex");
		return serial;
	}

	public void Deserialize(string stringToApply){
		Dictionary<string,string> bodyColors = new Dictionary<string, string>();
		Dictionary<string,string> faceColors = new Dictionary<string, string>();

		string[] dataStrings = stringToApply.Split ('|');
		int cursorPos = 0;

		foreach (string property in bodyColPropNames) {
			bodyColors.Add (property, dataStrings[cursorPos]);
			cursorPos++;
		}

		foreach (string property in faceColPropNames) {
			faceColors.Add (property, dataStrings[cursorPos]);
			cursorPos++;
		}

		int faceID = int.Parse(dataStrings[cursorPos]);
		SetFace (faceID);
		ApplyColors (bodyColors, faceColors);
	}

	public void LoadFace(){
		curFaceTex = PlayerPrefs.GetInt ("$face$_OverlayTex");
		SetFace(curFaceTex);
	}

	public void SaveFace(){
		PlayerPrefs.SetInt ("$face$_OverlayTex", curFaceTex);
	}
}
