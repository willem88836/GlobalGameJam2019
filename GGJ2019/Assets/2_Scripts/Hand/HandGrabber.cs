using System.Collections.Generic;
using UnityEngine;

public class HandGrabber : MonoBehaviour
{
	bool _isGrabbing;
	GrabObject _grabbedObject;

	List<GrabObject> _grabables = new List<GrabObject>();

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

	void ReleaseGrab()
	{
		_isGrabbing = false;
		if (_grabbedObject == null)
			return;

		_grabbedObject.Release();
		_grabbedObject = null;
	}

	void HoldObject()
	{
		_grabbedObject.transform.position = transform.position;
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
