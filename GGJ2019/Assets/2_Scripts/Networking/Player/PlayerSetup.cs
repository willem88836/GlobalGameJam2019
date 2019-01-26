using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
	[SerializeField] Behaviour[] _localBehaviours;
	[SerializeField] GameObject[] _localObjects;

	PlayerList _playerList;
	PlayerSlotter _playerSlotter;
	NetworkPlayer _networkPlayer;

    void Start()
    {
		_playerList = PlayerList.Singleton();
		_networkPlayer = GetComponent<NetworkPlayer>();

		_playerList.AddPlayer(_networkPlayer);

		if (isLocalPlayer)
			LocalSetup();
		else
			RemoteSetup();

		if (isServer)
		{
			_playerSlotter = PlayerSlotter.Singleton();
			_playerSlotter.AddPlayer(_networkPlayer);
		}
	}

	void OnDestroy()
	{
		//Debug.Log("A player has left");
		_playerList.RemovePlayer(_networkPlayer);

		if (isServer)
		{
			_playerSlotter.RemovePlayer(_networkPlayer);
		}
	}

	void LocalSetup()
	{
		//Debug.Log("A LOCAL player has joined");

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
		//Debug.Log("A REMOTE player has joined");

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
