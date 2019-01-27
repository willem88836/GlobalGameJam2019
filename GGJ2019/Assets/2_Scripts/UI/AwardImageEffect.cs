using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwardImageEffect : MonoBehaviour
{
	[SerializeField] Image _image;
	[Space]
	[SerializeField] Sprite _ateTheMostSprite;
	[SerializeField] Sprite _cleanestTableSprite;
	[SerializeField] Sprite _spilledTheMostSprite;
	[SerializeField] Sprite _youWonNothingSprite;
	[Space]
	[SerializeField] AnimationCurve _animationCurve;
	[SerializeField] float _animationSpeed;

	bool _isPlaying = false;

	float _time = 0;

	float _screenWidth;

	void Start()
	{
		_screenWidth = Screen.width;
	}

	private void Update()
	{
		MoveUI();

		if (Input.GetKeyDown(KeyCode.Space))
			PlayAteTheMost();
	}

	public void PlayAteTheMost()
	{
		_image.sprite = _ateTheMostSprite;
		_isPlaying = true;
	}

	public void PlayCleanestTable()
	{
		_image.sprite = _cleanestTableSprite;
		_isPlaying = true;
	}

	public void SpilledTheMost()
	{
		_image.sprite = _spilledTheMostSprite;
		_isPlaying = true;
	}

	public void PlayYouWonNothing()
	{
		_image.sprite = _ateTheMostSprite;
		_isPlaying = true;
	}

	private void MoveUI()
	{
		if (_isPlaying)
		{
			_time += Time.deltaTime * _animationSpeed;

			_image.transform.position = new Vector3(
				_animationCurve.Evaluate(_time) * _screenWidth * 2 - (_screenWidth / 2),
				_image.transform.position.y,
				_image.transform.position.z);

			if (_time >= 1)
			{
				_time = 0;
				_isPlaying = false;
			}
		}
	}
}
