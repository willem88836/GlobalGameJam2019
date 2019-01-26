using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] Button _serverButton;

	[SerializeField] Button _joinButton;

	[SerializeField] InputField _ipInput;

	private void Awake()
	{
		_serverButton.onClick.AddListener(delegate { ServerPressed(); });
		_joinButton.onClick.AddListener(delegate { JoinPressed(); });
		_ipInput.onValueChanged.AddListener(delegate { IpChanged(_ipInput.text); });

		IpChanged(_ipInput.text);
	}

	void ServerPressed()
	{
		StartServer();
	}

	void JoinPressed()
	{
		StartClient();
	}

	void IpChanged(string text)
	{
		networkAddress = text;
	}
}
