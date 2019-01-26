using System;
using UnityEngine;

public class Minigame_finish : MonoBehaviour
{
	public Action EndMinigame;

	LayerMask _handLayer = 10;

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == _handLayer)
		{
			if (EndMinigame != null)
				EndMinigame.Invoke();
		}
	}
}
