using UnityEngine;

public abstract class ToolPoint<T> : MonoBehaviour
{
	protected GrabableObject MyGrabObject;

	private void Awake()
	{
		MyGrabObject = transform.parent.GetComponent<GrabableObject>();

		if (!MyGrabObject.isServer)
			enabled = false;	
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!MyGrabObject.IsGrabbed())
			return;

		T obj = other.GetComponent<T>();
		if (obj != null)
		{
			Debug.Log(other.name);
			Invoke(obj);
		}
	}

	protected abstract void Invoke(T obj);
}
