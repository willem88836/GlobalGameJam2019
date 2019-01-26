using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSlotter : NetworkBehaviour
{

	// SERVER ONLY

	static PlayerSlotter _instance;

	[SerializeField] PlayerSlot[] _playerSlots;

	private Dictionary<PlayerSlot, NetworkPlayer> _playerDictionary;

	void Awake()
	{
		if (_instance != null)
		{
			Destroy(this);
			return;
		}

		_instance = this;

		_playerDictionary = new Dictionary<PlayerSlot, NetworkPlayer>();

		for (int i = 0; i < _playerSlots.Length; i++)
		{
			PlayerSlot current = _playerSlots[i];
			_playerDictionary.Add(current, null);
		}
	}

	PlayerSlot GetEmptySlot()
	{
		for (int i = 0; i < _playerSlots.Length; i++)
		{
			PlayerSlot current = _playerSlots[i];

			if (_playerDictionary[current] == null)
				return current;
		}

		return null;
	}

	[Server]
	public void AddPlayer(NetworkPlayer player)
	{

		PlayerSlot nextSlot = GetEmptySlot();

		if (nextSlot == null)
			return; // Spectator mode?

		_playerDictionary[nextSlot] = player;

		player.PreparePlayer(nextSlot);
	}

	[Server]
	public void RemovePlayer(NetworkPlayer player)
	{
		for (int i = 0; i < _playerSlots.Length; i++)
		{
			PlayerSlot current = _playerSlots[i];

			if (_playerDictionary[current] == player)
			{
				_playerDictionary[current] = null;
				return;
			}
		}
	}

	[Server]
	public static PlayerSlotter Singleton()
	{
		return _instance;
	}
}
