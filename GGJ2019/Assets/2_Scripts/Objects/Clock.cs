using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
	[SerializeField] Text _timerText;

	float _timeLeft = 0;

	bool _isCountingDown = false;

	void Start()
	{
		_timerText.text = Mathf.Ceil(_timeLeft).ToString();
	}

	void Update()
    {
		if (_isCountingDown)
			CountDown();
    }

	public void SetTimer(float time, bool startCountingDown)
	{
		_timeLeft = time;

		if (startCountingDown)
			ToggleCountdown(true);
	}

	public void ToggleCountdown(bool isCountingDown)
	{
		_isCountingDown = isCountingDown;
	}

	void CountDown()
	{
		_timeLeft -= Time.deltaTime;
		_timerText.text = Mathf.Ceil(_timeLeft).ToString();

		if (_timeLeft <= 0)
		{
			_timeLeft = 0;
			_isCountingDown = false;
			FinishedCountdown();
		}	
	}

	void FinishedCountdown()
	{
		// does nothing atm, but might be functional
	}
}
