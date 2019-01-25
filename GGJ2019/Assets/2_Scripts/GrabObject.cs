using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabObject : MonoBehaviour
{


	// will eventually also contain information about how far the hand closes, dirt value, et cetera
	int _defaultLayer = 0; 
	int _grabbedLayer = 9;

	bool _released = false;



	public void Grab()
	{
		gameObject.layer = 9;
		_released = false;
	}

	public void PrepareRelease()
	{
		//gameObject.layer = 0;
		StartCoroutine(Release());
	}

	IEnumerator Release()
	{
		yield return new WaitUntil(()=> HasCollision() == false);
		Debug.Log("layered");
		//gameObject.layer = 0;
	}

	bool HasCollision()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, 1);
		
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].tag == "Hand")
			{
				Debug.Log("true");
				return true;
			}
		}
		Debug.Log("false");
		return false;
	}
}
