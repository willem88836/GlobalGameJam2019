using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
	static PlayerList _instance;

	private List<NetworkPlayer> _playerList;

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
			_playerList.Add(player);
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

	public static PlayerList Singleton()
	{
		return _instance;
	}
}
