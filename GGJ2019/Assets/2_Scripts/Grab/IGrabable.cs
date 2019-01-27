using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IGrabable
{
	void OnGrab(Transform point, PlayerGrabber grabber);

	void OnCarry(Transform point);

	void OnRelease(Vector3 velocity);

	NetworkInstanceId GetNetId();

	bool IsGrabbed();
}
