using System;
using UnityEngine;

public class Grapes : MonoBehaviour, IObjective, IEdible, IDisgusting
{
	public int Type { get; set; }
	public int Player { get; set; }
	public Action<IObjective> OnComplete { get; set; }

	[SerializeField] private float _maxVelocity;

	private Vector3 _previousPosition;
	private void Start()
	{
		_previousPosition = transform.position;
	}



	[SerializeField] private float _disgustingValue;
	public float GetDistgustingValue()
	{
		return _disgustingValue;
	}

	public void OnEat(Eater eater)
	{
		if (OnComplete != null)
			OnComplete.Invoke(this);
	}


	private void Update()
	{
		Vector3 currentPosition = transform.position;
		if (gameObject.layer == GrabObject.GRABBEDLAYER)
		{
			float vel = (currentPosition - _previousPosition).magnitude;
			if (vel >= _maxVelocity)
			{
				GetComponent<GrabObject>().GrabParent.ForceRelease();
				Debug.Log("Relaese");
			}
		}
		_previousPosition = currentPosition;
	}
}
