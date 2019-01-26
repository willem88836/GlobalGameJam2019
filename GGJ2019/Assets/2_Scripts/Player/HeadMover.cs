using UnityEngine;

public class HeadMover : MonoBehaviour
{
	Vector3 _rotation = new Vector3();

	[SerializeField] float _speed;

	[SerializeField] Vector2 _minBoundaries;
	[SerializeField] Vector2 _maxBoundaries;

	void Start()
	{
		_rotation = transform.localEulerAngles;
		_minBoundaries.x += _rotation.x;
		_maxBoundaries.x += _rotation.x;
		_minBoundaries.y += _rotation.y;
		_maxBoundaries.y += _rotation.y;
	}

	void Update()
    {
		Rotate();
    }

	void Rotate()
	{
		float deltaSpeed = _speed * Time.deltaTime;

		float horizontalInput = Input.GetAxis("HeadHorizontal");
		_rotation.y -= deltaSpeed * horizontalInput;
		_rotation.y = Mathf.Clamp(_rotation.y, _minBoundaries.y, _maxBoundaries.y);

		float verticalInput = Input.GetAxis("HeadVertical");
		_rotation.x -= deltaSpeed * verticalInput;
		_rotation.x = Mathf.Clamp(_rotation.x, _minBoundaries.x, _maxBoundaries.x);

		RotationBoundaries();
		transform.localEulerAngles = _rotation;
	}

	void RotationBoundaries()
	{
		
	}
}
