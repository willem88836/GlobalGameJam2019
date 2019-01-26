using UnityEngine;

public class HeadMover : MonoBehaviour
{
	Vector3 _rotation = new Vector3();

	[SerializeField] float _speed;

	[SerializeField] Vector2 _minBoundaries;
	[SerializeField] Vector2 _maxBoundaries;

	void Update()
    {
		Rotate();
    }

	void Rotate()
	{
		float deltaSpeed = _speed * Time.deltaTime;

		float horizontalInput = Input.GetAxis("HeadHorizontal");
		_rotation.y -= deltaSpeed * horizontalInput;

		float verticalInput = Input.GetAxis("HeadVertical");
		_rotation.x -= deltaSpeed * verticalInput;
		
		RotationBoundaries();
		transform.localEulerAngles = _rotation;
	}

	void RotationBoundaries()
	{
		_rotation = new Vector3(
			Mathf.Clamp(_rotation.x,
			_minBoundaries.x,
			_maxBoundaries.x),
			Mathf.Clamp(_rotation.y,
			_minBoundaries.y,
			_maxBoundaries.y)
			);
	}
}
