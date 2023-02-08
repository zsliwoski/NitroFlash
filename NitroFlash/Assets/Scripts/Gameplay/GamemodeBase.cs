using UnityEngine;
using System.Collections;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.UI;
using AssemblyCSharp;

/// <summary>
/// The base class for all gamemodes, defines general rules such as team name, score goal, etc. related to a match
/// </summary>
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

	/// <summary>
	/// Network state begins
	/// </summary>
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
	
	/// <summary>
	/// Clears each teams scores
	/// </summary>
	public void ClearScores(){
		networkObject.teamAScore = 0;
		networkObject.teamBScore = 0;
	}

	/// <summary>
	/// Calculates the winner depending on whether or not teams are used
	/// </summary>
	/// <returns>
	/// NetworkId of winning player if no teams are used, otherwise returns winning team
	/// </returns>
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
			//TODO: circular dependency here, should abstract out
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

	/// <summary>
	/// Ends the current match and notifies all connected players of the winner
	/// </summary>
	public void EndMatch(){
		int winningTeam = GetWinner();

		counter = 0;
		networkObject.gamemodeActive = false;
		networkObject.SendRpc (NetworkGamemodeObjectBehavior.RPC_MULTICAST__ROUND_END, BeardedManStudios.Forge.Networking.Receivers.All, winningTeam);
		networkObject.gamemodeType = 1;
	}

	/// <summary>
	/// Clears existing winner and notifies every player that the round is about to begin
	/// </summary>
	public void BeginMatch(){
		ClearScores ();
		networkObject.gamemodeType = matchGamemode;
		networkObject.SendRpc (NetworkGamemodeObjectBehavior.RPC_MULTICAST__ROUND_START, BeardedManStudios.Forge.Networking.Receivers.All);
		networkObject.gamemodeActive = true;
		counter = 0;
	}

	/// <summary>
	/// Client handled event that invokes the logic for the end of a round
	/// </summary>
	/// <param name="args">The received RoundEnd arguments</param>
	public override void Multicast_RoundEnd(RpcArgs args){
		int winner = args.GetNext<int> ();
		print("ROUND ENDED");
		if (RoundEndEvent != null) {
			RoundEndEvent.Invoke (winner);
		}
	}

	/// <summary>
	/// Client handled event that invokes the logic for the start of a round
	/// </summary>
	/// <param name="args">The received RoundStart arguments</param>
	public override void Multicast_RoundStart(RpcArgs args){
		print("ROUND STARTED");
		if (RoundStartEvent != null) {
			RoundStartEvent.Invoke ();
		}
	}

	/// <summary>
	/// Adds score to the specified team
	/// </summary>
	/// <param name="args">The received team and score amount</param>
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
