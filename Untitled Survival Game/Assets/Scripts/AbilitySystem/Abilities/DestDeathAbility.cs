using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AsyncTasks;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "DestDeathAbility", menuName = "AbilitySystem/Ability/DestDeathAbility")]
	public class DestDeathAbility : Ability
	{
		[SerializeField] private AbilityTrait _cueOnStart;

		[SerializeField] private string _animTrigger;

		public override bool CanActivate(AbilityHandle handle)
		{
			return true;
		}


		public override void Activate(AbilityHandle handle)
		{
			CueEventData data = new CueEventData()
			{
				Target = handle.AbilityData.User,
			};

			CueManager.HandleCue(_cueOnStart, CueEventType.OnExecute, data);

			TransformAnimator anim = handle.AbilityData.User.Actor.TransformAnimator;

			TransformAnimTask task = new TransformAnimTask(handle, anim, _animTrigger);

			handle.AbilityData.Task = task;

			handle.AbilityData.Task.TaskCanceled += Task_Canceled;

			handle.AbilityData.Task.TaskCompleted += Task_Completed;

			handle.AbilityData.User.TaskEventRecieved += handle.AbilityData.Task.HandleTaskEvent;

			handle.AbilityData.Task.Start();
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

			if (handle.AbilityData.User.IsServer)
			{
				handle.AbilityData.User.Actor.NotifyDeathAbilityEnd();
			}
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
	}
}

