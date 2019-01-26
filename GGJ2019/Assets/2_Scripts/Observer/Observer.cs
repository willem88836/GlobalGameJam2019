﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
	[SerializeField] private Vector2 _waitInterval;
	[SerializeField] private float _distgustThreshold;
	[SerializeField] private float _rotationSpeed;
	[SerializeField] private Transform _head;
	[SerializeField] private TableSegment[] _tableSegments;

	// TODO: reference to a list of players;
	public List<Target> PotentialTargets = new List<Target>();

	private Vector3 _originRotation;
	private List<float> _targetLastLookedAtTimeStamps = new List<float>();




	private void Start()
	{
		foreach(TableSegment segment in _tableSegments)
		{
			PotentialTargets.Add(new Target() { Position = segment.transform.position });
			_targetLastLookedAtTimeStamps.Add(Time.timeSinceLevelLoad);
		}






		_originRotation = transform.forward;
		StartCoroutine(LookBehaviour());
	}

	// is ran on server.
	private IEnumerator LookBehaviour()
	{
		while (true)
		{
			// waits for the action to start. 
			float pauseDuration = Random.Range(_waitInterval.x, _waitInterval.y);
			yield return new WaitForSeconds(pauseDuration);

			Target target = FindRandomTarget();

			// something with an animation (put on glasses or something).
			float animationDuration = 5; // TODO: change this to animation duration.
			yield return new WaitForSeconds(animationDuration);

			// rotates towards target.
			float a = 0;
			while (a < _rotationSpeed)
			{
				Rotate(_originRotation, target.Position, a);
				a += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			
			// Tests the target's disgusting table.
			int index = PotentialTargets.IndexOf(target);
			float disgustingValue = _tableSegments[index].GetDisgustingValue();
			if (disgustingValue >= _distgustThreshold)
			{
				target.Punish();
			}

			// Rotates back
			while (a > 0)
			{
				Rotate(_originRotation, target.Position, a);
				a -= Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}
	}


	private void Rotate(Vector3 o, Vector3 t, float a)
	{
		Vector3 updatedDir = Vector3.Lerp(o, t, a);
		_head.LookAt(updatedDir);
	}


	// is ran on server.
	private Target FindRandomTarget()
	{
		float oldestTimeStamp = float.MaxValue;
		int oldestTimeStampIndex = -1;

		for (int i = 0; i < PotentialTargets.Count; i++)
		{
			float current = _targetLastLookedAtTimeStamps[i];
			if (current < oldestTimeStamp)
			{
				oldestTimeStamp = current;
				oldestTimeStampIndex = i;
			}
		}

		if (oldestTimeStampIndex != -1)
		{
			_targetLastLookedAtTimeStamps[oldestTimeStampIndex] = Time.timeSinceLevelLoad;
			return PotentialTargets[oldestTimeStampIndex];
		}

		return null;
	}

}


// TODO: Replace this class with a script that actually represents a player. 
public class Target
{
	public Vector3 Position = Vector3.zero;

	public void Punish()
	{

	}
}