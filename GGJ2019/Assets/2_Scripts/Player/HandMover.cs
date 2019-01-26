using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HandMover : MonoBehaviour
{
	[HideInInspector] public Rigidbody Rigidbody;

	[SerializeField] Transform _head;
	[SerializeField] Collider _tableZone;

	[SerializeField] float _speedHorizontal;
	[SerializeField] float _speedVertical;

	Vector3 _velocity;

	Vector3 _smoothVelocity = Vector3.zero;
	[SerializeField] float _dampTime = 0.5f;

	[Space]
	Vector3 _minMoveBoundaries;
	Vector3 _maxMoveBoundaries;

	[Space]
	[Header("Rotation")]
	Vector3 _rotation;

	[SerializeField] float _rotationSpeed;
	[SerializeField] Vector2 _minRotationBoundaries;
	[SerializeField] Vector2 _maxRotationBoundaries;

	void Start()
	{
		Rigidbody = GetComponent<Rigidbody>();

		SetMoveBoundaries();
	}

	void Update()
    {
		Move();
		Rotate();
	}

	void SetMoveBoundaries()
	{
		_minMoveBoundaries.x = _tableZone.bounds.min.x;
		_maxMoveBoundaries.x = _tableZone.bounds.max.x;

		_minMoveBoundaries.y = _tableZone.bounds.min.y;
		_maxMoveBoundaries.y = _tableZone.bounds.max.y;

		_minMoveBoundaries.z = _tableZone.bounds.min.z;
		_maxMoveBoundaries.z = _tableZone.bounds.max.z;
	}

	void Move()
	{
		float moveZ = Input.GetAxis("Horizontal2");
		float moveX = Input.GetAxis("Horizontal1");

		float moveY = 0;
		if (Input.GetAxisRaw("LeftTrigger") < 0)
			moveY = -1;
		else if (Input.GetKey(KeyCode.Joystick1Button4))
			moveY = 1;
		
		// makes sure to aim the hand based on head-rotation
		Quaternion relativeRotation = new Quaternion(
			0,
			_head.rotation.y,
			0,
			_head.rotation.w
			);

		_velocity = relativeRotation * new Vector3(
			moveX * _speedHorizontal,
			moveY * _speedVertical,
			moveZ * _speedHorizontal);

		Rigidbody.velocity = Vector3.SmoothDamp(Rigidbody.velocity, _velocity, ref _smoothVelocity, _dampTime);

		MovementBoundaries();
	}

	void MovementBoundaries()
	{
		transform.localPosition = new Vector3(
			Mathf.Clamp(transform.position.x,
			_minMoveBoundaries.x,
			_maxMoveBoundaries.x),
			Mathf.Clamp(transform.position.y,
			_minMoveBoundaries.y,
			_maxMoveBoundaries.y),
			Mathf.Clamp(transform.position.z,
			_minMoveBoundaries.z,
			_maxMoveBoundaries.z)
			);
	}

	void Rotate()
	{
		// also get the y rotation from the head
		_rotation = new Vector3(
			transform.localEulerAngles.x,
			_head.localEulerAngles.y,
			transform.localEulerAngles.z);

		float deltaSpeed = _rotationSpeed * Time.deltaTime;

		float horizontalInput = Input.GetAxis("DPadHorizontal");
		_rotation.z += deltaSpeed * horizontalInput;

		float verticalInput = Input.GetAxis("DPadVertical");
		_rotation.x -= deltaSpeed * verticalInput;

		//RotationBoundaries();

		transform.localEulerAngles = _rotation;
	}

	void RotationBoundaries()
	{
		_rotation = new Vector3(
			Mathf.Clamp(_rotation.x,
			_minRotationBoundaries.x,
			_maxRotationBoundaries.x),
			_rotation.y,
			Mathf.Clamp(_rotation.z,
			_minRotationBoundaries.y,
			_maxRotationBoundaries.y)
			);
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
