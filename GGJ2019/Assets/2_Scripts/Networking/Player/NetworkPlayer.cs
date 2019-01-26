using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour
{
	[HideInInspector] public float SegmentDirtyness = 0.0f;

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
		_handSync.ForceSync(slot.HandStartPoint);
		//_handSync.SetLocalMovement();
		//_handSync.SetLocalRotate();

		_headSync.ForceSync(slot.HeadStartPoint);
		//_headSync.SetLocalRotate();
	}
}
