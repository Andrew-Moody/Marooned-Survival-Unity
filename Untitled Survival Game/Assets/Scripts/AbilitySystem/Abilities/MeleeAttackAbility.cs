using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AsyncTasks;
using Actors;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "MeleeAttackAbility", menuName = "AbilitySystem/Ability/MeleeAttackAbility")]
	public class MeleeAttackAbility : Ability
	{
		[SerializeField]
		private Effect _effect;

		[SerializeField]
		private Targeter _targeter;

		[SerializeField]
		private string _animationTrigger;

		[SerializeField]
		private AudioClip _startSound;

		[SerializeField]
		private float _knockbackStrength;


		public override void Activate(AbilityHandle handle)
		{
			Debug.LogError("Ability Activate");

			AudioSource audioSource = handle.Actor.AudioSource;

			audioSource.PlayOneShot(_startSound);

			Animator anim = handle.Actor.Animator;

			AnimationTask task = new AnimationTask(handle, anim, _animationTrigger);

			task.AnimEventRecieved += Task_AnimEventRecieved;

			handle.Task = task;

			handle.Task.TaskCanceled += Task_Canceled;

			handle.Task.TaskCompleted += Task_Completed;

			

			handle.User.TaskEventRecieved += handle.Task.HandleTaskEvent;

			handle.Task.Start();
		}


		public override void Cancel(AbilityHandle handle)
		{
			if (handle.Task != null)
			{
				handle.Task.Stop();
			}
			else
			{
				End(handle);
			}
		}


		protected override void End(AbilityHandle handle)
		{
			if (handle.Task != null)
			{
				handle.User.TaskEventRecieved -= handle.Task.HandleTaskEvent;

				handle.Task = null;
			}

			handle.OnAbilityEnded(null);
		}


		private void Task_Canceled(IAsyncTask task, TaskResultData result)
		{
			if (task.TaskOwner is AbilityHandle handle)
			{
				End(handle);
			}
		}


		private void Task_Completed(IAsyncTask task, TaskResultData result)
		{
			if (task.TaskOwner is AbilityHandle handle)
			{
				End(handle);
			}
		}


		private void Task_AnimEventRecieved(IAsyncTask task, TaskResultData result)
		{
			if (!(task.TaskOwner is AbilityHandle handle))
			{
				Debug.LogError("Task Owner not set");
				return;
			}

			AbilityActor user = handle.User;

			List<TargetResult> targetResults = _targeter.FindTargets(user, new TargetingArgs());

			Debug.LogError($"Found {targetResults.Count} targets");

			foreach (TargetResult targetResult in targetResults)
			{
				if (!(targetResult is ActorTargetResult actorResult))
				{
					continue;
				}

				Actor actor = actorResult.Actor;

				AbilityActor target = actor.AbilityActor;

				Agent agent = actor.Components.Agent;

				if (agent != null && user.IsServer)
				{
					Vector3 direction = (target.transform.position - user.transform.position).normalized;

					direction.y += Mathf.Atan(Mathf.Deg2Rad * 30f); // add an upward component

					// Will want to calculate from target and user stats eventually
					float strength = _knockbackStrength;

					agent.KnockBack(direction, strength);

					target.Actor.Animator.SetTrigger("HIT");
				}

				ApplyEffect(handle, _effect, target);
			}
		}


		private void ApplyEffect(AbilityHandle handle, Effect effect, AbilityActor target)
		{
			EffectEventData effectData = new EffectEventData()
			{
				Source = handle.User,
				Target = target
			};

			EffectHandle effectHandle = new EffectHandle(effect, effectData);

			target.ApplyEffect(effectHandle);
		}
	}
}

