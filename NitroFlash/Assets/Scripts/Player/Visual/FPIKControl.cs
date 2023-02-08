using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Small Class that handles First Person Inverse Kinematics targeting
/// </summary>
[RequireComponent(typeof(Animator))]
public class FPIKControl : MonoBehaviour {

	protected Animator animator;

	public bool ikActive = false;
	public Transform rightHandObj = null;
	public Transform leftHandObj = null;

	public Transform defaultRightHandObj = null;
	public Transform defaultLeftHandObj = null;

	// Use this for initialization
	void Start ()
	{
		animator = GetComponent<Animator>();
	}

	//Callback for calculating IK
	void OnAnimatorIK()
	{
		if(animator) {
			//if the IK is active, set the position and rotation directly to the goal.
			if(ikActive) {
				// Set the right hand target position and rotation, if one has been assigned
				if (rightHandObj != null) {
					animator.SetIKPositionWeight (AvatarIKGoal.RightHand, 1);
					animator.SetIKRotationWeight (AvatarIKGoal.RightHand, 1);  
					animator.SetIKPosition (AvatarIKGoal.RightHand, rightHandObj.position);
					animator.SetIKRotation (AvatarIKGoal.RightHand, rightHandObj.rotation);
				} else {
					rightHandObj = defaultRightHandObj;
				}
				if (leftHandObj != null) {
					animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, 1);
					animator.SetIKRotationWeight (AvatarIKGoal.LeftHand, 1);  
					animator.SetIKPosition (AvatarIKGoal.LeftHand, leftHandObj.position);
					animator.SetIKRotation (AvatarIKGoal.LeftHand, leftHandObj.rotation);
				} else {
					leftHandObj = defaultLeftHandObj;
				}
			}

			//if the IK is not active, set the position and rotation of the hand and head back to the original position
			else {          
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0);
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,0);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,0);  
			}
		}
	}    
}
