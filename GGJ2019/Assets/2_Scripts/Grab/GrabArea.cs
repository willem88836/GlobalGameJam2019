using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabArea : MonoBehaviour
{
	List<Transform> _grabables;

	void Awake()
	{
		_grabables = new List<Transform>();
	}

	private void OnTriggerEnter(Collider col)
	{
		IGrabable grab = col.GetComponent<IGrabable>();

		if (grab != null && !_grabables.Contains(col.transform))
		{
			_grabables.Add(col.transform);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		IGrabable grab = col.GetComponent<IGrabable>();

		if (grab != null && _grabables.Contains(col.transform))
			_grabables.Remove(col.transform);
	}

	public Transform GetClosestGrabable()
	{
		float shortDistance = float.MaxValue;
		int shortIndex = -1;

		for (int i = 0; i < _grabables.Count; i++)
		{
			Transform current = _grabables[i];

			float currentDistance = Vector3.Distance(transform.position, current.position);

			if (currentDistance < shortDistance)
			{
				shortDistance = currentDistance;
				shortIndex = i;
			}
		}

		if (shortIndex < 0)
			return null;

		return _grabables[shortIndex];
	}
}
