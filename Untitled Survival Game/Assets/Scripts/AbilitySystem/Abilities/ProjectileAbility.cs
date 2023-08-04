using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor = Actors.Actor;
using AsyncTasks;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "ProjectileAbility", menuName = "AbilitySystem/Ability/ProjectileAbility")]
	public class ProjectileAbility : Ability
	{
		[SerializeField]
		private GameObject _projectilePrefab;

		[SerializeField]
		private float _speed;

		[SerializeField]
		private AudioClip _launchSound;

		[SerializeField]
		private string _animationTrigger;

		
		public override void Activate(AbilityHandle handle)
		{
			handle.Actor.AudioSource.PlayOneShot(_launchSound);

			if (handle.User.IsServer)
			{
				SpawnProjectile(handle);
			}

			Animator anim = handle.Actor.Animator;

			AnimationTask task = new AnimationTask(handle, anim, _animationTrigger);

			handle.Task = task;

			task.AnimEventRecieved += Task_AnimEventRecieved;

			task.TaskCanceled += Task_Canceled;

			task.TaskCompleted += Task_Completed;

			handle.User.TaskEventRecieved += task.HandleTaskEvent;

			task.Start();


			End(handle);
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
			if (task.TaskOwner is AbilityHandle handle)
			{
				if (handle.User.IsServer)
				{
					LaunchProjectile(handle);
				}
			}
			else
			{
				Debug.LogError("Task Owner not set");
			}
		}


		private void SpawnProjectile(AbilityHandle handle)
		{
			Actor actor = handle.Actor;

			ProjectileBase prefab = _projectilePrefab.GetComponent<ProjectileBase>();

			Transform attachPoint = actor.AttachPoints.FindAttachPoint("Projectile_Attach");

			if (attachPoint == null)
			{
				attachPoint = actor.transform;
			}

			Vector3 position = attachPoint.position;
			Quaternion rotation = attachPoint.rotation;

			ProjectileBase projectile = ProjectileManager.SpawnProjectile(prefab, position, rotation, actor);

			projectile.SetFollowTarget(attachPoint);

			if (handle.AbilityData is ProjectileAbilityData data)
			{
				data.Projectile = projectile;
			}
		}


		private void LaunchProjectile(AbilityHandle handle)
		{
			if (handle.AbilityData is ProjectileAbilityData data)
			{
				ProjectileBase projectile = data.Projectile;

				Vector3 velocity = _speed * handle.Actor.ViewTransform.transform.forward;
				projectile.Launch(velocity);
			}
			else
			{
				Debug.LogError("Failed to launch projectile: AbilityData was not of type ProjectileAbilityData");
			}
		}


		private void OnValidate()
		{
			if (_projectilePrefab != null && _projectilePrefab.GetComponent<ProjectileBase>() == null)
			{
				_projectilePrefab = null;
				Debug.LogWarning("Prefab must have a Projectile component");
			}
		}


		public override AbilityInstanceData CreateInstanceData(AbilityActor user)
		{
			return new ProjectileAbilityData() { };
		}
	}
}
