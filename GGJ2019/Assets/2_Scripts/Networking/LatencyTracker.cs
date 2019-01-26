using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LatencyTracker : NetworkBehaviour
{
	[SerializeField] float _pingInterval = 0.5f;

	PlayerList _playerList;

	void Start()
	{
		_playerList = PlayerList.Singleton();

		if (!isServer)
			StartCoroutine(PingRequest());
	}

	[Server]
	IEnumerator PingRequest()
	{
		int hostId = NetworkServer.serverHostId;

		for (; ; )
		{
			int stamp = NetworkTransport.GetNetworkTimestamp();

			//Debug.Log(hostId + " : " + stamp);
			RpcPingPlayers(hostId, stamp);

			yield return new WaitForSeconds(_pingInterval);
		}
	}

	[ClientRpc(channel = 1)]
	void RpcPingPlayers(int hostId, int serverStamp)
	{
		NetworkPlayer localPlayer = _playerList.GetLocalPlayer();

		if (localPlayer == null)
			return;

		localPlayer.PingPlayer(hostId, serverStamp);
		Debug.Log(_playerList.GetLocalPlayer().GetLatency());
	}
}
