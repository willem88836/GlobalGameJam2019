using UnityEngine;

public class Minigame_manager : MonoBehaviour
{
	[SerializeField] Transform _startPoint;
	[SerializeField] HandMover _hand;

	[SerializeField] Minigame_bound[] _bounds;
	[SerializeField] Minigame_finish _finish;

	float _minigameDuration;

    void InitMinigame()
    {
		_hand.transform.position = _startPoint.position;
		_hand.MinigameActive = true;
		_minigameDuration = 15;

		for (int i = 0; i < _bounds.Length; i++)
		{
			_bounds[i].ResetMinigame += ResetMinigame;
		}

		_finish.EndMinigame += EndMinigame;
	}

	void Update()
	{
		CountDown();	
	}

	void CountDown()
	{
		_minigameDuration -= Time.deltaTime;
		if (_minigameDuration <= 0)
			EndMinigame();
	}

	void ResetMinigame()
	{
		_hand.transform.position = _startPoint.position;
		_hand.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	public void EndMinigame()
	{
		_hand.transform.position = _startPoint.position;
		_hand.MinigameActive = false;
		_minigameDuration = 15;

		for (int i = 0; i < _bounds.Length; i++)
		{
			_bounds[i].ResetMinigame += ResetMinigame;
		}

		_finish.EndMinigame += EndMinigame;
	}
}
