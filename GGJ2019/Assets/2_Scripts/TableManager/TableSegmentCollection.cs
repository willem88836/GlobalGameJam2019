using UnityEngine;

public class TableSegmentCollection : MonoBehaviour
{
	public TableSegment[] Segments;

	public TableSegment this[int i] { get { return Segments[i]; } }
	public int Count { get { return Segments.Length; } }
}
