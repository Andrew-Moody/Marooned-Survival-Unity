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
				Target = handle.User,
			};

			CueManager.HandleCue(_cueOnStart, CueEventType.OnExecute, data);

			TransformAnimator anim = handle.Actor.TransformAnimator;

			TransformAnimTask task = new TransformAnimTask(handle, anim, _animTrigger);

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
	}
}

