using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;

/// <summary>
/// Class containing all the logic for the Shotgun ability
/// </summary>
public class Shotgun : MonoBehaviour {
	public int maxAmmo = 20;
	public int curAmmo = 20;
	public int range = 10;
	public int damage = 25;
	public int fof = 100;
	public float fireRate = 0.2f;
	public GameObject hitEffect;
	public AudioSource aud;
	public AudioClip shoot, reload, hitSFX;
	bool reloading = false;
	public LayerMask affected;

	public Animator armAnimator;
	public GameObject particles;
	public Transform particleSpawnPoint;
	float nextFire = -0.1f;

	PlayerMovement owner;

	// Use this for initialization
	void Start () {
		owner = GetComponentInParent<PlayerMovement> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (owner.GetMouseInputFrozen () == false) {
			if (Input.GetButtonDown ("Fire1")) {
				Shoot ();
				print ("you shot man");
			}
		}
		if (nextFire > 0.0f ){
			nextFire -= Time.deltaTime;
		}
	}

	/// <summary>
	/// Handles visual feedback as well as calling rpc to damage hit player serverside
	/// </summary>
	void Shoot(){
		if (nextFire < 0) {
			if (curAmmo == 0) {
				if (!reloading) {
					StartCoroutine (Reload ());
				}
				return;
			}
			nextFire = fireRate;
			curAmmo -= 1;
			armAnimator.CrossFade("shotgun_fired",0.015f);
			Destroy(Instantiate (particles, particleSpawnPoint),0.4f);
			owner.networkObject.SendRpc (NetworkPlayerBehavior.RPC_MULTICAST__GUN_FIRED, BeardedManStudios.Forge.Networking.Receivers.All);
			aud.clip = shoot;
			aud.Play ();
			Collider[] cols = Physics.OverlapSphere (transform.position, range, affected);
			foreach (Collider c in cols) {
				AudioSource.PlayClipAtPoint (hitSFX, (transform.forward * range) + transform.position);
				if (c.gameObject.tag == "Enemy") {
					print ("tag check");
					if (Vector3.Angle (transform.forward, c.transform.position - transform.position) < (fof / 2)) {
						print ("angle was right");
						if (Physics.Linecast (transform.position, c.transform.position,affected)) {
							int dam = Random.Range (damage - 5, damage + 5);
							PlayerMovement epm = c.GetComponent<PlayerMovement> ();

							if (!owner.networkObject.isDead) {
								GameObject hitInstance = Instantiate (hitEffect, epm.transform.position, epm.transform.rotation, epm.transform);
								Destroy (hitInstance, 10.0f);
								object[] rpcArgs = { dam, owner.playerName, owner.networkObject.NetworkId};
								epm.networkObject.SendRpc (NetworkPlayerBehavior.RPC_SERVER__TAKE_DAMAGE, BeardedManStudios.Forge.Networking.Receivers.Owner, rpcArgs);
								object[] forceArgs = { transform.forward, 20f, true };
								epm.networkObject.SendRpc (NetworkPlayerBehavior.RPC_CLIENT__ADD_FORCE, BeardedManStudios.Forge.Networking.Receivers.Owner, forceArgs);
							}
								
							print (c.name + " : hit with " + dam);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Asynchronous method that awaits a reload time before allowing the player to fire
	/// </summary>
	/// <returns>Yield that waits for a successful reload</returns>
	IEnumerator Reload(){
		reloading = true;
		aud.clip = reload;
		aud.Play ();
		yield return new WaitForSeconds (3);
		curAmmo = maxAmmo;
		reloading = false;
	}
}
