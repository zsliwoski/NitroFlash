using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is a generic implementation of a settings subsection for the UI
/// Basic methods are included
/// </summary>
public class UI_SettingsSubsection : MonoBehaviour {
	//TODO: flesh this class out more later
	public Button applyButton;

	/// <summary>
	/// Called when the attached subsection has been transitioned to
	/// </summary>
	virtual public void TransitionedTo(){
		gameObject.SetActive (true);
	}

	/// <summary>
	/// Hides the attached subsection
	/// </summary>
	virtual public void Hide(){
		gameObject.SetActive (false);
	}

	/// <summary>
	/// Called when the attached subsection has been transitioned away from
	/// </summary>
	virtual public void TransitionedFrom(){
		Hide ();
	}

	/// <summary>
	/// Called when options are applied in the attached subsection
	/// </summary>
	virtual public void ApplySettings(){
		if(applyButton != null){
			applyButton.enabled = false;
		}
	}

	/// <summary>
	/// Called when an option in the attached subsection has been changed
	/// </summary>
	virtual public void Changed(){
		if(applyButton != null){
			applyButton.enabled = true;
		}
	}
}
