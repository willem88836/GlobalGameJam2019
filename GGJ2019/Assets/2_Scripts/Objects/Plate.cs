using UnityEngine;

public class Plate : MonoBehaviour // TODO: Make this grabbable.
{
	[SerializeField] private float _heightThreshold = -2f;

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

	private void Update()
	{
		if (transform.position.y <= _heightThreshold)
		{
			transform.SetPositionAndRotation(_originPosition, _originRotation);
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}
}
