using UnityEngine;

public class HeadMover : MonoBehaviour
{
	[SerializeField] float _speed;

    void Update()
    {
		Move();
    }

	void Move()
	{
		float deltaSpeed = _speed * Time.deltaTime;

		float horizontalInput = Input.GetAxis("HeadHorizontal");
		transform.Rotate(Vector3.down * deltaSpeed * horizontalInput, Space.World);

		float verticalInput = Input.GetAxis("HeadVertical");
		transform.Rotate(Vector3.left * deltaSpeed * verticalInput, Space.Self);
	}
}
