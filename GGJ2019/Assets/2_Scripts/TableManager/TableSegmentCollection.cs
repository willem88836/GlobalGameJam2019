using UnityEngine;
using System.Collections.Generic;

public class TableSegmentCollection : MonoBehaviour
{
	public TableSegment[] Segments;

	public TableSegment this[int i] { get { return Segments[i]; } }
	public int Count { get { return Segments.Length; } }

	PlayerSlotter _slotter;

	void Start()
	{
		_slotter = PlayerSlotter.Singleton();
	}

	public TableSegment GetSegment(NetworkPlayer player)
	{
		for (int i = 0; i < Segments.Length; i++)
		{
			TableSegment current = Segments[i];

			PlayerSlot currentSlot = current.GetSlot();

			if (_slotter == null)
				_slotter = PlayerSlotter.Singleton();

			NetworkPlayer currentPlayer = _slotter.GetPlayer(currentSlot);

			if (currentPlayer == null)
				continue;

			if (currentPlayer == player)
				return current;
		}

		return null;
	}

	public TableSegment GetDirtiestSegment(bool playerRequired)
	{
		int _dirtIndex = -1;
		float _dirt = 0;

		for (int i = 0; i < Segments.Length; i++)
		{
			TableSegment current = Segments[i];

			NetworkPlayer player = _slotter.GetPlayer(current.GetSlot());
			if (playerRequired && player == null)
				continue;

			float disgustingValue = current.GetDisgustingValue();
			if (disgustingValue > 0)
			{
				_dirt = disgustingValue;
				_dirt = i;
			}
		}

		if (_dirtIndex < 0)
			return null;

		return Segments[_dirtIndex];
	}

	public List<NetworkPlayer> GetDirtiesPlayers()
	{
		List<NetworkPlayer> awarded = new List<NetworkPlayer>();
		float dirtValue = 0;

		for (int i = 0; i < Segments.Length; i++)
		{
			TableSegment current = Segments[i];
			NetworkPlayer player = _slotter.GetPlayer(current);

			if (player == null)
				continue;

			float value = current.GetDisgustingValue();

			if (value > dirtValue)
			{
				dirtValue = value;
				awarded.Clear();

				awarded.Add(player);
			}
			else if (value == dirtValue)
			{
				awarded.Add(player);
			}
		}

		return awarded;
	}
}
