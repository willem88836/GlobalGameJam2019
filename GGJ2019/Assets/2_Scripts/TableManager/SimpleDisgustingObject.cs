using UnityEngine;

public class SimpleDisgustingObject : MonoBehaviour, IDisgusting
{
	[SerializeField] private float _disgustingValue = 5;

	public virtual float GetDistgustingValue()
	{
		return _disgustingValue;
	}
}
