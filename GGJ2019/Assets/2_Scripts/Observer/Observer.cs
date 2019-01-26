using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Observer : NetworkBehaviour
{
	[SerializeField] private Vector2 _waitInterval;
	[SerializeField] private float _distgustThreshold;
	[SerializeField] private float _rotationTime;
	[SerializeField] private Transform _head;
	[SerializeField] private TableSegment[] _tableSegments;

	private Vector3 _originRotation;

	PlayerSlotter _slotter;
	//private List<float> _targetLastLookedAtTimeStamps = new List<float>();

	private void Start()
	{
		//foreach(TableSegment segment in _tableSegments)
		//{
		//	PotentialTargets.Add(new Target() { Position = segment.transform.position });
		//	//_targetLastLookedAtTimeStamps.Add(Time.timeSinceLevelLoad);
		//}

		_slotter = PlayerSlotter.Singleton();

		_originRotation = transform.forward;

		if (isServer)
			StartCoroutine(LookBehaviour());
	}

	[Server]
	private IEnumerator LookBehaviour()
	{
		for (; ; )
		{
			// waits for the action to start. 
			float pauseDuration = Random.Range(_waitInterval.x, _waitInterval.y);
			yield return new WaitForSeconds(pauseDuration);

			// something with an animation (put on glasses or something).
			float animationDuration = 5; // TODO: change this to animation duration.
			yield return new WaitForSeconds(animationDuration);

			// rotates towards target.
			RpcRotate(_originRotation, _originRotation + Vector3.up, _rotationTime);
			yield return Rotate(_originRotation, _originRotation + Vector3.up, _rotationTime);

			CheckSegments();

			// Rotates back
			RpcRotate(_originRotation + Vector3.up, _originRotation, _rotationTime);
			yield return Rotate(_originRotation + Vector3.up, _originRotation, _rotationTime);
		}
	}

	[Server]
	void CheckSegments()
	{
		Debug.Log("Checking Segments");

		for (int i = 0; i < _tableSegments.Length; i++)
		{
			TableSegment current = _tableSegments[i];
			if (current.GetDisgustingValue() > _distgustThreshold)
			{
				NetworkPlayer player = _slotter.GetPlayer(current.GetSlot());

				if (player == null)
					continue;

				player.PunishPlayer();
				// TODO: Punish player connected to segment
			}
		}
	}

	[ClientRpc]
	private void RpcRotate(Vector3 start, Vector3 end, float time)
	{
		StartCoroutine(Rotate(start, end, time));
	}

	IEnumerator Rotate(Vector3 start, Vector3 end, float time)
	{
		float timer = 0.0f;

		while (timer < time)
		{
			yield return null;

			timer += Time.deltaTime;
			float percent = timer / time;

			//Debug.Log(percent);

			Vector3 current = Vector3.Lerp(start, end, percent);
			_head.forward = current;
			//_head.LookAt(current);
		}

		_head.forward = end;
	}

	/*
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
	*/
}