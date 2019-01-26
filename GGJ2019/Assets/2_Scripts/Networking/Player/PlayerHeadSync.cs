using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 2, sendInterval = 1.0f / 15.0f)]
public class PlayerHeadSync : NetworkBehaviour
{
	[SerializeField] Transform _transform;
	[SerializeField] Rigidbody _rigid;

	Coroutine _syncRotate = null;

	Coroutine _smoothRotate = null;

	float _lastRotateUpdate = 0f;

	float _sendInterval = 0.1f;

	List<float> _rotateDelta = new List<float>();

	[SerializeField] int _trackCount = 10;
	[SerializeField] float _maxDeltaInterval = 0.5f;

	NetworkPlayer _networkPlayer;
	PlayerList _playerList;

	void Awake()
	{
		// Default set kinematic
		_rigid.isKinematic = true;

		_networkPlayer = GetComponent<NetworkPlayer>();
	}

	void Start()
	{
		_playerList = PlayerList.Singleton();

		_sendInterval = GetNetworkSendInterval();

		if (isServer)
			PlayerList.Singleton().OnPlayerJoined += delegate { ForceSync(_transform); };

		//ForceSync();
	}

	[Server]
	public void SetLocalRotate()
	{
		DisableRotateSync();
		RpcSetLocalRotate();
	}

	[ClientRpc]
	void RpcSetLocalRotate()
	{
		if (!isLocalPlayer)
			return;

		DisableRotateSync();

		_lastRotateUpdate = Time.time;

		_rigid.isKinematic = false;
		_syncRotate = StartCoroutine(SyncClientRotate());
	}

	[Server]
	public void SetServerRotate()
	{
		DisableRotateSync();
		RpcSetServerRotate();

		_lastRotateUpdate = Time.time;

		_syncRotate = StartCoroutine(SyncServerRotate());
	}

	[ClientRpc]
	void RpcSetServerRotate()
	{
		if (!isLocalPlayer)
			return;

		DisableRotateSync();
	}

	void DisableRotateSync()
	{
		if (_syncRotate != null)
		{
			StopCoroutine(_syncRotate);
			_syncRotate = null;
		}

		if (_smoothRotate != null)
		{
			StopCoroutine(_smoothRotate);
			_smoothRotate = null;
		}
	}

	// SYNC ROTATE MODULES

	[Client]
	IEnumerator SyncClientRotate()
	{
		for (; ; )
		{
			CmdRotateUpdate(_rigid.rotation, _rigid.angularVelocity);

			yield return new WaitForSeconds(_sendInterval);
		}
	}

	[Server]
	IEnumerator SyncServerRotate()
	{
		for (; ; )
		{
			RpcRotateUpdate(_rigid.rotation, _rigid.angularVelocity);

			yield return new WaitForSeconds(_sendInterval);
		}
	}

	// ROTATE SYNC

	[Command]
	void CmdRotateUpdate(Quaternion rotation, Vector3 angularVelocity)
	{
		AddRotateDelta();
		float avgDelta = GetRotateDelta();

		// Calculate world angularVelocity vector to euler rotation in local axis
		Vector3 eulerVelocity = angularVelocity * Mathf.Rad2Deg;
		float eulerLength = eulerVelocity.magnitude;
		Vector3 localVelocity = new Vector3(0.0f, eulerLength, 0.0f);

		Quaternion predictedRotation = rotation * Quaternion.Euler(localVelocity * GetPlayerLatency());

		RpcRemoteRotateUpdate(predictedRotation, angularVelocity);

		if (_smoothRotate != null)
			StopCoroutine(_smoothRotate);
		_smoothRotate = StartCoroutine(SmoothRotate(predictedRotation, avgDelta));
	}

	[ClientRpc]
	void RpcRemoteRotateUpdate(Quaternion rotation, Vector3 angularVelocity)
	{
		// Only sync from server on non local players
		if (isLocalPlayer)
			return;

		AddRotateDelta();
		float avgDelta = GetRotateDelta();

		// Calculate world angularVelocity vector to euler rotation in local axis
		Vector3 eulerVelocity = angularVelocity * Mathf.Rad2Deg;
		float eulerLength = eulerVelocity.magnitude;
		Vector3 localVelocity = new Vector3(0.0f, eulerLength, 0.0f);

		Quaternion predictedRotation = rotation * Quaternion.Euler(localVelocity * GetLocalLatency());

		if (_smoothRotate != null)
			StopCoroutine(_smoothRotate);
		_smoothRotate = StartCoroutine(SmoothRotate(predictedRotation, avgDelta));
	}

	[ClientRpc]
	void RpcRotateUpdate(Quaternion rotation, Vector3 angularVelocity)
	{
		AddRotateDelta();
		float avgDelta = GetRotateDelta();

		// Calculate world angularVelocity vector to euler rotation in local axis
		Vector3 eulerVelocity = angularVelocity * Mathf.Rad2Deg;
		float eulerLength = eulerVelocity.magnitude;
		Vector3 localVelocity = new Vector3(0.0f, eulerLength, 0.0f);

		Quaternion predictedRotation = rotation * Quaternion.Euler(localVelocity * GetLocalLatency());

		if (_smoothRotate != null)
			StopCoroutine(_smoothRotate);
		_smoothRotate = StartCoroutine(SmoothRotate(predictedRotation, avgDelta));
	}

	// SMOOTH SYNC

	IEnumerator SmoothRotate(Quaternion rotation, float duration)
	{
		Quaternion oldRotation = _rigid.rotation;

		float timer = 0.0f;

		while (timer <= duration)
		{
			yield return null;

			timer += Time.deltaTime;
			float percent = timer / duration;
			Quaternion currentRot = Quaternion.Lerp(oldRotation, rotation, percent);

			// Works for kinimatic
			_transform.rotation = currentRot;
		}

		_transform.rotation = rotation;
		_smoothRotate = null;
	}

	[Server]
	public void ForceSync(Transform point)
	{
		_lastRotateUpdate = Time.time;

		DisableRotateSync();

		_transform.position = point.position;
		_transform.rotation = point.rotation;

		RpcForceSync(point.position, point.rotation);

		SetLocalRotate();
	}

	[ClientRpc(channel = 0)]
	void RpcForceSync(Vector3 position, Quaternion rotation)
	{
		_lastRotateUpdate = Time.time;

		DisableRotateSync();

		_transform.position = position;
		_transform.rotation = rotation;
	}

	// ACCESS

	void AddRotateDelta()
	{
		float delta = Time.time - _lastRotateUpdate;
		_lastRotateUpdate = Time.time;

		if (delta > _maxDeltaInterval)
			return;

		_rotateDelta.Add(delta);

		if (_rotateDelta.Count > _trackCount)
			_rotateDelta.RemoveAt(0);
	}

	public float GetRotateDelta()
	{
		if (_rotateDelta.Count <= 0)
			return _sendInterval;

		float average = 0;

		for (int i = 0; i < _rotateDelta.Count; i++)
		{
			average += _rotateDelta[i];
		}
		average /= _rotateDelta.Count;

		return average;
	}

	float GetPlayerLatency()
	{
		return _networkPlayer.GetLatency();
	}

	float GetLocalLatency()
	{
		if (_playerList != null)
			return _playerList.GetLocalLatency();

		return 0;
	}
}
