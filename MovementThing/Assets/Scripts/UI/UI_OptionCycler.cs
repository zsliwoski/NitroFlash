using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_OptionCycler : MonoBehaviour {
	public delegate void OptionChangedDelegate(string optionID, string displayText);
	public event OptionChangedDelegate OptionChanged;

	[System.Serializable]
	public struct OptionCyclerOption{
		public string displayName;
		public string key;
		public OptionCyclerOption(string key,string displayName){
			this.displayName = displayName;
			this.key = key;
		}
	}
	public List<OptionCyclerOption> optionsList;
	public Text optionDisplay;
	int curOption = 0;

	// Use this for initialization
	void Start () {
		SetCurrentOption (curOption);
	}
	public void AddOption(string optionID, string displayName){
		optionsList.Add (new OptionCyclerOption (optionID, displayName));
	}

	public void AddMultipleOptions(Dictionary<string,string> options){
		foreach (var item in options){
			AddOption (item.Key, item.Value);
		}
	}

	public void SetCurrentOption(int index){
		int finalSelection = index;
		int count = optionsList.Count;
		if (count == 0) {
			Debug.LogWarning ("No Options for Option Cycler: " + gameObject.name);
			return;
		}
		//index too high
		if (count < finalSelection + 1) {
			finalSelection = 0;
		}
		//index too low
		if (finalSelection < 0){
			finalSelection = count - 1;
		}
		curOption = finalSelection;
		optionDisplay.text = optionsList [curOption].displayName;
		if (OptionChanged != null) {
			OptionChanged.Invoke (optionsList [curOption].key, optionsList [curOption].displayName);
		}
	}
	public void OptionDecrement(){
		SetCurrentOption (curOption - 1);
	}
	public void OptionIncrement(){
		SetCurrentOption (curOption + 1);
	}
}
