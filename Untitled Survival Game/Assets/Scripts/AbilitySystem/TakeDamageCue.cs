using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TakeDamageCue", menuName = "AbilitySystem/TakeDamageCue")]
public class TakeDamageCue : CueOneShot
{
	[SerializeField] private GameObject _particlePF;

	[SerializeField] private AudioClip _audioClip;

	[SerializeField] private bool _attachToOwner;

	[SerializeField] private string _attachPoint;

	public override void OnExecute(CueEventData data)
	{
		base.OnExecute(data);

		PlayParticleEffect(data);

		PlaySoundEffect(data);

		PlayTransformAnimation(data);
	}


	private void PlayParticleEffect(CueEventData data)
	{
		if (_particlePF == null)
		{
			Debug.LogError("ParticleOneShotCue has no particle effect specified");
			return;
		}

		Vector3 position = data.Position;

		Quaternion rotation = data.Rotation;

		Transform parent = null;

		if (_attachToOwner)
		{
			parent = data.Target.FindAttachPoint(_attachPoint);

			if (parent == null)
			{
				parent = data.Target.transform;
			}

			if (parent != null)
			{
				position += parent.position;

				rotation = rotation * parent.rotation;
			}
		}


		GameObject particleObj = Instantiate(_particlePF, position, rotation, parent);

		if (particleObj.TryGetComponent(out ParticleSystem particle))
		{
			particle.Play();

			// For particle effects to be reused in different contexts, settings like what to do when finished need to be set in code

			ParticleSystem.MainModule main = particle.main;
			//main.stopAction = ParticleSystemStopAction.Destroy;
		}
	}


	private void PlaySoundEffect(CueEventData data)
	{
		AudioSource audioSource = data.Target.AudioSource;

		if (audioSource != null)
		{
			audioSource.PlayOneShot(_audioClip);
		}
	}


	private void PlayTransformAnimation(CueEventData data)
	{
		TransformAnimator transformAnimator = data.Target.TransformAnimator;

		if (transformAnimator != null)
		{
			transformAnimator.PlayAnimation(_tag.ToString());
		}
	}


	private void OnValidate()
	{
		if (_particlePF != null && !_particlePF.GetComponent<ParticleSystem>())
		{
			_particlePF = null;
			Debug.LogError("ParticlePF must be a prefab that has a ParticleSystem component");
		}
	}
}
