using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 2, sendInterval = 1.0f / 15.0f)]
public class PlayerMovementSync : NetworkBehaviour
{
    Rigidbody _rigid;

	Coroutine _syncMovement = null;
	Coroutine _syncRotate = null;

	Coroutine _smoothMovement = null;
	Coroutine _smoothRotate = null;

	float _backRotation = 0.0f;

	float _lastMoveUpdate = 0f;
	float _lastRotateUpdate = 0f;

	Vector3 _lastVelocity = Vector3.zero;

	bool _walking = true;

	float _sendInterval = 0.1f;

	List<float> _movementDelta = new List<float>();
	List<float> _rotateDelta = new List<float>();

	[SerializeField] int _trackCount = 10;

	[SerializeField] float _maxDeltaInterval = 0.5f;

	void Awake()
	{
		_rigid = GetComponent<Rigidbody>();

		// Default set kinematic
		_rigid.isKinematic = true;

		//_sendInterval = GetNetworkSendInterval();

		//StartCoroutine(WaitForRegister());
	}

	void Start()
    {
		_sendInterval = GetNetworkSendInterval();

		if (isServer)
			SetLocalMovement();
	}

	[Server]
	public void SetLocalMovement()
	{
		Debug.Log("Setting local movement");

		DisableMovementSync();
		RpcSetLocalMovement();

		_rigid.isKinematic = true;
	}

	[ClientRpc]
	void RpcSetLocalMovement()
	{
		if (!isLocalPlayer)
			return;

		Debug.Log("Setting local movement");

		DisableMovementSync();

		_lastMoveUpdate = Time.time;

		_rigid.isKinematic = false;
		_syncMovement = StartCoroutine(SyncClientMovement());
	}

	[Server]
	public void SetServerMovement()
	{
		DisableMovementSync();
		RpcSetServerMovement();

		_lastMoveUpdate = Time.time;

		_rigid.isKinematic = false;
		_syncMovement = StartCoroutine(SyncServerMovement());
	}

	[ClientRpc]
	void RpcSetServerMovement()
	{
		if (!isLocalPlayer)
			return;

		DisableMovementSync();

		_rigid.isKinematic = true;
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
	[Client]
	IEnumerator SyncClientMovement()
	{
		for (; ; )
		{

			Vector3 horVelocity = _rigid.velocity;
			horVelocity.y = 0;

			if (horVelocity.sqrMagnitude > 1.0f)
				_walking = true;
			else
				_walking = false;

			CmdMovementUpdate(_rigid.position, _rigid.velocity);

			yield return new WaitForSeconds(_sendInterval);
		}
	}

	[Server]
	IEnumerator SyncServerMovement()
	{
		for (; ; )
		{
			Vector3 horVelocity = _rigid.velocity;
			horVelocity.y = 0;

			if (horVelocity.sqrMagnitude > 1.0f)
				_walking = true;
			else
				_walking = false;

			RpcMovementUpdate(_rigid.position, _rigid.velocity);

			yield return new WaitForSeconds(_sendInterval);
		}
	}

	// SYNC ROTATE MODULES

	[Client]
	IEnumerator SyncClientRotate()
	{
		for (; ; )
		{
			CmdRotateUpdate(_rigid.rotation, _rigid.angularVelocity, _backRotation);

			yield return new WaitForSeconds(_sendInterval);
		}
	}

	[Server]
	IEnumerator SyncServerRotate()
	{
		for (; ; )
		{
			RpcRotateUpdate(_rigid.rotation, _rigid.angularVelocity, _backRotation);

			yield return new WaitForSeconds(_sendInterval);
		}
	}

	// MOVEMENT SYNC

	[Command]
	void CmdMovementUpdate(Vector3 position, Vector3 velocity)
	{
		AddMovementDelta();
		float avgDelta = GetMovementDelta();

		_lastVelocity = velocity;

		Vector3 predictedPosition = position; // + (velocity * GetPlayerLatency());

		RpcRemoteMovementUpdate(predictedPosition, velocity);

		if (_smoothMovement != null)
			StopCoroutine(_smoothMovement);
		_smoothMovement = StartCoroutine(SmoothMove(predictedPosition, avgDelta));

		if (velocity.sqrMagnitude > 1.0f)
			_walking = true;
		else
			_walking = false;
	}

	[ClientRpc]
	void RpcRemoteMovementUpdate(Vector3 position, Vector3 velocity)
	{
		// Only sync from server on non local players
		if (isLocalPlayer)
			return;

		AddMovementDelta();
		float avgDelta = GetMovementDelta();

		_lastVelocity = velocity;

		Vector3 predictedPosition = position; // + (velocity * GetLocalLatency());

		if (_smoothMovement != null)
			StopCoroutine(_smoothMovement);
		_smoothMovement = StartCoroutine(SmoothMove(predictedPosition, avgDelta));

		if (velocity.sqrMagnitude > 1.0f)
			_walking = true;
		else
			_walking = false;
	}

	[ClientRpc]
	void RpcMovementUpdate(Vector3 position, Vector3 velocity)
	{
		AddMovementDelta();
		float avgDelta = GetMovementDelta();

		Vector3 predictedPosition = position; // + (velocity * GetLocalLatency());

		if (_smoothMovement != null)
			StopCoroutine(_smoothMovement);
		_smoothMovement = StartCoroutine(SmoothMove(predictedPosition, avgDelta));

		if (velocity.sqrMagnitude > 1.0f)
			_walking = true;
		else
			_walking = false;
	}

	// ROTATE SYNC

	[Command]
	void CmdRotateUpdate(Quaternion rotation, Vector3 angularVelocity, float backRotation)
	{
		AddRotateDelta();
		float avgDelta = GetRotateDelta();

		// Calculate world angularVelocity vector to euler rotation in local axis
		Vector3 eulerVelocity = angularVelocity * Mathf.Rad2Deg;
		float eulerLength = eulerVelocity.magnitude;
		Vector3 localVelocity = new Vector3(0.0f, eulerLength, 0.0f);

		Quaternion predictedRotation = rotation; // * Quaternion.Euler(localVelocity * GetPlayerLatency());

		RpcRemoteRotateUpdate(predictedRotation, angularVelocity, backRotation);

		if (_smoothRotate != null)
			StopCoroutine(_smoothRotate);
		_smoothRotate = StartCoroutine(SmoothRotate(predictedRotation, backRotation, avgDelta));
	}

	[ClientRpc]
	void RpcRemoteRotateUpdate(Quaternion rotation, Vector3 angularVelocity, float backRotation)
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

		Quaternion predictedRotation = rotation; // * Quaternion.Euler(localVelocity * GetLocalLatency());

		if (_smoothRotate != null)
			StopCoroutine(_smoothRotate);
		_smoothRotate = StartCoroutine(SmoothRotate(predictedRotation, backRotation, avgDelta));
	}

	[ClientRpc]
	void RpcRotateUpdate(Quaternion rotation, Vector3 angularVelocity, float backRotation)
	{
		AddRotateDelta();
		float avgDelta = GetRotateDelta();

		// Calculate world angularVelocity vector to euler rotation in local axis
		Vector3 eulerVelocity = angularVelocity * Mathf.Rad2Deg;
		float eulerLength = eulerVelocity.magnitude;
		Vector3 localVelocity = new Vector3(0.0f, eulerLength, 0.0f);

		Quaternion predictedRotation = rotation; // * Quaternion.Euler(localVelocity * GetLocalLatency());

		if (_smoothRotate != null)
			StopCoroutine(_smoothRotate);
		_smoothRotate = StartCoroutine(SmoothRotate(predictedRotation, backRotation, avgDelta));
	}

	// SMOOTH SYNC

	IEnumerator SmoothMove(Vector3 position, float duration)
	{
		Vector3 oldPosition = _rigid.position;

		float timer = 0.0f;

		while (timer <= duration)
		{
			yield return null;

			timer += Time.deltaTime;
			float percent = timer / duration;
			Vector3 currentPos = Vector3.Lerp(oldPosition, position, percent);

			// Works for kinimatic
			transform.position = currentPos;

		}

		//_rigid.MovePosition(position);
		transform.position = position;
		_smoothMovement = null;
	}

	IEnumerator SmoothRotate(Quaternion rotation, float backAngle, float duration)
	{
		Quaternion oldRotation = _rigid.rotation;
		float oldBackAngle = _backRotation;

		float timer = 0.0f;

		while (timer <= duration)
		{
			yield return null;

			timer += Time.deltaTime;
			float percent = timer / duration;
			Quaternion currentRot = Quaternion.Lerp(oldRotation, rotation, percent);
			float currentAngle = Mathf.Lerp(oldBackAngle, backAngle, percent);

			// Update rotation for next sync
			_backRotation = currentAngle;

			// Works for kinimatic
			transform.rotation = currentRot;
		}

		transform.rotation = rotation;
		_smoothRotate = null;
	}

	// ACCESS

	public bool IsWalking()
	{
		return _walking;
	}

	[Client]
	public void SetBackRotation(float angle)
	{
		_backRotation = angle;
	}

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

	/*
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
	*/

	public Vector3 GetLastVelocity()
	{
		return _lastVelocity;
	}
}
