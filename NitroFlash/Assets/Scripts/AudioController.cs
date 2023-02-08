using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Controls active audio mixer levels
/// </summary>
public class AudioController : MonoBehaviour {
	[SerializeField]
	public AudioMixer targetMix;

	float masterVol = 1.0f;
	float sfxVol = 1.0f;
	float musicVol = 1.0f;

	public float masterVolume {
		get { return masterVol; }
	}
	public float sfxVolume {
		get { return sfxVol; }
	}
	public float musicVolume {
		get { return musicVol; }
	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
		masterVol = PlayerPrefs.GetFloat ("MasterVolume", 1.0f);
		sfxVol = PlayerPrefs.GetFloat ("SFXVolume", 1.0f);
		musicVol = PlayerPrefs.GetFloat ("MusicVolume", 1.0f);
		SetMasterVolume (masterVol);
		SetSFXVolume (sfxVol);
		SetMusicVolume (musicVol);
	}

	/// <summary>
	/// Sets the volume of a mixer channel
	/// </summary>
	/// <param name="channel">ID of channel</param>
	/// <param name="newVol">Target volume (from 0 to 1)</param>
	public void SetVolume (string channel, float newVol)
	{
		PlayerPrefs.SetFloat (channel + "Volume", newVol);
		targetMix.SetFloat (channel + "Volume", newVol);
	}

	/// <summary>
	/// Sets volume of "Master" channel
	/// </summary>
	/// <param name="newVol">Target volume (from 0 to 1)</param>
	public void SetMasterVolume (float newVol){
		SetVolume ("Master", newVol);
	}

	/// <summary>
	/// Sets volume of "SFX" channel
	/// </summary>
	/// <param name="newVol">Target volume (from 0 to 1)</param>
	public void SetSFXVolume (float newVol){
		SetVolume ("SFX", newVol);
	}

	/// <summary>
	/// Sets volume of "Music" channel
	/// </summary>
	/// <param name="newVol">Target volume (from 0 to 1)</param>
	public void SetMusicVolume (float newVol){
		SetVolume ("Music", newVol);
	}
}
