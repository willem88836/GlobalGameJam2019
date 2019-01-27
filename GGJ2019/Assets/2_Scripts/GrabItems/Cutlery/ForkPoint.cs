using UnityEngine;

public class ForkPoint : ToolPoint<IForkable>
{
	GrabableObject _grabbedObject;
	
	protected override void Invoke(IForkable obj)
	{
		obj.OnFork();
		_grabbedObject = (GrabableObject)obj;
	}

	private void Update()
	{
		if(_grabbedObject != null)
		{
			if (_grabbedObject.gameObject == null)
			{
				_grabbedObject = null;
				return;
			}

			_grabbedObject.transform.position = transform.position;

			if(!MyGrabObject.IsGrabbed())
			{
				_grabbedObject.OnRelease(Vector3.zero);
				_grabbedObject = null;
				_grabbedObject.gameObject.layer = GrabObject.GRABBEDLAYER;
				_grabbedObject.GetComponent<Collider>().enabled = true;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		IForkable forkable = other.GetComponent<IForkable>();
		if (MyGrabObject.IsGrabbed() && _grabbedObject == null && forkable != null)
		{
			other.enabled = false;
			_grabbedObject = (GrabableObject)forkable;
			_grabbedObject.OnGrab(transform, MyGrabObject._playerGrabber);
		}
	}
}