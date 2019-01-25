using System.Collections.Generic;
using UnityEngine;

public class TableSegment : MonoBehaviour
{
	private List<IDisgusting> _disgustingObjects = new List<IDisgusting>();


	private void OnTriggerEnter(Collider other)
	{
		IDisgusting disgusting = other.GetComponent<IDisgusting>();
		if (disgusting != null && !_disgustingObjects.Contains(disgusting))
		{
			Debug.LogFormat("Added: {0}", other.name);
			_disgustingObjects.Add(disgusting);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		IDisgusting disgusting = other.GetComponent<IDisgusting>();
		if (disgusting != null && _disgustingObjects.Contains(disgusting))
		{
			Debug.LogFormat("Removed: {0}", other.name);
			_disgustingObjects.Remove(disgusting);
		}
	}


	public float GetDisgustingValue()
	{
		float totalDisgusting = 0;
		foreach(IDisgusting disgusting in _disgustingObjects)
		{
			totalDisgusting += disgusting.GetDistgustingValue();
		}
		return totalDisgusting;
	}

}
