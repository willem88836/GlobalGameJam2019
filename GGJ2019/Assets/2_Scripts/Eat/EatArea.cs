using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatArea : MonoBehaviour
{
	List<Transform> _edibles;

	void Awake()
	{
		_edibles = new List<Transform>();
	}

	private void OnTriggerEnter(Collider col)
	{
		IEdible edible = col.GetComponent<IEdible>();

		if (edible != null && !_edibles.Contains(col.transform))
		{
			_edibles.Add(col.transform);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		IEdible edible = col.GetComponent<IEdible>();

		if (edible != null && _edibles.Contains(col.transform))
			_edibles.Remove(col.transform);
	}

	public Transform GetClosestEdible()
	{
		_edibles.RemoveAll(x => x == null);

		float shortDistance = float.MaxValue;
		int shortIndex = -1;

		for (int i = 0; i < _edibles.Count; i++)
		{
			Transform current = _edibles[i];

			float currentDistance = Vector3.Distance(transform.position, current.position);

			if (currentDistance < shortDistance)
			{
				shortDistance = currentDistance;
				shortIndex = i;
			}
		}

		if (shortIndex < 0)
			return null;

		return _edibles[shortIndex];
	}
}
