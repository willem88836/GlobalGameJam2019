using UnityEngine;

public abstract class ToolPoint<T> : MonoBehaviour
{
	private GrabObject _grabObject;

	private void Awake()
	{
		_grabObject = transform.parent.GetComponent<GrabObject>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_grabObject.Grabbed)
			return;

		T obj = other.GetComponent<T>();
		if (obj != null)
		{
			Invoke(obj);
		}
	}

	protected abstract void Invoke(T obj);
}
