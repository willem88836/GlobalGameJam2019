using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabObject : MonoBehaviour
{
	Rigidbody _rigidbody;	

	// will eventually also contain information about how far the hand closes, dirt value, et cetera
	int _defaultLayer = 0; 
	int _grabbedLayer = 9;

	bool _released = false;

	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();	
	}

	public void Grab()
	{
		gameObject.layer = 9;
		_rigidbody.useGravity = false;
	}

	public void Release(Vector3 velocity)
	{
		gameObject.layer = 0;
		_rigidbody.useGravity = true;
		_rigidbody.velocity = velocity;
	}
}
