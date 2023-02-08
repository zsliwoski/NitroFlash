using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class defines necessary methods for the Audio submenu in the settings menu
/// </summary>
public class UI_AudioSubmenu : UI_SettingsSubsection {

	public AudioController audioController;
	public Slider masterSlider;
	public Slider sfxSlider;
	public Slider musicSlider;

	/// <summary>
	/// Called when the attached subsection has been transitioned to
	/// </summary>
	public override void TransitionedTo(){
		if (audioController == null) {
			audioController = FindObjectOfType<AudioController> ();
		}

		masterSlider.value = audioController.masterVolume;
		sfxSlider.value = audioController.sfxVolume;
		musicSlider.value = audioController.musicVolume;

		gameObject.SetActive (true);
	}

	/// <summary>
	/// Called when options are applied in the attached subsection
	/// </summary>
	public override void ApplySettings ()
	{
		base.ApplySettings ();
		ApplyAudioSettings (masterSlider.value, sfxSlider.value, musicSlider.value);
	}

	/// <summary>
	/// Applies selected audio settings in subsection
	/// </summary>
	/// <param name="master">From 0-1 the volume of the "master" audio channel</param>
	/// <param name="sfx">From 0-1 the volume of the "sfx" audio channel</param>
	/// <param name="music">From 0-1 the volume of the "music" audio channel</param>
	public void ApplyAudioSettings(float master, float sfx, float music){
		if (audioController == null) {
			audioController = FindObjectOfType<AudioController> ();
		}

		audioController.SetMasterVolume (master);
		audioController.SetSFXVolume (sfx);
		audioController.SetMusicVolume (music);
	}
}
