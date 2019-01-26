using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 2, sendInterval = 1.0f / 15.0f)]
public class ObjectSync : NetworkBehaviour
{
	[SerializeField] Transform _transform;
	[SerializeField] Rigidbody _rigid;
	[SerializeField] Collider[] _colliders;

	Coroutine _syncMovement = null;
	Coroutine _syncRotate = null;

	Coroutine _smoothMovement = null;
	Coroutine _smoothRotate = null;

	float _lastMoveUpdate = 0f;
	float _lastRotateUpdate = 0f;

	float _sendInterval = 0.1f;

	List<float> _movementDelta = new List<float>();
	List<float> _rotateDelta = new List<float>();

	[SerializeField] int _trackCount = 10;
	[SerializeField] float _maxDeltaInterval = 0.5f;

	PlayerList _playerList;

	void Awake()
	{
		// Default set kinematic
		//_rigid.isKinematic = true;
	}

	void Start()
	{
		_playerList = PlayerList.Singleton();

		_sendInterval = GetNetworkSendInterval();

		if (isServer)
		{
			SetServerMovement();
			SetServerRotate();
		}
	}

	[Server]
	public void SetServerMovement()
	{
		DisableMovementSync();
		RpcSetServerMovement();

		_lastMoveUpdate = Time.time;

		_rigid.isKinematic = false;
		
		for (int i = 0; i < _colliders.Length; i++)
		{
			Collider current = _colliders[i];

			current.enabled = true;
		}

		_transform.GetComponent<Collider>().enabled = true;

		_syncMovement = StartCoroutine(SyncServerMovement());

		PlayerList.Singleton().OnPlayerJoined += delegate { ForceSync(_transform); };
	}

	[ClientRpc]
	void RpcSetServerMovement()
	{
		DisableMovementSync();

		_rigid.isKinematic = true;

		for (int i = 0; i < _colliders.Length; i++)
		{
			Collider current = _colliders[i];

			current.enabled = false;
		}
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

	public void DisableSync()
	{
		DisableMovementSync();
		DisableRotateSync();

		PlayerList.Singleton().OnPlayerJoined -= delegate { ForceSync(_transform); };
	}

	void DisableMovementSync()
	{
		if (_syncMovement != null)
		{
			StopCoroutine(_syncMovement);
			_syncMovement = null;
		}

		if (_smoothMovement != null)
		{
			StopCoroutine(_smoothMovement);
			_smoothMovement = null;
		}
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

	// SYNC MOVEMENT MODULES

	[Server]
	IEnumerator SyncServerMovement()
	{
		for (; ; )
		{
			RpcMovementUpdate(_rigid.position, _rigid.velocity);

			yield return new WaitForSeconds(_sendInterval);
		}
	}

	// SYNC ROTATE MODULES

	[Server]
	IEnumerator SyncServerRotate()
	{
		for (; ; )
		{
			RpcRotateUpdate(_rigid.rotation, _rigid.angularVelocity);

			yield return new WaitForSeconds(_sendInterval);
		}
	}

	// MOVEMENT SYNC

	[ClientRpc]
	void RpcRemoteMovementUpdate(Vector3 position, Vector3 velocity)
	{
		// Only sync from server on non local players
		if (isLocalPlayer)
			return;

		AddMovementDelta();
		float avgDelta = GetMovementDelta();

		Vector3 predictedPosition = position + (velocity * GetLocalLatency());

		if (_smoothMovement != null)
			StopCoroutine(_smoothMovement);
		_smoothMovement = StartCoroutine(SmoothMove(predictedPosition, avgDelta));
	}

	[ClientRpc]
	void RpcMovementUpdate(Vector3 position, Vector3 velocity)
	{
		AddMovementDelta();
		float avgDelta = GetMovementDelta();

		Vector3 predictedPosition = position + (velocity * GetLocalLatency());

		if (_smoothMovement != null)
			StopCoroutine(_smoothMovement);
		_smoothMovement = StartCoroutine(SmoothMove(predictedPosition, avgDelta));
	}

	// ROTATE SYNC

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

	IEnumerator SmoothMove(Vector3 position, float duration)
	{
		Vector3 oldPosition = _rigid.position;
		//_rigid.velocity = Vector3.zero;

		float timer = 0.0f;

		while (timer <= duration)
		{
			yield return null;

			timer += Time.deltaTime;
			float percent = timer / duration;
			Vector3 currentPos = Vector3.Lerp(oldPosition, position, percent);

			// Works for kinimatic
			_transform.position = currentPos;

		}

		//_rigid.MovePosition(position);
		_transform.position = position;
		_smoothMovement = null;
	}

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

	// ACCESS

	void AddMovementDelta()
	{
		float delta = Time.time - _lastMoveUpdate;
		_lastMoveUpdate = Time.time;

		if (delta > _maxDeltaInterval)
			return;

		_movementDelta.Add(delta);

		if (_movementDelta.Count > _trackCount)
			_movementDelta.RemoveAt(0);
	}

	public float GetMovementDelta()
	{
		if (_movementDelta.Count <= 0)
			return _sendInterval;

		float average = 0;

		for (int i = 0; i < _movementDelta.Count; i++)
		{
			average += _movementDelta[i];
		}
		average /= _movementDelta.Count;

		return average;
	}

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

	[Server]
	public void ForceSync(Transform point)
	{
		_lastMoveUpdate = Time.time;
		_lastRotateUpdate = Time.time;

		DisableMovementSync();

		DisableRotateSync();

		_transform.position = point.position;
		_transform.rotation = point.rotation;

		RpcForceSync(point.position, point.rotation);

		SetServerMovement();
		SetServerRotate();
	}

	[ClientRpc(channel = 0)]
	void RpcForceSync(Vector3 position, Quaternion rotation)
	{
		_lastMoveUpdate = Time.time;
		_lastRotateUpdate = Time.time;

		DisableMovementSync();

		DisableRotateSync();

		_transform.position = position;
		_transform.rotation = rotation;
	}

	float GetLocalLatency()
	{
		if (_playerList != null)
			return _playerList.GetLocalLatency();

		return 0;
	}
}
