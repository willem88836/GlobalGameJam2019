using UnityEngine;

public class ForkPoint : ToolPoint<IForkable>
{
	GrabableObject _grabbedObject;
	
	protected override void Invoke(IForkable obj)
	{
		obj.OnFork();
		_grabbedObject = (obj as MonoBehaviour).GetComponent<GrabableObject>();
		_grabbedObject.OnGrab(transform, MyGrabObject._playerGrabber);
		_grabbedObject.GetComponent<Collider>().enabled = false;
		_grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
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
				Drop();
			}
		}
	}

	private void Drop()
	{
		_grabbedObject.OnRelease(Vector3.zero);
		_grabbedObject = null;
		_grabbedObject.gameObject.layer = GrabObject.GRABBEDLAYER;
		_grabbedObject.GetComponent<Collider>().enabled = true;
		_grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
	}
}