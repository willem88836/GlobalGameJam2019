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

	[SerializeField] Animator _animator;
	float _currentTimeframe;
	float _targetTimeFrame = 0;
	float _animateSpeed = 0;
	float _lerpValue = 0;

	bool _triggerPressed = false;

	void Update()
	{
		if (isServer)
			ServerUpdate();
		else
			ClientUpdate();

		Animate();
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
			CmdAnimateOpen();
			//_handAnimator.SetFloat("Blend", 1.0f);
			CmdAttemptGrab();
		}
		else if (Input.GetAxisRaw("RightTrigger") >= 0 && _triggerPressed)
		{
			_triggerPressed = false;
			CmdAnimateClose();
			//_handAnimator.SetFloat("Blend", 0.0f);
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

	[Command]
	void CmdAnimateOpen()
	{
		StartAnimateOpen();
		RpcAnimateOpen();
	}

	[ClientRpc]
	void RpcAnimateOpen()
	{
		StartAnimateOpen();
	}

	[Command]
	void CmdAnimateClose()
	{
		StartAnimateClose();
		RpcAnimateClose();
	}

	[ClientRpc]
	void RpcAnimateClose()
	{
		StartAnimateClose();
	}

	void StartAnimateOpen()
	{
		_targetTimeFrame = 1;
		_animateSpeed = 2;
	}

	void StartAnimateClose()
	{
		_targetTimeFrame = 0;
		_animateSpeed = -2;
	}

	void Animate()
	{
		if (_animateSpeed != 0)
		{
			_lerpValue += _animateSpeed * Time.deltaTime;
			_currentTimeframe = Mathf.Lerp(_currentTimeframe, _targetTimeFrame, _lerpValue);

			_animator.Play("Open", 0, _currentTimeframe);

			_lerpValue = Mathf.Clamp(_lerpValue, 0, 1);
			if (_lerpValue == 0 || _lerpValue == 1)
				_animateSpeed = 0;
		}
	}
}
