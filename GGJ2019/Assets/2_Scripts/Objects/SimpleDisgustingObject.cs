using System;
using UnityEngine;

public class SimpleDisgustingObject : GrabObject, IDisgusting, IEdible, IObjective, IForkable
{
	[SerializeField] private float _disgustingValue = 5;

	public Action<IObjective> OnComplete { get; set; }

	public int Type { get; set; }
	public int Player { get; set; }
	public bool IsOnPlate { get; set; }

	public virtual float GetDistgustingValue()
	{
		return _disgustingValue;
	}

	public void OnEat(Eater eater)
	{
		if (OnComplete != null)
			OnComplete.Invoke(this);

		Destroy(gameObject);
	}

	public void OnFork()
	{
	}
}
