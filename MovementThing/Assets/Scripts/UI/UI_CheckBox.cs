using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_CheckBox : MonoBehaviour {
	public bool isChecked;
	public Image checkmarkImage;

	void Start () {
		checkmarkImage.enabled = isChecked;
	}
	public void ToggleCheckmark(){
		isChecked = !isChecked;
		checkmarkImage.enabled = isChecked;
	}
}
