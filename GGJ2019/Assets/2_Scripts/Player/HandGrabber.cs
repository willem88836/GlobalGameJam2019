﻿using System.Collections.Generic;
using UnityEngine;

public class HandGrabber : MonoBehaviour
{
	HandMover _handMover;

	bool _isGrabbing;
	GrabObject _grabbedObject;

	List<GrabObject> _grabables = new List<GrabObject>();

	void Start()
	{
		_handMover = transform.parent.GetComponent<HandMover>();
	}

	void Update()
    {
		if (_isGrabbing)
		{
			if (Input.GetAxisRaw("RightTrigger") >= 0)
				ReleaseGrab();
		}
		else
		{
			if (Input.GetAxisRaw("RightTrigger") < 0)
				Grab();
		}
		
		if (_grabbedObject != null)
			HoldObject();
	}

	void Grab()
	{
		_isGrabbing = true;

		if (_grabables.Count == 0)
			return;
		
		_grabbedObject = _grabables[0];

		if (_grabables.Count > 1)
			GetClosestObject();

		_grabbedObject.Grab();
	}

	void GetClosestObject()
	{
		for (int i = 0; i < _grabables.Count; i++)
		{
			float closestDistance = Vector3.Distance(transform.position, _grabbedObject.transform.position);
			float thisDistance = Vector3.Distance(transform.position, _grabables[i].transform.position);

			if (thisDistance < closestDistance)
			{
				_grabbedObject = _grabables[i];
			}
		}
	}

	/// <summary>
	///		Release the grabbed object
	/// </summary>
	void ReleaseGrab()
	{
		_isGrabbing = false;
		if (_grabbedObject == null)
			return;

		_grabbedObject.Release(_handMover.Rigidbody.velocity);
		_grabbedObject = null;
	}

	/// <summary>
	///		Hold the object in place, in the hand
	/// </summary>
	void HoldObject()
	{
		Transform grabPivot = _grabbedObject.transform;
		grabPivot.position = transform.position;
		grabPivot.rotation = transform.rotation;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<GrabObject>())
			_grabables.Add(other.GetComponent<GrabObject>());;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<GrabObject>())
			_grabables.Remove(other.GetComponent<GrabObject>());
	}
}
