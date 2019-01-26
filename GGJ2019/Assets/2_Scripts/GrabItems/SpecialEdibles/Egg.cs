using System;
using UnityEngine;

public class Egg : GrabObject, ISpoonable, IObjective
{
	public int Type { get; set; }
	public int Player { get; set; }
	public Action<IObjective> OnComplete { get; set; }


	[SerializeField] private GameObject _eggHolder;
	[SerializeField] private GameObject _egg;


	public void OnSpoon()
	{
		Instantiate(_eggHolder, transform.position + Vector3.left, Quaternion.identity, transform.parent);

		GameObject egg = Instantiate(_egg, transform.position + Vector3.right, Quaternion.identity, transform.parent);
		IObjective obj = egg.GetComponent<IObjective>();
		obj.Type = this.Type;
		obj.Player = this.Player;
		obj.OnComplete = this.OnComplete;

		Destroy(gameObject);
	}
}
