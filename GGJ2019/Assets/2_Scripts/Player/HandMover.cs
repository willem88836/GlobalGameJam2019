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

	[Space]
	[SerializeField] Vector3 _minMoveBoundaries;
	[SerializeField] Vector3 _maxMoveBoundaries;

	[Space]
	[Header("Rotation")]
	Vector3 _rotation;

	[SerializeField] float _rotationSpeed;
	[SerializeField] Vector2 _minRotationBoundaries;
	[SerializeField] Vector2 _maxRotationBoundaries;

	void Start()
	{
		Rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
    {
		Move();
		Rotate();
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
		transform.position = new Vector3(
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

		RotationBoundaries();

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
}
