using UnityEngine;

public class WaterDrop : MonoBehaviour
{
	Rigidbody _rigidbody;

    void Start()
    {
		_rigidbody = GetComponent<Rigidbody>();
		Vector3 force = new Vector3(
			Random.Range(-5, 5),
			Random.Range(3, 8),
			Random.Range(-5, 5)
			);
		_rigidbody.AddForce(force, ForceMode.VelocityChange);
    }
}
