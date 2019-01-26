﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerList : MonoBehaviour
{
	static PlayerList _instance;

	private List<NetworkPlayer> _playerList;

	public UnityAction OnPlayerJoined;

	void Awake()
	{
		if (_instance != null)
		{
			Destroy(this);
			return;
		}

		_instance = this;

		_playerList = new List<NetworkPlayer>();
	}

	public void AddPlayer(NetworkPlayer player)
	{
		if (!_playerList.Contains(player))
		{
			_playerList.Add(player);

			if (OnPlayerJoined != null)
				OnPlayerJoined.Invoke();
		}
	}

	public void RemovePlayer(NetworkPlayer player)
	{
		if (_playerList.Contains(player))
			_playerList.Remove(player);
	}

	public int GetPlayerCount()
	{
		return _playerList.Count;
	}

	public List<NetworkPlayer> GetList()
	{
		return _playerList;
	}

	public static PlayerList Singleton()
	{
		return _instance;
	}
}
