using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actors
{
	public class Actor : NetworkBehaviour, IActor
	{
		public event ActorEventHandler DeathStarted;

		public event ActorEventHandler DeathFinished;


		public Stats Stats => _stats;
		[SerializeField] private Stats _stats;

		public Animator Animator { get => _animator; set => _animator = value; } // Currently sill used by mobs during setup
		[SerializeField] private Animator _animator;

		public AudioSource AudioSource => _audioSource;
		[SerializeField] private AudioSource _audioSource;

		public AttachPoints AttachPoints => _attachPoints;
		[SerializeField] private AttachPoints _attachPoints;

		public TransformAnimator TransformAnimator => _transformAnimator;
		[SerializeField] private TransformAnimator _transformAnimator;

		public Transform ViewTransform => _viewTransform;
		[SerializeField] private Transform _viewTransform;
		

		//private ActorState _actorState;

		public virtual void SetAnimTrigger(string name)
		{
			if (_animator != null)
			{
				_animator.SetTrigger(name);
			}
			else if (_transformAnimator != null)
			{
				_transformAnimator.PlayAnimation(name);
			}
		}


		public virtual void SetAnimFloat(string name, float value)
		{
			if (_animator != null)
			{
				_animator.SetFloat(name, value);
			}
		}


		protected virtual void StartDeath()
		{
			//_actorState = ActorState.Dying;

			OnDeathStarted();
		}


		protected virtual void FinishDeath()
		{
			//_actorState = ActorState.Dead;

			OnDeathFinished();
		}


		protected void OnDeathStarted()
		{
			DeathStarted?.Invoke(this, new ActorEventData());
		}


		protected void OnDeathFinished()
		{
			DeathFinished?.Invoke(this, new ActorEventData());
		}


		private enum ActorState
		{
			Alive,
			Dying,
			Dead
		}
	}
}
