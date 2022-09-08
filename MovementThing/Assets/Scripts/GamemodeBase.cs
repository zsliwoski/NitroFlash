using UnityEngine;
using System.Collections;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.UI;
using AssemblyCSharp;
	
public class GamemodeBase : NetworkGamemodeObjectBehavior{
	[System.Serializable]
	public class TeamInfo{
		public Color teamColor;
		public string teamName;
	}

	float counter = 0.0f;

	public int topTargetTime = 600;
	public int intermissionTime = 30;
	public int matchGamemode = 2;
	public int scorePerKill = 100;
	public bool usingTeams = false;
	public bool usingScoreGoal = false;
	public bool killsAreScore = false;
	public int scoreGoal = 0;

	public TeamInfo[] teamsInfos;

	//EVENTS//
	public delegate void RoundEndDelegate (int winningTeam);
	public event RoundEndDelegate RoundEndEvent;

	public delegate void RoundStartDelegate ();
	public event RoundStartDelegate RoundStartEvent;

	public string[] gamemodeTypes = {
		"Pre-Match",
		"Intermission",
		"DEFAULT"
	};

	protected override void NetworkStart ()
	{
		base.NetworkStart ();
	}
	
	// Update is called once per frame
	void Update () {
		if (networkObject.IsServer) {
			int curTime = topTargetTime - (int)Mathf.Floor(counter);
			
			if (!networkObject.gamemodeActive){
				curTime = intermissionTime - (int)Mathf.Floor(counter);
			}
			counter += Time.deltaTime;

			curTime = Mathf.Clamp(curTime,0,10000);
			networkObject.timeRunning = curTime;
			
			if (curTime <= 0){
				if (networkObject.gamemodeActive){
					EndMatch();
				}else{
					BeginMatch();
				}
			}
		}
	}
		
	public void ClearScores(){
		networkObject.teamAScore = 0;
		networkObject.teamBScore = 0;
	}

	public int GetWinner(){
		if (usingTeams) {
			if (networkObject.teamAScore > networkObject.teamBScore) {
				return 0;
			} else if (networkObject.teamAScore == networkObject.teamBScore) {
				return -1;
			} else {
				return 1;
			}
		} else {
			//TODO: circular dependency here, should find a solution
			GameController gam = FindObjectOfType<GameController> ();
			NetworkPlayerBehavior np = gam.GetPlayerWithMost (GameController.PlayerOrderBy.SCORE);

			//if we have a valid best player, they win
			if (np != null) {
				return (int)np.networkObject.NetworkId;
			} else {
				//otherwise return tie
				return -1;
			}
		}
	}

	public void EndMatch(){
		int winningTeam = GetWinner();

		counter = 0;
		networkObject.gamemodeActive = false;
		networkObject.SendRpc (NetworkGamemodeObjectBehavior.RPC_MULTICAST__ROUND_END, BeardedManStudios.Forge.Networking.Receivers.All, winningTeam);
		networkObject.gamemodeType = 1;
	}

	public void BeginMatch(){
		ClearScores ();
		networkObject.gamemodeType = matchGamemode;
		networkObject.SendRpc (NetworkGamemodeObjectBehavior.RPC_MULTICAST__ROUND_START, BeardedManStudios.Forge.Networking.Receivers.All);
		networkObject.gamemodeActive = true;
		counter = 0;
	}

	public override void Multicast_RoundEnd(RpcArgs args){
		int winner = args.GetNext<int> ();
		print("ROUND ENDED");
		if (RoundEndEvent != null) {
			RoundEndEvent.Invoke (winner);
		}
	}

	public override void Multicast_RoundStart(RpcArgs args){
		print("ROUND STARTED");
		if (RoundStartEvent != null) {
			RoundStartEvent.Invoke ();
		}
	}

	public override void Client_AddScore(RpcArgs args){
		int amount = args.GetNext<int> ();
		int team = args.GetNext<int> ();

		if (team == 0) {
			networkObject.teamAScore += amount;
		} else {
			networkObject.teamBScore += amount;
		}
	}
}
