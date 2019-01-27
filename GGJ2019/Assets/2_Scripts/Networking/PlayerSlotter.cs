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
	public NetworkPlayer GetPlayer(PlayerSlot slot)
	{
		return _playerDictionary[slot];
	}

	[Server]
	public NetworkPlayer GetPlayer(TableSegment segment)
	{
		return GetPlayer(segment.GetSlot());
	}
	[Server]
	public PlayerSlot GetSlot(NetworkPlayer player)
	{
		List<PlayerSlot> keys = new List<PlayerSlot>(_playerDictionary.Keys);

		for (int i = 0; i < _playerDictionary.Count; i++)
		{
			if (_playerDictionary[keys[i]] == player)
				return keys[i];
		}

		return null;
	}

	//[Server]
	public static PlayerSlotter Singleton()
	{
		return _instance;
	}
}
