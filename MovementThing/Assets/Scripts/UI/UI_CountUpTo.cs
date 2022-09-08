using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CountUpTo : MonoBehaviour {
	float current = 0f;
	public int decimalPlaces = 0;
	public float speed = 1.0f;
	float targetCount = 0.0f;
	bool countActive = false;
	public bool finishedCount = false;
	bool targetSet = false;
	public Text targetText;

	// Update is called once per frame
	void Update () {
		if (countActive && targetSet && !finishedCount) {
			current += Time.deltaTime * (targetCount / speed);
			if (current > targetCount) {
				countActive = false;
				finishedCount = true;
				current = targetCount;
			}
			targetText.text = current.ToString ("F" + decimalPlaces);
		}
	}
	public void SetTarget (float target){
		targetSet = true;
		targetCount = target;
	}
	public void StartCount(){
		countActive = true;
	}
}
