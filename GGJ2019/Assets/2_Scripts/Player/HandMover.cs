using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HandMover : MonoBehaviour
{
	Rigidbody _rigidbody;

	[SerializeField] float _speed;

	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
    {
		Move();
    }

	void Move()
	{
		float deltaSpeed = _speed * Time.deltaTime;

		// Move forward and backward
		float forwardInput = Input.GetAxis("Horizontal1");
		_rigidbody.AddForce(Vector3.right * forwardInput * deltaSpeed);
		
		// Move left and right
		float sidewaysInput = Input.GetAxis("Horizontal2");
		_rigidbody.AddForce(Vector3.forward * sidewaysInput * deltaSpeed);

		// Move up 
		if (Input.GetAxisRaw("LeftTrigger") < 0)
			_rigidbody.AddForce(Vector3.up * deltaSpeed);
		else if (Input.GetKey(KeyCode.Joystick1Button4))
			_rigidbody.AddForce(Vector3.down * deltaSpeed);

			// TODO set a max velocity here if needed
	}
}
