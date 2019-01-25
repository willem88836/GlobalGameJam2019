using System;
using UnityEngine;

public class HandGrabber : MonoBehaviour
{
	bool _isGrabbing;
	Transform _grabbedObject;

	Collider[] _grabables;

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

		_grabbedObject = _grabables[0].transform;

		Debug.Log(_grabbedObject);

		if (_grabables.Length > 1)
			return;

		// get the one thats the closest 
		for (int i = 0; i < _grabables.Length; i++)
		{
			float closestDistance = Vector3.Distance(transform.position, _grabbedObject.position);
			float thisDistance = Vector3.Distance(transform.position, _grabables[i].transform.position);

			if (thisDistance < closestDistance)
			{
				_grabbedObject = _grabables[i].transform;
			}
		}
	}

	void ReleaseGrab()
	{
		_isGrabbing = false;
		_grabbedObject = null;
	}

	void HoldObject()
	{
		_grabbedObject.position = transform.position;
	}

	void OnTriggerEnter(Collider other)
	{
		
	}
}
