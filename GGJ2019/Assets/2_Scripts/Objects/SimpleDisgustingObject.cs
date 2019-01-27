using System;
using UnityEngine;

public class SimpleDisgustingObject : MonoBehaviour, IDisgusting, IObjective
{
	[SerializeField] private float _disgustingValue = 5;

	public Action<IObjective> OnComplete { get; set; }

	public int Type { get; set; }
	public NetworkPlayer Player { get; set; }
	public bool IsOnPlate { get; set; }

	public virtual float GetDistgustingValue()
	{
		return _disgustingValue;
	}

	public void OnFork()
	{
	}
}

