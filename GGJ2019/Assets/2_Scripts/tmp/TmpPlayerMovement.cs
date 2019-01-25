using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpPlayerMovement : MonoBehaviour
{
	[SerializeField] float _moveSpeed = 5.0f;

	Rigidbody _rigid;
	Vector3 _velocity = Vector3.zero;

	void Awake()
	{
		_rigid = GetComponent<Rigidbody>();
	}

	void Start()
    {
        
    }

    void Update()
    {
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

		Vector3 direction = new Vector3(moveX, 0.0f, moveZ);
		direction.Normalize();

		_velocity = direction * _moveSpeed;
		_velocity.y += _rigid.velocity.y;

		ApplyVelocity();
    }

	void ApplyVelocity()
	{
		_rigid.velocity = _velocity;
	}
}
