using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DestroyOnEat : NetworkBehaviour, IEdible
{
	[Server]
    public void OnEat()
	{
		//Destroy(gameObject);
		NetworkServer.Destroy(gameObject);
	}
}
