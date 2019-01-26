using System.Collections.Generic;
using UnityEngine;

public class HandGrabber : MonoBehaviour
{
	HandMover _handMover;

	bool _isGrabbing;
	GrabObject _grabbedObject;

	List<GrabObject> _grabables = new List<GrabObject>();
	List<int> _ignoredObjects = new List<int>();

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

		_grabbedObject.Grab(this);
	}

	void GetClosestObject()
	{
		for (int i = 0; i < _grabables.Count; i++)
		{
			if (_ignoredObjects.Contains(_grabables[i].GetInstanceID()))
				continue;

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
	public void ReleaseGrab()
	{
		_isGrabbing = false;
		if (_grabbedObject == null)
			return;

		_grabbedObject.Release(_handMover.Rigidbody.velocity);
		_grabbedObject = null;
	}

	public void ForceRelease()
	{
		_ignoredObjects.Add(_grabbedObject.GetInstanceID());
		ReleaseGrab();
	}

	/// <summary>
	///		Hold the object in place, in the hand
	/// </summary>
	void HoldObject()
	{
		_grabbedObject.transform.position = transform.position;
		_grabbedObject.transform.rotation = transform.rotation;
	}

	void OnTriggerEnter(Collider other)
	{
		GrabObject obj = other.GetComponent<GrabObject>();
		if (obj != null && !_grabables.Contains(obj))
			_grabables.Add(obj);
	}

	private void OnTriggerExit(Collider other)
	{
		GrabObject obj = other.GetComponent<GrabObject>();
		if (obj != null && _grabables.Contains(obj))
		{
			_grabables.Remove(obj);

			int id = obj.GetInstanceID();
			if (_ignoredObjects.Contains(id))
			{
				_ignoredObjects.Remove(id);
			}
		}
	}
}
