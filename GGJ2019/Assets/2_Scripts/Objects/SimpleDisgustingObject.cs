using System;
using UnityEngine;

public class SimpleDisgustingObject : MonoBehaviour, IDisgusting
{
	[SerializeField] private float _disgustingValue = 5;

	public bool IsOnPlate { get; set; }

	public virtual float GetDistgustingValue()
	{
		return _disgustingValue;
	}

	public void OnFork()
	{
	}
}

