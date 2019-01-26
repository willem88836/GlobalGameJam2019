using System.Collections.Generic;
using UnityEngine;
using Framework.Core;

public class ObjectiveManager : MonoBehaviour
{
	[SerializeField] private float _newObjectivesInterval = 30f;
	[SerializeField, Int2Range(0, 20)] private Int2 _ediblesRange;
	[SerializeField, Int2Range(0, 10)] private Int2 _objectiveRange;
	[SerializeField] private Transform _objectContainer;
	[SerializeField] private ObjectiveObject[] _edibles;

	[SerializeField] private TableSegmentCollection _segmentCollection;

	// TODO: instead of the int, reference to the player thingy.
	private Dictionary<int, Objective> _playerObjectives = new Dictionary<int, Objective>();


	private void Start()
	{
		StartCoroutine(ObjectiveCreationCycle());
	}

	private IEnumerator<WaitForSeconds> ObjectiveCreationCycle()
	{
		while(true)
		{
			Clear();

			Dictionary<int, Objective>.KeyCollection keys = _playerObjectives.Keys;

			Dictionary<int, Objective> newObjectives = new Dictionary<int, Objective>();

			foreach(int key in keys)
			{
				newObjectives.Add(key, CreateObjective(key));
			}

			_playerObjectives = newObjectives;

			yield return new WaitForSeconds(_newObjectivesInterval);
		}
	}


	private void Clear()
	{
		for (int i = _objectContainer.childCount - 1; i >= 0; i--)
		{
			Destroy(_objectContainer.GetChild(i).gameObject);
		}
	}


	private Objective CreateObjective(int key)
	{
		int objectiveCount = Random.Range(_objectiveRange.X, _objectiveRange.Y);
		int ediblesCount = Random.Range(_ediblesRange.X, _ediblesRange.Y) - objectiveCount;

		ObjectiveObject edible = _edibles[Random.Range(0, _edibles.Length)];

		Objective objective = new Objective()
		{
			Count = objectiveCount,
			Type = edible.Type
		};

		// TODO: instead of the key use the players' seat Index.
		SpawnSphere spawn = _segmentCollection[key].SpawnLocation;
		Vector3 pos;
		Quaternion rot;
		for (int i = 0; i < objectiveCount; i++)
		{
			spawn.GetSpawn(out pos, out rot);
			ObjectiveObject newEdible = Instantiate(edible, pos, rot, _objectContainer);
		}

		for (int i = 0; i < ediblesCount; i++)
		{
			spawn.GetSpawn(out pos, out rot);
			edible = _edibles[Random.Range(0, _edibles.Length)];
			ObjectiveObject newEdible = Instantiate(edible, pos, rot, _objectContainer);
		}

		return objective;
	}


	public void AddPlayer(int id)
	{
		if(!_playerObjectives.ContainsKey(id))
		{
			_playerObjectives.Add(id, CreateObjective(id));
		}
	}

	public void RemovePlayer(int id)
	{
		if(_playerObjectives.ContainsKey(id))
		{
			_playerObjectives.Remove(id);
		}
	}

}

public class Objective
{
	public int Type;
	public int Count;
}
