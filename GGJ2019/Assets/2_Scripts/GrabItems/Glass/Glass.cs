using UnityEngine;

public class Glass : GrabObject
{
	[SerializeField] GameObject _waterdropPrefab;

	Transform _spilLocation;



	float _spillTimer;
	float _spillDuration = 0.2f;
	
	public override void Start()
	{
		base.Start();

		_spilLocation = transform.GetChild(0);
	}

	void Update()
	{
		if (Grabbed)
		{
			SpilWater();
		}
	}

	void SpilWater()
	{
		_spillTimer -= Time.deltaTime;
		if (_spillTimer <= 0)
		{
			Instantiate(_waterdropPrefab, _spilLocation.position, Quaternion.identity);
			_spillTimer = _spillDuration;
		}
	}
}
