using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerOnly : NetworkBehaviour
{
    void Start()
    {
		if (!isServer)
			gameObject.SetActive(false);
    }
}
