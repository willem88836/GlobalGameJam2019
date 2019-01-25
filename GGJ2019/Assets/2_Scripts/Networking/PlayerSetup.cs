using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
	[SerializeField] Behaviour[] _localBehaviours;
	[SerializeField] GameObject[] _localObjects;

    void Start()
    {
		if (isLocalPlayer)
			LocalSetup();
		else
			RemoteSetup();


    }

	void LocalSetup()
	{
		Debug.Log("Setting a LOCAL player");

		for (int i = 0; i < _localBehaviours.Length; i++)
		{
			Behaviour current = _localBehaviours[i];

			if (!current.enabled)
				current.enabled = true;
		}

		for (int i = 0; i < _localObjects.Length; i++)
		{
			GameObject current = _localObjects[i];

			if (!current.activeSelf)
				current.SetActive(true);
		}
	}

	void RemoteSetup()
	{
		Debug.Log("Setting a REMOTE player");

		for (int i = 0; i < _localBehaviours.Length; i++)
		{
			Behaviour current = _localBehaviours[i];

			if (current.enabled)
				current.enabled = false;
		}

		for (int i = 0; i < _localObjects.Length; i++)
		{
			GameObject current = _localObjects[i];

			if (current.activeSelf)
				current.SetActive(false);
		}
	}
}
