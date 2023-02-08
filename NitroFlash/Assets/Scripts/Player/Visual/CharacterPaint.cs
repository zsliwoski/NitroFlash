using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing logic related to character customization
/// </summary>
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
	public bool usesFace = true;
	public bool usesBody = true;

	// Use this for initialization
	void Start(){
		if (loadOnStart) {
			Deserialize (Serialize());
		}
	}

	/// <summary>
	/// Changes which face texture to use on the attached character
	/// </summary>
	/// <param name="faceNum">The index of the chosen face</param>
	public void SetFace(int faceNum){
		if (faceNum + 1 > faceTextures.Length || faceTextures.Length == 0) {
			Debug.LogWarning ("Invalid character face specified : " + gameObject.name);
		} else {
			curFaceTex = faceNum;
			model.materials [1].SetTexture (faceTexProp, faceTextures [faceNum]);
		}
	}

	/// <summary>
	/// Used to apply a set of colors to the attached character
	/// </summary>
	/// <param name="bodyColors">Dictionary of the body color layers</param>
	/// <param name="faceColors">Dictionary of the facey color layers</param>
	void ApplyColors(Dictionary<string,string> bodyColors, Dictionary<string,string> faceColors){
		foreach (string property in bodyColPropNames) {
			SetColor (property, bodyColors [property]);
		}
		foreach (string property in faceColPropNames) {
			SetColor (property, faceColors [property], false);
		}
	}

	/// <summary>
	/// Used to set the color of a specified material property
	/// </summary>
	/// <param name="name">Name of the property to set</param>
	/// <param name="colStr">Color to use</param>
	/// <param name="isBody">Whether to check body material property</param>
	public void SetColor(string name, string colStr, bool isBody = true){
		SetColor (name, ParseColorString (colStr), isBody);
	}

	/// <summary>
	/// Used to set the color of a specified material property
	/// </summary>
	/// <param name="name">Name of the property to set</param>
	/// <param name="col">Color to use</param>
	/// <param name="isBody">Whether to check body material property</param>
	public void SetColor(string name, Color col, bool isBody = true){
		if (isBody && usesBody) {
			model.materials [0].SetColor (name, col);
		} else if (usesFace) {
			model.materials [1].SetColor (name, col);
		}
	}

	/// <summary>
	/// Helper method to parse color strings into a usable type
	/// </summary>
	/// <param name="inStr">String to convert</param>
	/// <returns>Native Color object</returns>
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

	/// <summary>
	/// Used to locally serialize the attached character's current color choice
	/// </summary>
	public void SaveCurrentColors(){
		foreach (string property in bodyColPropNames) {
			PlayerPrefs.SetString ("$body$" + property, model.materials [0].GetColor (property).ToString ());
			print(property + ":" + model.materials [0].GetColor (property).ToString ());
		}
		foreach (string property in faceColPropNames) {
			PlayerPrefs.SetString ("$face$" + property, model.materials [1].GetColor (property).ToString ());
		}
	}

	/// <summary>
	/// Gets the Color of the specified material property or "Color Channel"
	/// </summary>
	/// <param name="channel">Material property to check</param>
	/// <param name="isFace">Whether or not to check the face</param>
	/// <returns>A native Color object for the given property</returns>
	public Color GetChannelColor(string channel, bool isFace){
		if (isFace) {
			return model.materials [1].GetColor (channel);
		} else {
			return model.materials [0].GetColor (channel);
		}
	}

	/// <summary>
	/// Loads the locally saved colors to the attached character
	/// </summary>
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

	/// <summary>
	/// Aggregates all custom material properties of the attached character into a portable format
	/// </summary>
	/// <returns>A String formatted dictionary of all custom material properties for the attached character</returns>
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

	/// <summary>
	/// Sets all custom material properties of the attached character from a serialized string
	/// </summary>
	/// <param name="stringToApply">A String formatted dictionary of all custom material properties for the attached character</param>
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
		if (usesFace) {
			SetFace (faceID);
		}
		ApplyColors (bodyColors, faceColors);
	}

	/// <summary>
	/// Applies locally saved face to attached character
	/// </summary>
	public void LoadFace(){
		curFaceTex = PlayerPrefs.GetInt ("$face$_OverlayTex");
		SetFace(curFaceTex);
	}

	/// <summary>
	/// Saves applied face to local system
	/// </summary>
	public void SaveFace(){
		PlayerPrefs.SetInt ("$face$_OverlayTex", curFaceTex);
	}
}
