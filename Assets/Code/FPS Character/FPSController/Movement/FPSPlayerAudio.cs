using System.Collections;
using System.Collections.Generic;
using EventManagement;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FPSPlayerAudio : MonoBehaviour
{
	[SerializeField] private List<AudioClip> _jumpAudioClips;
	[SerializeField] private List<AudioClip> _landAudioClips;

	private AudioSource _audioSource;

	private FPSPlayer  _player;
	
	private void Awake()
	{
		_player = GetComponentInParent<FPSPlayer>();
		
		if (_player == null) Debug.LogError("FPSPlayerAudio requires an FPS Player be a component in a parent game object");
		
		_audioSource = GetComponent<AudioSource>();
	}

	// Use this for initialization
	private void OnEnable()
	{
		_player.Events.OnJump += PlayerJumpEvent;
		_player.Events.OnLand += PlayerLandEvent;
	}

	private void OnDisable()
	{
		_player.Events.OnJump -= PlayerJumpEvent;
		_player.Events.OnLand -= PlayerLandEvent;
	}

	// Update is called once per frame
	void PlayerJumpEvent ()
	{
		AudioClip clip = _jumpAudioClips[Random.Range(0, _jumpAudioClips.Count - 1)];
		_audioSource.PlayOneShot(clip, 0.2f);
	}
	
	// Update is called once per frame
	void PlayerLandEvent (float distance)
	{
		float scale = distance / 5;
		float volume = Mathf.Clamp(scale, 0.1f, 1.2f);
		
		AudioClip clip = _landAudioClips[Random.Range(0, _landAudioClips.Count - 1)];
		_audioSource.PlayOneShot(clip, volume);
	}
}
