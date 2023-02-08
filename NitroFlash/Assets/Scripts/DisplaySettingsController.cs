using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls active display settings
/// </summary>
public class DisplaySettingsController : MonoBehaviour {

	// Called before first frame
	void Awake(){
		DontDestroyOnLoad (this);
	}

	// Use this for initialization
	void Start(){
		LoadResolution ();
	}

	/// <summary>
	/// Applies player preferred resolution
	/// </summary>
	public void LoadResolution(){		
		int width = PlayerPrefs.GetInt ("ScreenWidth", Screen.currentResolution.width);
		int height = PlayerPrefs.GetInt ("ScreenHeight", Screen.currentResolution.height);
		int fs = PlayerPrefs.GetInt ("isFullscreen", 1);
		int refreshRate = PlayerPrefs.GetInt ("RefreshRate", Screen.currentResolution.refreshRate);
		bool isFullscreen = fs == 1 ? true : false; 
		Screen.SetResolution (width, height, isFullscreen, refreshRate);
	}

	/// <summary>
	/// Sets player preferred resolution
	/// </summary>
	/// <param name="r">Resolution to use</param>
	/// <param name="isFullscreen">Whether new resolution should set the window to fullscreen</param>
	public void SetResolution(Resolution r, bool isFullscreen){
		int fs = isFullscreen ? 1 : 0; 
		Screen.SetResolution (r.width, r.height, isFullscreen, r.refreshRate);
		PlayerPrefs.SetInt ("ScreenWidth", r.width);
		PlayerPrefs.SetInt ("ScreenHeight", r.height);
		PlayerPrefs.SetInt ("isFullscreen", fs);
		PlayerPrefs.SetInt ("RefreshRate", r.refreshRate);
	}
}
