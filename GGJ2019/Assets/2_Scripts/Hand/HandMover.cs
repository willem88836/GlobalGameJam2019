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
		float speed = _speed * Time.deltaTime;

		// Move forward and backward
		float forwardInput = Input.GetAxis("Horizontal1");
		_rigidbody.AddForce(Vector3.right * forwardInput * speed);
		
		// Move left and right
		float sidewaysInput = Input.GetAxis("Horizontal2");
		_rigidbody.AddForce(Vector3.forward * sidewaysInput * speed);

		// Move up 
		if (Input.GetAxisRaw("LeftTrigger") < 0)
			_rigidbody.AddForce(Vector3.up * speed);
		else if (Input.GetKey(KeyCode.Joystick1Button4))
			_rigidbody.AddForce(Vector3.down * speed);


			// TODO set a max velocity here if needed
		}
}
