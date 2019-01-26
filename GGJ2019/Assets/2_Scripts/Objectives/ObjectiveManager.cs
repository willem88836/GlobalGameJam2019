using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using UnityEngine.Networking;

public class ObjectiveManager : NetworkBehaviour
{
	static ObjectiveManager _instance;

	[SerializeField] private float _newObjectivesInterval = 30f;

	Vector2 _objectiveRange = new Vector2(2, 4);
	Vector2 _spawnRange = new Vector2(6, 10);
	//[SerializeField] private Int2 _ediblesRange;
	//[SerializeField] private Int2 _objectiveRange;

	[SerializeField] private Transform _objectContainer;

	[SerializeField] private GameObject[] _objectivePrefabs;
	[SerializeField] private GameObject[] _ediblePrefabs;

	[SerializeField] private TableSegmentCollection _segmentCollection;

	List<GameObject> _edibles = new List<GameObject>();

	private Dictionary<TableSegment, Objective> _objectives = new Dictionary<TableSegment, Objective>();

	void Awake()
	{
		if (_instance != null)
		{
			Destroy(this);
			return;
		}

		_instance = this;
	}

	void Start()
	{

		if (isServer)
		{
			RegisterSegments();
			StartCoroutine(ObjectiveCreationCycle());
		}
	}

	void RegisterSegments()
	{
		for (int i = 0; i <_segmentCollection.Count; i++)
		{
			_objectives.Add(_segmentCollection[i], null);
		}
	}

	[Server]
	private IEnumerator<WaitForSeconds> ObjectiveCreationCycle()
	{
		for (; ; )
		{
			Clear();

			//Dictionary<NetworkPlayer, Objective>.KeyCollection keys = _objectives.Keys;

			//Dictionary<NetworkPlayer, Objective> newObjectives = new Dictionary<NetworkPlayer, Objective>();

			int amount = Random.Range((int)_objectiveRange.x, (int)_objectiveRange.y);

			for (int i = 0; i < _segmentCollection.Count; i++)
			{
				_objectives[_segmentCollection[i]] = new Objective(amount);
			}

			SpawnObjects();

			yield return new WaitForSeconds(_newObjectivesInterval);
		}
	}

	[Server]
	public void OnAteEdible(NetworkPlayer player)
	{
		TableSegment segment = _segmentCollection.GetSegment(player);

		if (segment == null)
			return;

		Objective objective = _objectives[segment];
		objective.Progress++;

		Debug.Log(">> Player ate <<");

		if (objective.IsComplete())
		{
			// TODO: Reward?
			Debug.Log("A player completed the challenge");
		}
	}
	//private void OnComplete(IObjective completed)
	//{
	//	_objectives[completed.Player].Progress++;
	//	// TODO: somethign with points and effects or something	? 
	//}

	[Server]
	private void Clear()
	{
		for (int i = 0; i < _edibles.Count; i++)
		{
			GameObject current = _edibles[i];

			if (current != null)
				Destroy(current);
		}

		_edibles.Clear();
	}

	void SpawnObjects()
	{
		int amount = Random.Range((int)_spawnRange.x, (int)_spawnRange.y);

		for (int i = 0; i < _segmentCollection.Count; i++)
		{
			TableSegment current = _segmentCollection[i];
			SpawnSphere spawner = current.SpawnLocation;

			Vector3 pos;
			Quaternion rot;

			for (int j = 0; j < amount; j++)
			{
				spawner.GetSpawn(out pos, out rot);
				GameObject newObject = Instantiate(GetRandomEdiblePrefab(), pos, rot, _objectContainer);
				_edibles.Add(newObject);

				// IObjective stuff

				NetworkServer.Spawn(newObject);

			}
		}
	}

	GameObject GetRandomEdiblePrefab()
	{
		return _ediblePrefabs[Random.Range(0, _ediblePrefabs.Length)];
	}

	[Server]
	public static ObjectiveManager Singleton()
	{
		return _instance;
	}

	/*
	private Objective CreateObjective(NetworkPlayer key)
	{
		int objectiveCount = Random.Range(_objectiveRange.X, _objectiveRange.Y);
		int ediblesCount = Random.Range(_ediblesRange.X, _ediblesRange.Y) - objectiveCount;

		GameObject EdibleObjectivePrefab = _objectivePrefabs[Random.Range(0, _ediblePrefabs.Length)];

		IObjective objectiveObject = EdibleObjectivePrefab.GetComponent<IObjective>();
		Objective objective = new Objective()
		{
			Count = objectiveCount,
			Type = objectiveObject.Type,
			Progress = 0,
		};

		// TODO: instead of the key use the players' seat Index.
		SpawnSphere spawn = _segmentCollection.GetSegment(key).SpawnLocation;
		Vector3 pos;
		Quaternion rot;
		for (int i = 0; i < objectiveCount; i++)
		{
			spawn.GetSpawn(out pos, out rot);
			GameObject newObjectiveObject = Instantiate(EdibleObjectivePrefab, pos, rot, _objectContainer);
			IObjective newObjective = newObjectiveObject.GetComponent<IObjective>();
			newObjective.Type = objective.Type;
			newObjective.Player = key;
			newObjective.OnComplete = OnComplete;

			NetworkServer.Spawn(newObjectiveObject);
		}

		for (int i = 0; i < ediblesCount; i++)
		{
			spawn.GetSpawn(out pos, out rot);
			GameObject prefab = _ediblePrefabs[Random.Range(0, _ediblePrefabs.Length)];
			GameObject newObject = Instantiate(prefab, pos, rot, _objectContainer);

			NetworkServer.Spawn(newObject);
		}

		return objective;
	}
	*/
}

public class Objective
{
	//public int Type;
	public int Count;
	public int Progress;

	public Objective(int amount)
	{
		Count = amount;
		Progress = 0;
	}

	public bool IsComplete()
	{
		if (Progress >= Count)
			return true;

		return false;
	}
}
