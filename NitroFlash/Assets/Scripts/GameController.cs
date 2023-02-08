using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using System.Collections.Generic;

/// <summary>
/// Controls Local Game State during match
/// </summary>
public class GameController : MonoBehaviour {
	
	public delegate void PlayerRosterChangeDelegate (PlayerMovement player);
	public event PlayerRosterChangeDelegate PlayerJoinRosterEvent;
	public event PlayerRosterChangeDelegate PlayerLeaveRosterEvent;

	GameObject[] spawns;
	public List<PlayerMovement> playerObjects;

	public enum PlayerOrderBy {
		SCORE,
		KILLS,
		DEATHS,
		KD_RATIO
	};

	// Use this for initialization
	void Start () {
		playerObjects = new List<PlayerMovement> ();
		playerObjects.AddRange(FindObjectsOfType<PlayerMovement>());
		spawns = GameObject.FindGameObjectsWithTag ("Respawn");
		Transform spawn = RandomSpawn();
		NetworkManager.Instance.InstantiateNetworkPlayer (0,spawn.position,spawn.rotation);
	}

	/// <summary>
	/// Applies names from dictionary to relevant player characters
	/// </summary>
	/// <param name="nameDict">Dictionary of names to apply to net ID'd characters</param>
	public void ApplyNames(Dictionary<uint,string> nameDict){
		foreach (PlayerMovement npb in playerObjects) {
			PlayerMovement pm = (PlayerMovement)npb;
			string outDictValue;
			if (nameDict.TryGetValue(pm.networkObject.NetworkId, out outDictValue)){
				pm.playerName = outDictValue;
			}
		}
	}

	/// <summary>
	/// Chooses a random spawn from current map
	/// </summary>
	/// <returns>Transform representing a random spawn</returns>
	public Transform RandomSpawn(){
		return (spawns[Random.Range(0,(spawns.Length))]).transform;
	}

	/// <summary>
	/// Gets a place to spawn
	/// </summary>
	/// <returns>Transform representing a spawn point</returns>
	public Transform GetSpawn(){
		return RandomSpawn ();
	}
		
	
	//TODO: Implement a function that actually just sorts these based on each
	//Not currently necessary, but will be important going forward
	/// <summary>
	/// Gets player with the highest value in one of the scoreboard categories
	/// </summary>
	/// <param name="param">Parameter to search through</param>
	/// <returns>Player of the highest target scoreboard parameter</returns>
	public PlayerMovement GetPlayerWithMost(PlayerOrderBy param){
		PlayerMovement retVal = null;
		foreach(PlayerMovement np in playerObjects){
			if (retVal == null) {
				retVal = np;
			} else {
				float a = -1;
				float b = -1;
				switch (param) {
				case PlayerOrderBy.SCORE:
					a = retVal.networkObject.score;
					b = np.networkObject.score;
					break;

				case PlayerOrderBy.KILLS:
					a = retVal.networkObject.kills;
					b = np.networkObject.kills;
					break;
				
				case PlayerOrderBy.DEATHS:
					a = retVal.networkObject.death;
					b = np.networkObject.death;
					break;

				case PlayerOrderBy.KD_RATIO:
					//TODO: see if this logic is necessary
					a = retVal.GetKDRatio();
					b = np.GetKDRatio();
					break;
				}

				if (a < b) {
					retVal = np;
				}
			}
		}
		return retVal;
	}

	/// <summary>
	/// Gets first instance of a character that the client owns
	/// </summary>
	/// <returns>First instance of a character that the client owns</returns>
	public PlayerMovement GetOwnedPlayer(){
		foreach (PlayerMovement np in playerObjects) {
			if (np.networkObject.IsOwner) {
				return np;
			}
		}
		return null;
	}

	/////////////////////////////////////
	///**** SERVER USED FUNCTIONS ****///
	/////////////////////////////////////

	/// <summary>
	/// Gets player from registry with provided NetworkID
	/// </summary>
	/// <param name="id">NetworkID of player to retrieve</param>
	/// <returns>Player of given network ID or null if none exists</returns>
	public PlayerMovement GetNetworkPlayerFromID(uint id){
		foreach (PlayerMovement np in playerObjects) {
			if (np.networkObject.NetworkId == id) {
				return np;
			}
		}
		return null;
	}

	/// <summary>
	/// Adds player character to list of active player characters
	/// </summary>
	/// <param name="np">Player Character to add</param>
	public void AddPlayerToRegistry(PlayerMovement np){
		playerObjects.Add (np);
		if (PlayerJoinRosterEvent != null) {
			PlayerJoinRosterEvent.Invoke (np);
		}
	}

	/// <summary>
	/// Removes player character to list of active player characters
	/// </summary>
	/// <param name="np">Player Character to remove</param>
	public void RemovePlayerFromRegistry(PlayerMovement np){
		playerObjects.Remove (np);
		if (PlayerLeaveRosterEvent != null) {
			PlayerLeaveRosterEvent.Invoke (np);
		}
	}
}
