using System;
using UnityEngine;
using UnityEngine.Networking;

public class Egg : NetworkBehaviour, ISpoonable, IObjective
{
	public int Type { get; set; }
	public NetworkPlayer Player { get; set; }
	public Action<IObjective> OnComplete { get; set; }


	[SerializeField] private GameObject _eggHolder;
	[SerializeField] private GameObject _egg;


	public void OnSpoon()
	{
		GameObject eggHolder = Instantiate(_eggHolder, transform.position + Vector3.left, Quaternion.identity, transform.parent);
		NetworkServer.Spawn(eggHolder);

		GameObject egg = Instantiate(_egg, transform.position + Vector3.right, Quaternion.identity, transform.parent);
		IObjective obj = egg.GetComponent<IObjective>();
		obj.Type = this.Type;
		obj.Player = this.Player;
		obj.OnComplete = this.OnComplete;

		NetworkServer.Spawn(egg);

		NetworkServer.Destroy(this.gameObject);
	}
}
