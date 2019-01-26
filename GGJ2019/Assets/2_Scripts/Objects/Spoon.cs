using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Spoon : MonoBehaviour
{
	[SerializeField] private float _heightThreshold = -2f;

	private Vector3 _originPosition;
	private Quaternion _originRotation;


	private void Start()
	{
		_originPosition = transform.position;
		_originRotation = transform.rotation;
	}

	private void Update()
	{
		if (transform.position.y <= _heightThreshold)
		{
			transform.SetPositionAndRotation(_originPosition, _originRotation);
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}
}
