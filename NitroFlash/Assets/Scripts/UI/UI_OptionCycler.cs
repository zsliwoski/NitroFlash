using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI class for a cycling through a list of options
/// </summary>
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
	public int currentOptionIndex {get{return curOption;}}
	public string currentOptionID {get{return optionsList [curOption].key;}}
	public string currentOptionDisplayName {get{return optionsList [curOption].displayName;}}

	// Use this for initialization
	void Start () {
		SetCurrentOption (curOption);
	}

	/// <summary>
	/// Adds a selectable option
	/// </summary>
	/// <param name="optionID">String ID to reference the option</param>
	/// <param name="displayName">String for human readable display name</param>
	public void AddOption(string optionID, string displayName){
		optionsList.Add (new OptionCyclerOption (optionID, displayName));
	}

	/// <summary>
	/// Adds a dictionary of options to the option cycler, useful for initialization
	/// </summary>
	/// <param name="options">Dictionary of options to add</param>
	public void AddMultipleOptions(Dictionary<string,string> options){
		foreach (var item in options){
			AddOption (item.Key, item.Value);
		}
	}

	/// <summary>
	/// Clear out all options from cycler
	/// </summary>
	public void ClearOptions(){
		optionDisplay.text = "";
		curOption = -1;
		optionsList.Clear ();
	}

	/// <summary>
	/// Sets the current visible option in the cycler
	/// </summary>
	/// <param name="index">Index of the target option</param>
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

	/// <summary>
	/// Selects previous option
	/// </summary>
	public void OptionDecrement(){
		SetCurrentOption (curOption - 1);
	}

	/// <summary>
	/// Selects next option
	/// </summary>
	public void OptionIncrement(){
		SetCurrentOption (curOption + 1);
	}

	/// <summary>
	/// Checks whether cycler is empty
	/// </summary>
	/// <returns>True if cycler has no options</returns>
	public bool isEmpty(){
		return optionsList.Count == 0;
	}
}
