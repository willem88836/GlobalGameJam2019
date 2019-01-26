using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPrinter : MonoBehaviour {

	[SerializeField] GameObject _errorPrefab;
	[SerializeField] Transform _errorParent;

	List<GameObject> _messages = new List<GameObject>();

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
			Debug.LogError("Test_Error");

		if (Input.GetKeyDown(KeyCode.C))
			ClearLogs();
	}

	void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}

	void HandleLog(string logString, string stackTrace, LogType type)
	{
		Color color = Color.blue;
		switch (type)
		{
			case LogType.Log:
				color = Color.white;
				break;
			case LogType.Warning:
				color = Color.yellow;
				break;
			case LogType.Error:
				color = Color.red;
				break;
		}

		GameObject message = Instantiate(_errorPrefab, _errorParent);
		Text messageText = message.GetComponent<Text>();
		messageText.text = type.ToString() + ": " + logString + " -> " + stackTrace;
		messageText.color = color;

		_messages.Add(message);
	}

	void ClearLogs()
	{
		for (int i = 0; i < _messages.Count; i++)
		{
			GameObject current = _messages[i];
			
			if (current != null)
				Destroy(current);
		}

		_messages.Clear();
	}
}
