using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractEffect : MonoBehaviour
{

	[SerializeField]
	private AudioClip _clip;

	[SerializeField]
	private AudioSource _source;

	[SerializeField]
	private ParticleSystem _particle;

	private void OnEnable()
	{
		Debug.LogError("Playing effect");
		_source.PlayOneShot(_clip);
		_particle.Play();
	}
}
