using UnityEngine;

public class SimpleDisgustingObject : MonoBehaviour, IDisgusting, IEdible
{
	[SerializeField] private float _disgustingValue = 5;

	public virtual float GetDistgustingValue()
	{
		return _disgustingValue;
	}

	public void OnEat(Eater eater)
	{
		Destroy(gameObject);
	}
}
