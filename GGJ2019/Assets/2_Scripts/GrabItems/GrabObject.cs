using UnityEngine;

public class GrabObject : MonoBehaviour
{
	// will eventually also contain information about how far the hand closes, dirt value, et cetera

	Rigidbody _rigidbody;

	int _throwForceMultiplier = 2;

	public static int DEFAULTLAYER = 0; 
	public static int GRABBEDLAYER = 9;

	[HideInInspector] public bool Grabbed; // Mayhaps just replace this to GrabObject?


	public virtual void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	public virtual void Grab()
	{
		gameObject.layer = GRABBEDLAYER;
		if (_rigidbody)
			_rigidbody.useGravity = false;
		Grabbed = true;
	}

	public virtual void Release(Vector3 velocity)
	{
		gameObject.layer = DEFAULTLAYER;
		if (_rigidbody)
		{
			_rigidbody.useGravity = true;
			_rigidbody.velocity = velocity * _throwForceMultiplier;
		}
		Grabbed = false;
	}
}
