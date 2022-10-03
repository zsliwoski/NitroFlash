using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	public void SetColor(Color col){
		this.col = col;
		sliderJustSet = true;
		rSlider.value = col.r;
		gSlider.value = col.g;
		bSlider.value = col.b;
		sliderJustSet = false;
		UpdateColor ();
	}
	public void SetColor (float r, float g, float b){
		col = new Color (r, g, b, 1.0f);
		sliderJustSet = true;
		rSlider.value = r;
		gSlider.value = g;
		bSlider.value = b;
		sliderJustSet = false;
		UpdateColor ();
	}

	public void UpdateColor(){
		previewImage.color = col;
	}

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
