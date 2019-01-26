using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabObject : MonoBehaviour
{
	// will eventually also contain information about how far the hand closes, dirt value, et cetera

	public HandGrabber GrabParent;

	Rigidbody _rigidbody;

	int _throwForceMultiplier = 2;

	public static int DEFAULTLAYER = 0; 
	public static int GRABBEDLAYER = 9;

	bool _released = false;

	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();	
	}

	public void Grab(HandGrabber parent)
	{
		GrabParent = parent;
		gameObject.layer = GRABBEDLAYER;
		_rigidbody.useGravity = false;
	}

	public void Release(Vector3 velocity)
	{
		GrabParent = null;
		gameObject.layer = DEFAULTLAYER;
		_rigidbody.useGravity = true;
		_rigidbody.velocity = velocity * _throwForceMultiplier;
	}
}
