﻿using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnSphere : MonoBehaviour
{
	private const float MAXROT = 360f;

	[SerializeField] private float _spawnRadius;
	[SerializeField] private bool _randomRotation;

	
	public void GetSpawn(out Vector3 position, out Quaternion rotation)
	{
		position = transform.position + Random.insideUnitSphere * _spawnRadius;
		rotation = Quaternion.Euler((Random.insideUnitSphere) * MAXROT);
	}
}
