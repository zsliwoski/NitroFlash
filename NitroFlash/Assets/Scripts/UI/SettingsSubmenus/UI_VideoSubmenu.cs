using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class defines necessary methods for the Video submenu in the settings menu
/// </summary>
public class UI_VideoSubmenu : UI_SettingsSubsection {
	DisplaySettingsController displayController;

	public UI_OptionCycler resolutionOptions;
	public UI_OptionCycler fullscreenOptions;
	Resolution[] allResolutions;
	Dictionary<string,string> resOptionsDict = new Dictionary<string, string>();
	Dictionary<string,string> fullscreenOptionsDict = new Dictionary<string, string>{
		{"true","Fullscreen"},
		{"false", "Windowed"}
	};
	int lastResOption = 0;

	// Use this for initialization
	void Start(){
		resolutionOptions.OptionChanged += (string optionID, string displayText) => lastResOption = int.Parse(optionID);
	}

	/// <summary>
	/// Fills video menus with options to select from
	/// </summary>
	/// <param name="resToFill">An array of resolutions to fill as options</param>
	public void FillOptions(Resolution[] resToFill){
		if (displayController == null) {
			displayController = FindObjectOfType<DisplaySettingsController> ();
		}

		resOptionsDict.Clear ();
		resolutionOptions.ClearOptions ();

		int index = -1;

		foreach (Resolution r in resToFill) {
			index++;
			resOptionsDict.Add(index.ToString(), r.width + "x" + r.height + " @ " + r.refreshRate + "Hz");
		} 

		resolutionOptions.AddMultipleOptions(resOptionsDict);

		if (fullscreenOptions.isEmpty ()) {
			fullscreenOptions.AddMultipleOptions (fullscreenOptionsDict);
		}

	}

	/// <summary>
	/// Called when the attached subsection has been transitioned to
	/// </summary>
	public override void TransitionedTo(){
		if (displayController == null) {
			displayController = FindObjectOfType<DisplaySettingsController> ();
		}

		allResolutions = Screen.resolutions;
		FillOptions (allResolutions);
		//assumes we have at least one availible resolution
		resolutionOptions.SetCurrentOption (lastResOption);
		gameObject.SetActive (true);
	}


	/// <summary>
	/// Called when options are applied in the attached subsection
	/// </summary>
	public override void ApplySettings ()
	{
		ApplyDisplaySettings ();
	}

	/// <summary>
	/// Applies selected display settings in subsection
	/// </summary>
	public void ApplyDisplaySettings(){
		if (displayController == null) {
			displayController = FindObjectOfType<DisplaySettingsController> ();
		}
		int resChoice = int.Parse (resolutionOptions.currentOptionID);
		bool fullscreenChoice = bool.Parse (fullscreenOptions.currentOptionID);
		displayController.SetResolution (allResolutions [resChoice], fullscreenChoice);
	}
}
