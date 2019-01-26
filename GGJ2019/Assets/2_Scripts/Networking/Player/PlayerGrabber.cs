using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerGrabber : NetworkBehaviour
{
	[SerializeField] GrabArea _grabArea;
	[SerializeField] PlayerHandSync _handSync;
	[SerializeField] Transform _grabPoint;

	IGrabable _grabbedObject;

	bool _triggerPressed = false;

	void Update()
	{
		if (isServer)
			ServerUpdate();
		else
			ClientUpdate();
	}

	void ServerUpdate()
	{
		if (_grabbedObject != null)
			_grabbedObject.OnCarry(_grabPoint);
	}

	void ClientUpdate()
	{
		if (_grabbedObject != null)
			_grabbedObject.OnCarry(_grabPoint);

		if (!isLocalPlayer)
			return;

		if (Input.GetAxisRaw("RightTrigger") < 0 && !_triggerPressed)
		{
			_triggerPressed = true;
			CmdAttemptGrab();
		}
		else if (Input.GetAxisRaw("RightTrigger") >= 0 && _triggerPressed)
		{
			_triggerPressed = false;
			CmdAttemptRelease();
		}
	}

	[Command]
	void CmdAttemptGrab()
	{
		// TODO: Check and grab
		Transform grabTransform = _grabArea.GetClosestGrabable();

		if (grabTransform == null)
			return;

		IGrabable grab = grabTransform.GetComponent<IGrabable>();

		if (grab == null)
			return;

		_grabbedObject = grab;
		_grabbedObject.OnGrab(_grabPoint, this);

		RpcGrabLocalObject(grab.GetNetId());
	}

	[ClientRpc]
	void RpcGrabLocalObject(NetworkInstanceId netId)
	{
		//GameObject localObj = NetworkServer.FindLocalObject(netId);
		GameObject localObj = ClientScene.FindLocalObject(netId);

		if (localObj == null)
			return;

		IGrabable grab = localObj.GetComponent<IGrabable>();

		if (grab == null)
			return;

		_grabbedObject = grab;
	}

	[Server]
	public void ForceRelease()
	{
		if (_grabbedObject == null)
			return;

		RpcReleaseLocalObject();

		_grabbedObject.OnRelease(_handSync.GetLastVelocity());
		_grabbedObject = null;
	}

	[Command]
	void CmdAttemptRelease()
	{
		if (_grabbedObject == null)
			return;

		RpcReleaseLocalObject();

		_grabbedObject.OnRelease(_handSync.GetLastVelocity());
		_grabbedObject = null;
	}

	[ClientRpc]
	void RpcReleaseLocalObject()
	{
		if (_grabbedObject == null)
			return;

		_grabbedObject.OnRelease(_handSync.GetLastVelocity());
		_grabbedObject = null;
	}
}
