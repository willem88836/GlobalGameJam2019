using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour
{
	[Header("Latency")]
	[SerializeField] int _trackCount = 10;

	[Header("Awards")]
	[SerializeField] GameObject _mostEatenCrown;
	[SerializeField] GameObject _dirtiestCrown;
	[SerializeField] GameObject _cleanestCrown;

	List<int> _pingTracker = new List<int>();
	int _averagePing = 0;

	PlayerHandSync _handSync;
	PlayerHeadSync _headSync;

	bool _awarded = false;

    public NetworkInstanceId GetNetId()
	{
		return this.netId;
	}

	private void Awake()
	{
		_handSync = GetComponent<PlayerHandSync>();
		_headSync = GetComponent<PlayerHeadSync>();
	}

	void Start()
	{
		if (isServer)
		{
			PlayerList playerList = PlayerList.Singleton();
			playerList.OnPlayerJoined += delegate { SyncAwards(); };
		}
	}

	[Server]
	public void PreparePlayer(PlayerSlot slot)
	{
		transform.position = slot.transform.position;
		transform.forward = slot.transform.forward;

		RpcPositionPlayer(slot.transform.position, slot.transform.rotation);

		_handSync.ForceSync(slot.HandStartPoint);

		_headSync.ForceSync(slot.HeadStartPoint);
	}

	[ClientRpc]
	void RpcPositionPlayer(Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;
	}

	[Server]
	public void PunishPlayer()
	{
		// TODO: Punishment
		RpcPunishment();
	}

	[ClientRpc]
	void RpcPunishment()
	{
		if (!isLocalPlayer)
			return;

		Debug.Log(">> Granda is angry at you! <<");
	}
	
	// AWARDSs

	[Server]
	public void SyncAwards()
	{
		RpcSyncAwards(_mostEatenCrown.activeSelf, _dirtiestCrown.activeSelf, _cleanestCrown.activeSelf);
	}

	[ClientRpc]
	void RpcSyncAwards(bool mostEaten, bool dirtiest, bool cleanest)
	{
		_mostEatenCrown.SetActive(mostEaten);
		_dirtiestCrown.SetActive(dirtiest);
		_cleanestCrown.SetActive(cleanest);
	}

	[Server]
	public void MostEatenAward()
	{
		if (!_awarded)
		{
			_awarded = true;

			if (!_mostEatenCrown.activeSelf)
				_mostEatenCrown.SetActive(true);
			// TODO: show crown
		}

		RpcMostEatenAward();
	}

	[ClientRpc]
	void RpcMostEatenAward()
	{
		if (!_awarded)
		{
			_awarded = true;

			if (!_mostEatenCrown.activeSelf)
				_mostEatenCrown.SetActive(true);
			// TODO: show crown
		}

		if (isLocalPlayer)
		{
			// TOOD: spawn message
			Debug.Log("You wont the Most eaten award!");
		}
	}

	[Server]
	public void DirtiestAward()
	{
		if (!_awarded)
		{
			_awarded = true;

			if (!_dirtiestCrown.activeSelf)
				_dirtiestCrown.SetActive(true);
			// TODO: show crown
		}

		RpcDirtiestAward();
	}

	[ClientRpc]
	void RpcDirtiestAward()
	{
		if (!_awarded)
		{
			_awarded = true;

			if (!_dirtiestCrown.activeSelf)
				_dirtiestCrown.SetActive(true);
			// TODO: show crown
		}

		if (isLocalPlayer)
		{
			// TOOD: spawn message
			Debug.Log("You are the dirtiest player!");
		}
	}

	[Server]
	public void ResetAwards()
	{
		if (!_awarded)
			return;

		_awarded = false;

		if (_mostEatenCrown.activeSelf)
			_mostEatenCrown.SetActive(false);
		// TODO: Disable each award object
	}

	// PING

	[Client]
	public void PingPlayer(int hostId, int serverStamp)
	{
		NetworkConnection conn = connectionToServer;

		byte error;
		int ping = NetworkTransport.GetRemoteDelayTimeMS(hostId, conn.connectionId, serverStamp, out error);
		AddPingToTracker(ping);
		CalculateAveragePing();

		CmdSyncPing(_averagePing);
	}

	[Command(channel = 1)]
	void CmdSyncPing(int ping)
	{
		_averagePing = ping;
	}

	void AddPingToTracker(int ping)
	{
		_pingTracker.Add(ping);

		if (_pingTracker.Count > _trackCount)
			_pingTracker.RemoveAt(0);
	}

	void CalculateAveragePing()
	{
		if (_pingTracker.Count <= 0)
			_averagePing = 0;

		int average = 0;

		for (int i = 0; i < _pingTracker.Count; i++)
		{
			average += _pingTracker[i];
		}
		average /= _pingTracker.Count;

		_averagePing = average;
	}

	public int GetPing()
	{
		return _averagePing;
	}

	public float GetLatency()
	{
		return _averagePing / 1000f;
	}
}
