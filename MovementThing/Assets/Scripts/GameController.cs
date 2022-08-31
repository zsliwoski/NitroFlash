using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;

public class GameController : MonoBehaviour {
	GameObject[] spawns;
	// Use this for initialization
	void Start () {
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
}
