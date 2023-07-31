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
			handle.AbilityData.User.Actor.AudioSource.PlayOneShot(_launchSound);

			if (handle.AbilityData.User.IsServer)
			{
				SpawnProjectile(handle);
			}

			Animator anim = handle.AbilityData.User.Actor.Animator;

			AnimationTask task = new AnimationTask(handle, anim, _animationTrigger);

			handle.AbilityData.Task = task;

			task.AnimEventRecieved += Task_AnimEventRecieved;

			task.TaskCanceled += Task_Canceled;

			task.TaskCompleted += Task_Completed;

			handle.AbilityData.User.TaskEventRecieved += task.HandleTaskEvent;

			task.Start();


			End(handle);
		}


		public override void Cancel(AbilityHandle handle)
		{
			if (handle.AbilityData.Task != null)
			{
				handle.AbilityData.Task.Stop();
			}
			else
			{
				End(handle);
			}
		}


		protected override void End(AbilityHandle handle)
		{
			if (handle.AbilityData.Task != null)
			{
				handle.AbilityData.User.TaskEventRecieved -= handle.AbilityData.Task.HandleTaskEvent;

				handle.AbilityData.Task = null;
			}

			handle.AbilityData.User.HandleAbilityEnd();
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
				if (handle.AbilityData.User.IsServer)
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
			Actor actor = handle.AbilityData.User.Actor;

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

			handle.AbilityData.Projectile = projectile;
		}


		private void LaunchProjectile(AbilityHandle handle)
		{
			ProjectileBase projectile = handle.AbilityData.Projectile;

			Vector3 velocity = _speed * handle.AbilityData.User.Actor.ViewTransform.transform.forward;
			projectile.Launch(velocity);
		}


		private void OnValidate()
		{
			if (_projectilePrefab != null && _projectilePrefab.GetComponent<ProjectileBase>() == null)
			{
				_projectilePrefab = null;
				Debug.LogWarning("Prefab must have a Projectile component");
			}
		}
	}
}
