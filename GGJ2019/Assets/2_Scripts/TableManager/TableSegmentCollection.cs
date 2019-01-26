using UnityEngine;

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

			NetworkPlayer currentPlayer = _slotter.GetPlayer(currentSlot);
			if (currentPlayer == player)
				return current;
		}

		return null;
	}
}
