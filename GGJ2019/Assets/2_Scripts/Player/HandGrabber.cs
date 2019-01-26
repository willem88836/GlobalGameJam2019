using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class HandGrabber : MonoBehaviour
{
	HandMover _handMover;

	bool _isGrabbing;
	GrabObject _nearestObject;
	GrabObject _grabbedObject;

	List<GrabObject> _grabables = new List<GrabObject>();

	[SerializeField] Animator _animator;
	float _currentTimeframe;
	float _targetTimeFrame = 0;
	float _animateSpeed = 0;
	float _lerpValue = 0;

	void Start()
	{
		_handMover = transform.parent.GetComponent<HandMover>();
		_nearestObject = null;
	}

	void Update()
    {
		ClearMissingObjects();

		if (_isGrabbing)
		{
			if (Input.GetAxisRaw("RightTrigger") >= 0)
				ReleaseGrab();
		}
		else
		{
			GetClosestObject();

			if (Input.GetAxisRaw("RightTrigger") < 0)
				Grab();
		}

		if (_grabbedObject != null)
			HoldObject();

		Animate();
	}

	void Grab()
	{
		_isGrabbing = true;

		if (_grabables.Count == 0)
		{
			StartAnimateOpen(1);
			return;
		}

		_grabbedObject = _nearestObject;
		_nearestObject.GetComponent<Outline>().color = 0;

		_grabbedObject.Grab();

		StartAnimateOpen(0.17f); // eventually base this number on a value in GrabObject
	}

	void ClearMissingObjects()
	{
		for (int i = _grabables.Count - 1; i >= 0; i--)
		{
			if (_grabables[i] == null)
			{
				_grabables.RemoveAt(i);
			}
		}
	}

	void GetClosestObject()
	{
		if (_grabables.Count == 0)
		{
			if (_nearestObject != null)
			{
				_nearestObject.GetComponent<Outline>().color = 0;
				_nearestObject = null;
			}
			return;
		}

		_nearestObject = _grabables[0];

		for (int i = 0; i < _grabables.Count; i++)
		{
			float closestDistance = Vector3.Distance(transform.position, _nearestObject.transform.position);
			float thisDistance = Vector3.Distance(transform.position, _grabables[i].transform.position);

			if (thisDistance < closestDistance)
			{
				_nearestObject.GetComponent<Outline>().color = 0;
				_nearestObject = _grabables[i];
			}
		}

		_nearestObject.GetComponent<Outline>().color = 2;
	}

	/// <summary>
	///		Release the grabbed object
	/// </summary>
	void ReleaseGrab()
	{
		_isGrabbing = false;
		StartAnimateClose();
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

	void StartAnimateOpen(float timeFrame)
	{
		_targetTimeFrame = timeFrame;
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
