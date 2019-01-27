using UnityEngine;

public class ForkPoint : ToolPoint<IForkable>
{
	GrabObject _grabbedObject;
	
	protected override void Invoke(IForkable obj)
	{
		obj.OnFork();
		_grabbedObject = (GrabObject)obj;
	}

	private void Update()
	{
		if(_grabbedObject != null)
		{
			_grabbedObject.transform.position = transform.position;

			if(!MyGrabObject.Grabbed)
			{
				_grabbedObject.Release(Vector3.zero);
				_grabbedObject = null;
				_grabbedObject.gameObject.layer = GrabObject.GRABBEDLAYER;
				_grabbedObject.GetComponent<Collider>().enabled = true;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		IForkable forkable = other.GetComponent<IForkable>();
		if (MyGrabObject.Grabbed && _grabbedObject == null && forkable != null)
		{
			other.enabled = false;
			_grabbedObject = (GrabObject)forkable;
			_grabbedObject.Grab();
		}
	}
}