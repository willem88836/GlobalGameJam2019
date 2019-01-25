using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
	private const float MINANGLEDELTA = 0.005f;


	[SerializeField] private Vector2 _waitInterval;
	[SerializeField] private float _rotationSpeed;
	[SerializeField] private Transform _head;


	private Quaternion _originRotation;
	private List<float> _targetLastLookedAtTimeStamps = new List<float>();




	private void Start()
	{
		_originRotation = transform.rotation;
	}

	// is ran on server.
	private IEnumerator WaitToLook()
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
			Quaternion rotationTarget = Quaternion.Euler(target.Position - _head.position);
			float rotDistance = Quaternion.Angle(_head.rotation, rotationTarget);
			while(rotDistance > MINANGLEDELTA)
			{
				Rotate(_head.rotation, rotationTarget, ref rotDistance);
				yield return new WaitForEndOfFrame();
			}

			// Tests the target's disgusting table.
			if (target.MyTable.IsDisgusting())
			{
				target.Punish();
			}


			// Rotates back
			rotDistance = Quaternion.Angle(_head.rotation, _originRotation);
			while (rotDistance > MINANGLEDELTA)
			{
				Rotate(_head.rotation, _originRotation, ref rotDistance);
				yield return new WaitForEndOfFrame();
			}
		}
	}


	private void Rotate(Quaternion o, Quaternion t, ref float d)
	{
		Quaternion updatedRotation = Quaternion.RotateTowards(o, t, _rotationSpeed);
		_head.rotation = updatedRotation;
		d = Quaternion.Angle(_head.rotation, t);
	}


	// is ran on server.
	private Target FindRandomTarget()
	{
		// TODO: reference to a list of players;
		List<Target> potentialTargets = new List<Target>();



		float oldestTimeStamp = float.MaxValue;
		int oldestTimeStampIndex = -1;

		for (int i = 0; i < potentialTargets.Count; i++)
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
			return potentialTargets[oldestTimeStampIndex];
		}

		return null;
	}

}


// TODO: Replace this class with a script that actually represents a player. 
public class Target
{
	public class Table
	{
		public bool IsDisgusting()
		{
			return true;
		}
	}

	public Table MyTable = new Table();
	public Vector3 Position = Vector3.zero;


	public void Punish()
	{

	}
}