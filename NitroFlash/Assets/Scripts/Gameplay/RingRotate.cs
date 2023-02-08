using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Small class to allow environment objects to rotate at a fixed rate
/// </summary>
public class RingRotate : MonoBehaviour {
	public Vector3 axes;

	// Update is called once per frame
	void Update () {
		transform.Rotate (axes, Space.Self);
	}
}
