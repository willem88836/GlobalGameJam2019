using System.Collections.Generic;
using UnityEngine;

public class Eater : MonoBehaviour
{
	[SerializeField] public int _grabbedLayer = 9;

	private List<GameObject> _currentEdibles = new List<GameObject>();


	private void OnTriggerEnter(Collider other)
	{
		IEdible edible = other.GetComponent<IEdible>();
		if (other.gameObject.layer == _grabbedLayer && edible != null && !_currentEdibles.Contains(other.gameObject))
		{
			_currentEdibles.Add(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		IEdible edible = other.GetComponent<IEdible>();
		if (other.gameObject.layer == _grabbedLayer && edible != null && _currentEdibles.Contains(other.gameObject))
		{
			_currentEdibles.Remove(other.gameObject);
		}
	}


	private void Update()
	{
		for (int i = _currentEdibles.Count - 1; i >= 0; i--)
		{
			GameObject edibleObject = _currentEdibles[i];
			if(edibleObject.layer != _grabbedLayer)
			{
				IEdible edible = edibleObject.GetComponent<IEdible>();
				edible.OnEat(this);
				_currentEdibles.RemoveAt(i);
			}
		}
	}
}
