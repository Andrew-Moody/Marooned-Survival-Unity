using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Actors;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "TakeDamageCue", menuName = "AbilitySystem/Cue/TakeDamageCue")]
	public class TakeDamageCue : CueOneShot
	{
		[SerializeField] private GameObject _particlePF;

		[SerializeField] private AudioClip _audioClip;

		[SerializeField] private bool _attachToOwner;

		[SerializeField] private string _attachPoint;

		[SerializeField] private string _transformAnimation;


		public override void OnExecute(CueEventData data)
		{
			base.OnExecute(data);

			Actor target = data.Target.Actor;

			if (target == null)
			{
				return;
			}

			PlayParticleEffect(target, data);

			PlaySoundEffect(target, data);

			PlayTransformAnimation(target, data);
		}


		private void PlayParticleEffect(Actor target, CueEventData data)
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
				if (target.AttachPoints != null)
				{
					parent = target.AttachPoints.FindAttachPoint(_attachPoint);
				}

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
				main.stopAction = ParticleSystemStopAction.Destroy;
			}
		}


		private void PlaySoundEffect(Actor target, CueEventData data)
		{
			AudioSource audioSource = target.AudioSource;

			if (audioSource != null)
			{
				audioSource.PlayOneShot(_audioClip);
			}
		}


		private void PlayTransformAnimation(Actor target, CueEventData data)
		{
			if (_transformAnimation == null || _transformAnimation == "")
			{
				return;
			}

			TransformAnimator transformAnimator = target.TransformAnimator;

			if (transformAnimator != null)
			{
				transformAnimator.SetTrigger(_transformAnimation);
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
}

