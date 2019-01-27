using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using UnityEngine.Networking;

public class ObjectiveManager : NetworkBehaviour
{
	static ObjectiveManager _instance;

	[Header("Duration")]
	[SerializeField] float _roundDuration = 30f;
	[SerializeField] float _pauseDuration = 5f;

	[Header("SpawnRanges")]
	[SerializeField] Vector2 _objectiveRange = new Vector2(2, 4);
	[SerializeField] Vector2 _spawnRange = new Vector2(6, 10);

	[Header("container")]
	[SerializeField] Transform _objectContainer;

	[Header("Prefabs")]
	[SerializeField] GameObject[] _objectivePrefabs;
	[SerializeField] GameObject[] _ediblePrefabs;

	[Header("Segments")]
	[SerializeField] TableSegmentCollection _segmentCollection;

	List<GameObject> _edibles = new List<GameObject>();

	Dictionary<TableSegment, Objective> _objectives = new Dictionary<TableSegment, Objective>();

	PlayerSlotter _slotter;

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
		_slotter = PlayerSlotter.Singleton();

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

			// PAUSE

			yield return new WaitForSeconds(_pauseDuration);

			int amount = Random.Range((int)_objectiveRange.x, (int)_objectiveRange.y);

			for (int i = 0; i < _segmentCollection.Count; i++)
			{
				_objectives[_segmentCollection[i]] = new Objective(amount);
			}

			SpawnObjects();

			yield return new WaitForSeconds(_roundDuration);

			ResetAwards();
			AssignAwards();
		}
	}

	[Server]
	void AssignAwards()
	{
		List<NetworkPlayer> dirtiestPlayers = _segmentCollection.GetDirtiesPlayers();
		for (int i = 0; i < dirtiestPlayers.Count; i++)
		{
			NetworkPlayer current = dirtiestPlayers[i];
			current.DirtiestAward();
		}

		List<NetworkPlayer> mostEatenPlayers = GetMostEatenPlayers();
		for (int i = 0; i < mostEatenPlayers.Count; i++)
		{
			NetworkPlayer current = mostEatenPlayers[i];
			current.MostEatenAward();
		}
	}

	List<NetworkPlayer> GetMostEatenPlayers()
	{
		List<NetworkPlayer> awarded = new List<NetworkPlayer>();

		int mostAmount = 0;
		
		for (int i = 0; i < _objectives.Count; i++)
		{
			TableSegment segment = _segmentCollection[i];
			Objective current = _objectives[segment];

			if (!current.HasEaten())
				continue;

			if (current.Progress > mostAmount)
			{
				mostAmount = current.Progress;
				awarded.Clear();

				NetworkPlayer player = _slotter.GetPlayer(segment);

				if (player != null)
					awarded.Add(player);
			}
			else if (current.Progress == mostAmount)
			{
				NetworkPlayer player = _slotter.GetPlayer(segment);

				if (player != null)
					awarded.Add(player);
			}
		}

		return awarded;
	}

	[Server]
	void ResetAwards()
	{
		for (int i = 0; i < _segmentCollection.Count; i++)
		{
			TableSegment current = _segmentCollection[i];

			NetworkPlayer player = _slotter.GetPlayer(current);

			if (player != null)
				player.ResetAwards();
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

		if (objective.IsComplete())
		{
			// TODO: Reward?
			//Debug.Log("A player completed the challenge");
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

	public bool HasEaten()
	{
		if (Progress > 0)
			return true;

		return false;
	}

	public bool IsComplete()
	{
		if (Progress >= Count)
			return true;

		return false;
	}
}
