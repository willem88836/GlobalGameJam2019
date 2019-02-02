using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IGrabable
{
	void OnGrab(PlayerGrabber grabber);

	void OnCarry();

	void OnRelease(Vector3 velocity);

	bool IsGrabbed();

	NetworkInstanceId GetNetId();

	bool IsGrabbed();
}
