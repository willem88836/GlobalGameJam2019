using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerEater : NetworkBehaviour
{
	[SerializeField] EatArea _eatArea;
	[SerializeField] float _eatCooldown = 1.0f;

	ObjectiveManager _objectiveManager;
	NetworkPlayer _player;

	private void Awake()
	{
		_player = GetComponent<NetworkPlayer>();
	}

	void Start()
	{
		if (isServer)
		{
			_objectiveManager = ObjectiveManager.Singleton();
			StartCoroutine(EatCheck());
		}
	}

	[Server]
	IEnumerator EatCheck()
	{
		for (; ; )
		{
			yield return null;

			Transform edibleTransform = _eatArea.GetClosestEdible();

			if (edibleTransform == null)
				continue;

			IEdible edible = edibleTransform.GetComponent<IEdible>();

			if (edible == null)
				continue;


			_objectiveManager.OnAteEdible(_player);
			edible.OnEat();

			yield return new WaitForSeconds(_eatCooldown);
		}
	}
}
