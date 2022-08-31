using UnityEngine;
using System.Collections;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.UI;
using AssemblyCSharp;

public class GamemodeBase : NetworkGamemodeObjectBehavior{
	float counter = 0.0f;

	public int topTargetTime = 600;
	public int intermissionTime = 30;
	public int matchGamemode = 2;
	public bool usingTeams = false;
	public bool usingScoreGoal = false;
	public int scoreGoal = 0;

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

	public void EndMatch(){
		int a = networkObject.teamAScore;
		int b = networkObject.teamBScore;

		int winningTeam = a < b ? 0 : 1;
		
		if (a == b){
			winningTeam = 2;
		}
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
}
