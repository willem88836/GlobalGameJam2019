using UnityEngine;

public class RespawnableObject : GrabObject
{
	[SerializeField] private float _heightThreshold = -2f;

	private Vector3 _originPosition;
	private Quaternion _originRotation;


	public override void Start()
	{
		base.Start();
		_originPosition = transform.position;
		_originRotation = transform.rotation;
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
