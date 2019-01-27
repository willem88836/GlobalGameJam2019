using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
	AudioSource _audioSource;

	[SerializeField] AudioClip[] _throwFoodFiles;
	[SerializeField] AudioClip[] _foodInMouthFiles;
	[SerializeField] AudioClip[] _foodLandsOnTableFiles;
	[SerializeField] AudioClip[] _foodHitsCharacterFiles;
	[SerializeField] AudioClip[] _OohFiles;
	[SerializeField] AudioClip[] _EnemyFoodFiles;
	[SerializeField] AudioClip[] _YeahFiles;

	private void Start()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	public void PlayThrowFood()
	{
		int randomClip = Random.Range(0, _throwFoodFiles.Length);
		_audioSource.PlayOneShot(_throwFoodFiles[randomClip]);
	}

	public void PlayFoodInMouth()
	{
		int randomClip = Random.Range(0, _foodInMouthFiles.Length);
		_audioSource.PlayOneShot(_foodInMouthFiles[randomClip]);
	}

	public void PlayFoodLandsOnTable()
	{
		int randomClip = Random.Range(0, _foodLandsOnTableFiles.Length);
		_audioSource.PlayOneShot(_foodLandsOnTableFiles[randomClip]);
	}

	public void PlayFoodHitsCharacter()
	{
		int randomClip = Random.Range(0, _foodHitsCharacterFiles.Length);
		_audioSource.PlayOneShot(_foodHitsCharacterFiles[randomClip]);
	}

	public void PlayOoh()
	{
		int randomClip = Random.Range(0, _OohFiles.Length);
		_audioSource.PlayOneShot(_OohFiles[randomClip]);
	}

	public void PlayEnemyFood()
	{
		int randomClip = Random.Range(0, _EnemyFoodFiles.Length);
		_audioSource.PlayOneShot(_EnemyFoodFiles[randomClip]);
	}

	public void PlayYeah()
	{
		int randomClip = Random.Range(0, _YeahFiles.Length);
		_audioSource.PlayOneShot(_YeahFiles[randomClip]);
	}
}
