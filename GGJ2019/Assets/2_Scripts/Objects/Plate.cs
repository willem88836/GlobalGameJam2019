using UnityEngine;

public class Plate : MonoBehaviour // TODO: Make this grabbable.
{
	private Vector3 _originPosition;
	private Quaternion _originRotation;


	private void Start()
	{
		_originPosition = transform.position;
		_originRotation = transform.rotation;
	}

	private void OnTriggerEnter(Collider other)
	{
		IDisgusting disgusting = other.GetComponent<IDisgusting>();
		if (disgusting != null)
		{
			disgusting.IsOnPlate = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		IDisgusting disgusting = other.GetComponent<IDisgusting>();
		if (disgusting != null)
		{
			disgusting.IsOnPlate = false;
		}
	}
}
