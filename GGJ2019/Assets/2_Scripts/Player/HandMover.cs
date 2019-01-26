using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HandMover : MonoBehaviour
{
	[HideInInspector] public Rigidbody Rigidbody;

	[SerializeField] float _speed;

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
		float deltaSpeed = _speed * Time.deltaTime;

		// Move forward and backward
		float forwardInput = Input.GetAxis("Horizontal1");
		Rigidbody.AddForce(Vector3.right * forwardInput * deltaSpeed);
		
		// Move left and right
		float sidewaysInput = Input.GetAxis("Horizontal2");
		Rigidbody.AddForce(Vector3.forward * sidewaysInput * deltaSpeed);

		// Move up 
		if (Input.GetAxisRaw("LeftTrigger") < 0)
			Rigidbody.AddForce(Vector3.up * deltaSpeed);
		else if (Input.GetKey(KeyCode.Joystick1Button4))
			Rigidbody.AddForce(Vector3.down * deltaSpeed);

			// TODO set a max velocity here if needed
	}
}
