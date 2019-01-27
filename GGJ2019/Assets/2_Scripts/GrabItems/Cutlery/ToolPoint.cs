using UnityEngine;

public abstract class ToolPoint<T> : MonoBehaviour
{
	protected GrabObject MyGrabObject;

	private void Awake()
	{
		MyGrabObject = transform.parent.GetComponent<GrabObject>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!MyGrabObject.Grabbed)
			return;

		T obj = other.GetComponent<T>();
		if (obj != null)
		{
			Invoke(obj);
		}
	}

	protected abstract void Invoke(T obj);
}
