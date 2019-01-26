using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HandMover : MonoBehaviour
{
	[HideInInspector] public Rigidbody Rigidbody;

	[SerializeField] Transform _head;

	[SerializeField] float _speedHorizontal;
	[SerializeField] float _speedVertical;
	//[SerializeField] float _breakSpeed;

	Vector3 _velocity;

	Vector3 _smoothVelocity = Vector3.zero;
	[SerializeField] float _dampTime = 0.5f;

	void Start()
	{
		Rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
    {
		Move();
    }

	void Move()
	{
		float moveZ = Input.GetAxis("Horizontal2");
		float moveX = Input.GetAxis("Horizontal1");

		float moveY = 0;
		if (Input.GetAxisRaw("LeftTrigger") < 0)
			moveY = 1;
		else if (Input.GetKey(KeyCode.Joystick1Button4))
			moveY = -1;
		
		// makes sure to aim the hand based on head-rotation
		Quaternion _relativeRotation = new Quaternion(
			0,
			_head.rotation.y,
			0,
			_head.rotation.w
			);

		_velocity = _relativeRotation * new Vector3(
			moveX * _speedHorizontal,
			moveY * _speedVertical,
			moveZ * _speedHorizontal);

		Rigidbody.velocity = Vector3.SmoothDamp(Rigidbody.velocity, _velocity, ref _smoothVelocity, _dampTime);
	}

	/*
		void MoveOld()
		{
		float deltaSpeed = _speed * Time.deltaTime;

			// Move forward and backward
			float forwardInput = Input.GetAxis("Horizontal1");
			Rigidbody.AddForce(_head.right * forwardInput * deltaSpeed);

			// Move left and right
			float sidewaysInput = Input.GetAxis("Horizontal2");
			Rigidbody.AddForce(_head.forward * sidewaysInput * deltaSpeed);

			// Move up 
			if (Input.GetAxisRaw("LeftTrigger") < 0)
				Rigidbody.AddForce(Vector3.up * deltaSpeed);
			else if (Input.GetKey(KeyCode.Joystick1Button4))
				Rigidbody.AddForce(Vector3.down * deltaSpeed);

				// TODO set a max velocity here if needed
		}*/

	/*
	
	float deltaSpeed = _speed * Time.deltaTime;
		float deltaBreakSpeed = _breakSpeed + Time.deltaTime;

		// Move forward and backward
		float xInput = Input.GetAxis("Horizontal1");
		if (xInput != 0)
		{
			_velocity.x += deltaSpeed * xInput;
		}
		else if (_velocity.x != 0)
		{
			_velocity.x /= deltaBreakSpeed;
		}

		// Move left and right
		float zInput = Input.GetAxis("Horizontal2");
		if (zInput != 0)
		{
			_velocity.z += deltaSpeed * zInput;
		}
		else if (_velocity.z != 0)
		{
			_velocity.z /= deltaBreakSpeed;
		}

		// Move up 
		if (Input.GetAxisRaw("LeftTrigger") < 0)
			_velocity.y += deltaSpeed;
		else if (Input.GetKey(KeyCode.Joystick1Button4))
			_velocity.y -= deltaSpeed;
		else if (_velocity.y != 0)
		{
			_velocity.y /= deltaBreakSpeed;
		}

	  */
}
