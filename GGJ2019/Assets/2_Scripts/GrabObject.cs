using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabObject : MonoBehaviour
{
	// will eventually also contain information about how far the hand closes, dirt value, et cetera

	Rigidbody _rigidbody;

	int _throwForceMultiplier = 2;

	int _defaultLayer = 0; 
	int _grabbedLayer = 9;

	bool _released = false;

	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();	
	}

	public void Grab()
	{
		gameObject.layer = _grabbedLayer;
		_rigidbody.useGravity = false;
	}

	public void Release(Vector3 velocity)
	{
		gameObject.layer = _defaultLayer;
		_rigidbody.useGravity = true;
		_rigidbody.velocity = velocity * _throwForceMultiplier;
	}
}
