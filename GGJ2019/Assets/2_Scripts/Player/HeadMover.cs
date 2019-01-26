using UnityEngine;

public class HeadMover : MonoBehaviour
{
	Vector3 rotation = new Vector3();

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
		//transform.Rotate(Vector3.down * deltaSpeed * horizontalInput, Space.World);
		rotation.y -= deltaSpeed * horizontalInput;

		float verticalInput = Input.GetAxis("HeadVertical");
		//transform.Rotate(Vector3.left * deltaSpeed * verticalInput, Space.Self);
		rotation.x -= deltaSpeed * verticalInput;
		transform.localEulerAngles = rotation;
		
		RotationBoundaries();
		transform.localEulerAngles = rotation;
	}

	void RotationBoundaries()
	{
		rotation = new Vector3(
			Mathf.Clamp(rotation.x,
			_minBoundaries.x,
			_maxBoundaries.x),
			Mathf.Clamp(rotation.y,
			_minBoundaries.y,
			_maxBoundaries.y)
			);
	}
}
