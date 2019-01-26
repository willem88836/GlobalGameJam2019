using System.Collections.Generic;
using UnityEngine;

public class TableSegment : MonoBehaviour
{
	private List<IDisgusting> _disgustingObjects = new List<IDisgusting>();

	PlayerSlot _slot;

	private void Awake()
	{
		_slot = GetComponent<PlayerSlot>();
	}

	private void OnTriggerEnter(Collider other)
	{
		IDisgusting disgusting = other.GetComponent<IDisgusting>();
		if (disgusting != null && !_disgustingObjects.Contains(disgusting))
		{
			_disgustingObjects.Add(disgusting);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		IDisgusting disgusting = other.GetComponent<IDisgusting>();
		if (disgusting != null && _disgustingObjects.Contains(disgusting))
		{
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

	public PlayerSlot GetSlot()
	{
		return _slot;
	}
}
