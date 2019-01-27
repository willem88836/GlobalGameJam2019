using UnityEngine;

public class Glass : GrabObject
{
	[SerializeField] GameObject _waterdropPrefab;

	Transform _spilLocation;

	Quaternion _defaultRotation;

	float _spillTimer;
	float _spillDuration = 0.2f;

	float _releaseTimer = 0;
	float _releaseDuration = 2;

	public override void Start()
	{
		base.Start();

		_spilLocation = transform.GetChild(0);

		_defaultRotation = transform.rotation;
	}

	void Update()
	{
		if (Grabbed || _releaseTimer > 0)
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

			// Keep going for a while after not being grabbed
			_releaseTimer -= Time.deltaTime;

			Debug.Log(_releaseTimer);

			if (Grabbed)
				_releaseTimer = _releaseDuration;
		}
	}
}
