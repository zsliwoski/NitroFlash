using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_CustomizationWindow : MonoBehaviour {
	//TODO: One improvement to this is to decouple the body dict properties from this class
	//and rely only on character paint for facetexture/customization properties
	[System.Serializable]
	public struct CustomizationFaceChoice
	{
		public string key;
		public string displayName;
		public Texture2D tex;
	}
	//public List<CustomizationFaceChoice> userFaceChoiceList = new List<CustomizationFaceChoice>();
	Dictionary<string, string> faceChoicesDict = new Dictionary<string, string>{
		{"0","Poggers"},
		{"1","Yaga"},
		{"2","Bingo"},
	};
	Dictionary<string,string> bodyColorDict = new Dictionary<string, string>{
		{"_Color", "Background Color"},
		{"_PrimaryColor", "First Color"},
		{"_SecondaryColor", "Second Color"},
		{"_TertiaryColor", "Third Color"},
	};
	Dictionary<string,string> faceColorDict = new Dictionary<string,string>{
		{"_PrimaryColor", "Screen Color"},
		{"_OverlayColor", "Face Color"},
	};
	public UI_OptionCycler bodyColorOptions;
	public UI_OptionCycler faceColorOptions;
	public UI_OptionCycler faceTypeOptions;

	public CharacterPaint charPaint;
	public UI_ColorAdjuster bodyColAdjuster;
	public UI_ColorAdjuster faceColAdjuster;

	public Button applyButton;

	string curBodyColorChannel = "";
	string curFaceColorChannel = "";
	string curFaceID = "";

	bool changesMade = false;
	bool defaultsLoaded = false;
	// Use this for initialization
	void Start () {
		/*foreach (CustomizationFaceChoice cfc in userFaceChoiceList) {
			faceChoicesDict.Add (cfc.key, cfc.displayName);
		}*/
		bodyColorOptions.AddMultipleOptions (bodyColorDict);
		faceColorOptions.AddMultipleOptions (faceColorDict);
		faceTypeOptions.AddMultipleOptions (faceChoicesDict);

		bodyColAdjuster.ColorChangeEvent += BodyColorChanged;
		faceColAdjuster.ColorChangeEvent += FaceColorChanged;

		bodyColorOptions.OptionChanged += (string optionID, string displayText) => {
			curBodyColorChannel = optionID;
			bodyColAdjuster.SetColor(charPaint.GetChannelColor(curBodyColorChannel, false));
		}; 
		faceColorOptions.OptionChanged += (string optionID, string displayText) => {
			curFaceColorChannel = optionID;
			faceColAdjuster.SetColor(charPaint.GetChannelColor(curFaceColorChannel, true));
		}; 
		faceTypeOptions.OptionChanged += (string optionID, string displayText) => {
			curFaceID = optionID;
			int faceIDResult = 0;
			changesMade = true;
			applyButton.interactable = changesMade;
			if (int.TryParse(curFaceID, out faceIDResult)){
				charPaint.SetFace(faceIDResult);
			}else{
				Debug.LogWarning("Warning! Improper ID specified, falling back");
				charPaint.SetFace(0);
			}
		};
		//hacky race condition, bleh
		charPaint.CharacterLoadEvent += SetDefaults;
		if (defaultsLoaded == false) {
			SetDefaults ();
		}
	}
	public void SaveCustomization(){
		charPaint.SaveCurrentColors ();
		charPaint.SaveFace ();
		changesMade = false;
		applyButton.interactable = changesMade;
	}
	void SetDefaults(){
		if (defaultsLoaded) {
			return;
		}
		defaultsLoaded = true;
		bodyColorOptions.SetCurrentOption (0);
		faceColorOptions.SetCurrentOption (0);
		bodyColAdjuster.SetColor(charPaint.GetChannelColor("_Color", false));
		faceColAdjuster.SetColor(charPaint.GetChannelColor("_PrimaryColor", true));
		//Hacky, but it works
		int curFaceTex = PlayerPrefs.GetInt ("$face$_OverlayTex");
		faceTypeOptions.SetCurrentOption (curFaceTex);
		applyButton.interactable = false;
	}
	void BodyColorChanged(Color col){
		charPaint.SetColor (curBodyColorChannel, col, true);
		changesMade = true;
		applyButton.interactable = changesMade;
	}
	void FaceColorChanged(Color col){
		charPaint.SetColor (curFaceColorChannel, col, false);
		changesMade = true;
		applyButton.interactable = changesMade;
	}
}
