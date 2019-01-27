using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GrabableObject : NetworkBehaviour, IGrabable
{
	Rigidbody _rigid;

	[SerializeField] float _velocityMultiplier = 1.0f;

	ObjectSync _objectSync;

	PlayerGrabber _playerGrabber;

	NetworkInstanceId _netId;

	void Awake()
	{
		_rigid = GetComponent<Rigidbody>();
		_objectSync = GetComponent<ObjectSync>();
	}

	void Start()
	{
		_netId = GetComponent<NetworkIdentity>().netId;
	}

	void OnDestroy()
	{
		if (isServer && _playerGrabber != null)
		{
			_playerGrabber.ForceRelease();
		}
	}

	public void OnGrab(PlayerGrabber grabber)
	{
		_playerGrabber = grabber;

		_rigid.isKinematic = true;

		OnCarry();

		if (isServer)
			_objectSync.DisableSync();
	}

	public void OnCarry()
	{
		if (!IsGrabbed())
			return;

		Transform grabPoint = _playerGrabber.GetGrabPoint();

		if (transform == null)
			Debug.LogWarning("Transform was null");

		if (!gameObject.activeSelf)
			Debug.LogWarning("Gameobject was inactive");

		transform.position = grabPoint.position;
		transform.rotation = grabPoint.rotation;
	}

	public void OnRelease(Vector3 velocity)
	{
		if (!IsGrabbed())
			return;

		_rigid.isKinematic = false;
		_rigid.velocity = velocity * _velocityMultiplier;

		if (isServer && gameObject.activeSelf)
		{
			_objectSync.SetServerMovement();
			_objectSync.SetServerRotate();
		}
	}

	public bool IsGrabbed()
	{
		if (this == null)
			return false;

		if (!this.enabled)
			return false;

		return _playerGrabber != null;
	}

	public NetworkInstanceId GetNetId()
	{
		return _netId;
	}
}
