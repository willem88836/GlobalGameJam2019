using System;
using UnityEngine;

public class Minigame_bound : MonoBehaviour
{
	LayerMask _handLayer = 10;

	public Action ResetMinigame;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == _handLayer)
		{
			if (ResetMinigame != null)
				ResetMinigame.Invoke();
		}
	}
}
