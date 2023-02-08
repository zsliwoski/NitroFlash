using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI class for a color adjustment widget
/// </summary>
public class UI_ColorAdjuster : MonoBehaviour {
	public delegate void ColorChangeDelegate (Color col);
	public event ColorChangeDelegate ColorChangeEvent;

	public Slider rSlider;
	public Slider gSlider;
	public Slider bSlider;

	public Image previewImage;

	Color col;
	bool sliderJustSet = false;

	// Use this for initialization
	void Start () {
		//col = new Color (1,1,1,1);
		//UpdateColor();
	}

	/// <summary>
	/// Sets the RGB sliders for the provided Color object
	/// </summary>
	/// <param name="col">Color object to set the slider values to</param>
	public void SetColor(Color col){
		this.col = col;
		sliderJustSet = true;
		rSlider.value = col.r;
		gSlider.value = col.g;
		bSlider.value = col.b;
		sliderJustSet = false;
		UpdateColor ();
	}

	/// <summary>
	/// Sets the RGB sliders for the provided Color object
	/// </summary>
	/// <param name="r">Red color channel value, ranges from 0 to 1</param>
	/// <param name="g">Green color channel value, ranges from 0 to 1</param>
	/// <param name="b">Blue color channel value, ranges from 0 to 1</param>
	public void SetColor (float r, float g, float b){
		col = new Color (r, g, b, 1.0f);
		sliderJustSet = true;
		rSlider.value = r;
		gSlider.value = g;
		bSlider.value = b;
		sliderJustSet = false;
		UpdateColor ();
	}

	/// <summary>
	/// Updates the tint of the preview color
	/// </summary>
	public void UpdateColor(){
		previewImage.color = col;
	}

	/// <summary>
	/// Called when a color slider changes value
	/// </summary>
	/// <param name="val">The input from the color slider, from 0 to 1</param>
	public void SliderValueChange(float val){
		col.r = rSlider.value;
		col.g = gSlider.value;
		col.b = bSlider.value;
		UpdateColor ();
		if (!sliderJustSet) {
			if (ColorChangeEvent != null) {
				ColorChangeEvent.Invoke (col);	
			}
		}
	}
}
