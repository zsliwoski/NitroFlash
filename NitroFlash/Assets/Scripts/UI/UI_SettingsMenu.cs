using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class controls the screen state and functionality of the settings menu
/// </summary>
public class UI_SettingsMenu : MonoBehaviour {

	public enum SettingsMenuType
	{
		NONE,
		VIDEO,
		AUDIO,
		CONTROLS
	}

	public UI_SettingsSubsection videoPanel;
	public UI_SettingsSubsection audioPanel;
	public UI_SettingsSubsection controlsPanel;
	UI_SettingsSubsection currentSelection = null;
	public SettingsMenuType curMenu = SettingsMenuType.NONE;

	/// <summary>
	/// Switches the current active menu to the target menu
	/// </summary>
	/// <param name="menu">Sub-menu to switch to</param>
	public void SwitchMenu(string menu){
		SettingsMenuType toMenuEnum = (SettingsMenuType)System.Enum.Parse (typeof(SettingsMenuType), menu);
		SwitchMenu (toMenuEnum);
	}

	/// <summary>
	/// Switches the current active menu to the target menu
	/// </summary>
	/// <param name="menu">Sub-menu to switch to</param>
	public void SwitchMenu(SettingsMenuType menu){
		if (curMenu != menu) {
			curMenu = menu;

			videoPanel.Hide ();
			audioPanel.Hide ();
			controlsPanel.Hide ();

			switch (curMenu) {
			case SettingsMenuType.VIDEO:
				videoPanel.TransitionedTo ();
				currentSelection = videoPanel;
					break;
			case SettingsMenuType.AUDIO:
				audioPanel.TransitionedTo ();
				currentSelection = audioPanel;
					break;
			case SettingsMenuType.CONTROLS:
				controlsPanel.TransitionedTo ();
				currentSelection = controlsPanel;
					break;
			}
		}
	}

	/// <summary>
	/// Applies the settings of the current active sub-menu
	/// </summary>
	public void ApplyCurrent(){
		if (currentSelection != null) {
			currentSelection.ApplySettings ();
		}
	}

}
