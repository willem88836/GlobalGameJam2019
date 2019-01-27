using System;
using UnityEngine;

public class Meatball : GrabObject, IDisgusting, ISliceable, IObjective, IForkable
{
	[SerializeField] private GameObject _meatballHalf;
	[SerializeField] private int _meatballHalfCount;

	public bool IsOnPlate { get; set; }
	public int Type { get; set; }
	public NetworkPlayer Player { get; set; }
	public Action<IObjective> OnComplete { get; set; }

	[SerializeField] private float _disgustingValue = 5;
	public float GetDistgustingValue()
	{
		return _disgustingValue;
	}

	public void OnSlice()
	{
		for (int i = 0; i < _meatballHalfCount; i++)
		{
			GameObject mbh = Instantiate(_meatballHalf, transform.parent);

			float rot = (float)i / _meatballHalfCount * 360f * Mathf.Deg2Rad;
			mbh.transform.position = transform.position + new Vector3(Mathf.Cos(rot), 1, -Mathf.Sin(rot));

			IObjective obj = mbh.GetComponent<IObjective>();
			obj.OnComplete = this.OnComplete;
			obj.Type = this.Type;
			obj.Player = this.Player;
		}

		Destroy(this);
	}

	public void OnFork()
	{
	}
}
