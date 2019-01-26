using UnityEngine;

public class GrabObject : MonoBehaviour
{
	// will eventually also contain information about how far the hand closes, dirt value, et cetera

	Rigidbody _rigidbody;

	int _throwForceMultiplier = 2;

	int _defaultLayer = 0; 
	int _grabbedLayer = 9;

	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	public virtual void Grab()
	{
		gameObject.layer = _grabbedLayer;
		_rigidbody.useGravity = false;
	}

	public virtual void Release(Vector3 velocity)
	{
		gameObject.layer = _defaultLayer;
		_rigidbody.useGravity = true;
		_rigidbody.velocity = velocity * _throwForceMultiplier;
	}
}
