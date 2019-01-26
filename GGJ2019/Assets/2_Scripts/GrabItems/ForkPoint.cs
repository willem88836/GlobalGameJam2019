using UnityEngine;

public class ForkPoint : MonoBehaviour
{
	GrabObject _fork;

	GrabObject _grabbedObject;

	void Start()
	{
		_fork = transform.parent.GetComponent<GrabObject>();	
	}

	private void Update()
	{
		HoldObject();
	}

	void HoldObject()
	{
		if (_grabbedObject != null)
		{
			_grabbedObject.transform.position = transform.position;

			if (_fork.Grabbed == false)
			{
				_grabbedObject.Release(Vector3.zero);
				_grabbedObject = null;
				_fork.GetComponent<Collider>().enabled = true;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (_fork.Grabbed && other.gameObject.GetComponent<GrabObject>() && _grabbedObject == null)
		{
			_grabbedObject = other.GetComponent<GrabObject>();
			_grabbedObject.Grab();
			_fork.GetComponent<Collider>().enabled = false;
		}
	}
}