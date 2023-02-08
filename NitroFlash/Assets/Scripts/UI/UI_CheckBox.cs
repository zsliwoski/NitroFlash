using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// UI class for a check box widget
/// </summary>
public class UI_CheckBox : MonoBehaviour {
	public bool isChecked;
	public Image checkmarkImage;

	// Use this for initialization
	void Start () {
		checkmarkImage.enabled = isChecked;
	}

	/// <summary>
	/// Toggles "Checked" state of checkmark box
	/// </summary>
	public void ToggleCheckmark(){
		isChecked = !isChecked;
		checkmarkImage.enabled = isChecked;
	}
}
