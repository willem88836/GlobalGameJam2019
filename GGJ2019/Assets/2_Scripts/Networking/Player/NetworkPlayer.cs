using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour
{
	[HideInInspector] public float SegmentDirtyness = 0.0f;

	[Header("Latency")]
	[SerializeField] int _trackCount = 10;

	List<int> _pingTracker = new List<int>();
	int _averagePing = 0;

	PlayerHandSync _handSync;
	PlayerHeadSync _headSync;

    public NetworkInstanceId GetNetId()
	{
		return this.netId;
	}

	private void Awake()
	{
		_handSync = GetComponent<PlayerHandSync>();
		_headSync = GetComponent<PlayerHeadSync>();
	}

	[Server]
	public void PreparePlayer(PlayerSlot slot)
	{
		transform.position = slot.transform.position;
		transform.forward = slot.transform.forward;

		RpcPositionPlayer(slot.transform.position, slot.transform.rotation);

		_handSync.ForceSync(slot.HandStartPoint);
		//_handSync.SetLocalMovement();
		//_handSync.SetLocalRotate();

		_headSync.ForceSync(slot.HeadStartPoint);
		//_headSync.SetLocalRotate();
	}

	[ClientRpc]
	void RpcPositionPlayer(Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;
	}


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
