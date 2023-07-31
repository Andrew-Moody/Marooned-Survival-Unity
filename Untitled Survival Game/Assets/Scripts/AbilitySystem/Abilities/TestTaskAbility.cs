using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AsyncTasks;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "NewTestTaskAbility", menuName = "AbilitySystem/Ability/TestTaskAbility")]
	public class TestTaskAbility : Ability
	{
		[SerializeField]
		private Effect _effect;


		public override void Activate(AbilityHandle handle)
		{
			if (handle.AbilityData.User.IsServer)
			{
				Debug.Log("TestAbility Activated AsServer");
			}
			else if (handle.AbilityData.User.IsOwner)
			{
				Debug.Log("TestAbility Activated AsOwner");
			}

			handle.AbilityData.Task = new WaitForEventTask(handle);

			// Using lambdas to capture the AbilityDataInstance works for now until I decide on
			// the best way to get the instance from the task
			// handle.AbilityData.Task.TaskCanceled += (task, result) => Task_Canceled(task, result, handle);
			// handle.AbilityData.Task.TaskCompleted += (task, result) => Task_Completed(task, result, handle);
			// Actually it doesn't as then there is no way to unsubscribe

			handle.AbilityData.Task.TaskCanceled += Task_Canceled;

			handle.AbilityData.Task.TaskCompleted += Task_Completed;

			handle.AbilityData.User.TaskEventRecieved += handle.AbilityData.Task.HandleTaskEvent;

			handle.AbilityData.Task.Start();
		}


		private void Task_Canceled(IAsyncTask task, TaskResultData result)
		{
			if (task.TaskOwner is AbilityHandle handle)
			{
				Debug.Log("Task Canceled");

				End(handle);
			}

		}


		private void Task_Completed(IAsyncTask task, TaskResultData result)
		{
			if (task.TaskOwner is AbilityHandle handle)
			{
				ApplyEffect(handle, handle.AbilityData.User);

				End(handle);
			}

		}


		public override void Cancel(AbilityHandle handle)
		{
			if (handle.AbilityData.User.IsServer)
			{
				Debug.Log("TestAbility Canceled AsServer");
			}
			else if (handle.AbilityData.User.IsOwner)
			{
				Debug.Log("TestAbility Canceled AsOwner");
			}

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
			if (handle.AbilityData.User.IsServer)
			{
				Debug.Log("TestAbility End AsServer");
			}
			else if (handle.AbilityData.User.IsOwner)
			{
				Debug.Log("TestAbility End AsOwner");
			}

			if (handle.AbilityData.Task != null)
			{
				handle.AbilityData.User.TaskEventRecieved -= handle.AbilityData.Task.HandleTaskEvent;

				handle.AbilityData.Task = null;
			}

			handle.AbilityData.User.HandleAbilityEnd();
		}


		private void ApplyEffect(AbilityHandle handle, AbilityActor target)
		{
			EffectEventData effectData = new EffectEventData()
			{
				Source = handle.AbilityData.User,
				Target = target
			};

			EffectHandle effectHandle = new EffectHandle(_effect, effectData);

			target.ApplyEffect(effectHandle);
		}
	}
}
