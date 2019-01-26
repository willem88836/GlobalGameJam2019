using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickupObject : NetworkBehaviour, IGrabable
{
	Rigidbody _rigid;

	[SerializeField] float _velocityMultiplier = 2.0f;

	ObjectSync _objectSync;

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

	public void OnGrab(Transform point)
	{
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
		_rigid.velocity = velocity; // * _velocityMultiplier;

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
