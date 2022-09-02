using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	
	public delegate void PlayerRosterChangeDelegate (NetworkPlayerBehavior player);
	public event PlayerRosterChangeDelegate PlayerJoinRosterEvent;
	public event PlayerRosterChangeDelegate PlayerLeaveRosterEvent;

	GameObject[] spawns;
	public List<NetworkPlayerBehavior> playerObjects;
	// Use this for initialization
	void Start () {
		playerObjects = new List<NetworkPlayerBehavior> ();
		playerObjects.AddRange(FindObjectsOfType<NetworkPlayerBehavior>());
		spawns = GameObject.FindGameObjectsWithTag ("Respawn");
		Transform spawn = RandomSpawn();
		NetworkManager.Instance.InstantiateNetworkPlayer (0,spawn.position,spawn.rotation);
	}

	public Transform RandomSpawn(){
		return (spawns[Random.Range(0,(spawns.Length))]).transform;
	}
	public Transform GetSpawn(){
		return RandomSpawn ();
	}
		
	//SERVER USED FUNCTIONS
	public NetworkPlayerBehavior GetNetworkPlayerFromID(uint id){
		foreach (NetworkPlayerBehavior np in playerObjects) {
			if (np.networkObject.NetworkId == id) {
				return np;
			}
		}
		return null;
	}
	public void AddPlayerToRegistry(NetworkPlayerBehavior np){
		playerObjects.Add (np);
		if (PlayerJoinRosterEvent != null) {
			PlayerJoinRosterEvent.Invoke (np);
		}
	}
	public void RemovePlayerFromRegistry(NetworkPlayerBehavior np){
		playerObjects.Remove (np);
		if (PlayerLeaveRosterEvent != null) {
			PlayerLeaveRosterEvent.Invoke (np);
		}
	}
}
