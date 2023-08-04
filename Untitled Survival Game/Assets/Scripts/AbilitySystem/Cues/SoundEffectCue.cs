using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Actors;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "SoundEffectCue", menuName = "AbilitySystem/Cue/SoundEffectCue")]
	public class SoundEffectCue : CueOneShot
	{
		[SerializeField] private AudioClip _audioClip;


		public override void OnExecute(CueEventData data)
		{
			base.OnExecute(data);

			Actor target = data.Target.Actor;

			if (target == null)
			{
				return;
			}

			PlaySoundEffect(target, data);
		}


		private void PlaySoundEffect(Actor target, CueEventData data)
		{
			AudioSource audioSource = target.AudioSource;

			if (audioSource != null)
			{
				audioSource.PlayOneShot(_audioClip);
			}
		}
	}
}

