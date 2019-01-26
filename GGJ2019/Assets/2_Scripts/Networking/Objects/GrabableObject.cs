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

	public void OnGrab(Transform point, PlayerGrabber grabber)
	{
		_playerGrabber = grabber;

		_rigid.isKinematic = true;

		transform.position = point.position;
		transform.rotation = point.rotation;

		_objectSync.DisableSync();
	}

	public void OnCarry(Transform point)
	{
		transform.position = point.position;
		transform.rotation = point.rotation;
	}

	public void OnRelease(Vector3 velocity)
	{
		_rigid.isKinematic = false;
		_rigid.velocity = velocity * _velocityMultiplier;

		if (isServer)
		{
			_objectSync.SetServerMovement();
			_objectSync.SetServerRotate();
		}
	}

	public NetworkInstanceId GetNetId()
	{
		return _netId;
	}
}
