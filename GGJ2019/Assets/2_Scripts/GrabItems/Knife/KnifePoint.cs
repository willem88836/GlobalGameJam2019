using UnityEngine;

public class KnifePoint : MonoBehaviour
{
	private GrabObject _knife;

	private void Awake()
	{
		_knife = transform.parent.GetComponent<GrabObject>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_knife.Grabbed)
			return;

		ISliceable sliceable = other.GetComponent<ISliceable>();
		if(sliceable != null)
		{
			sliceable.OnSlice();
		}
	}
}
