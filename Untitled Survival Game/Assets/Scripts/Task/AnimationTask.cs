using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AsyncTasks
{
	public class AnimationTask : AsyncTask
	{
		public event TaskResultHandler AnimEventRecieved;

		private Animator _animator;

		private string _trigger;

		private AnimEventHandler _animEventHandler;


		public AnimationTask(ITaskUser taskOwner, Animator animator, string trigger)
			: base(taskOwner)
		{
			_animator = animator;
			_trigger = trigger;
			_animEventHandler = _animator.gameObject.GetComponent<AnimEventHandler>();
		}
		

		protected override async Task<TaskResultData> PerformTaskAsync(CancellationToken token)
		{
			_taskSource = new TaskCompletionSource<TaskResultData>();

			// Allow the task to be canceled externally
			token.Register(() => _taskSource.TrySetCanceled());


			_animEventHandler.OnAbilityAnimEvent += AnimEventHandler_OnAbilityAnimEvent;

			_animEventHandler.OnAbilityEndAnimEvent += AnimEventHandler_OnAbilityEndAnimEvent;

			_animator.SetTrigger(_trigger);

			return await _taskSource.Task;
		}


		private void AnimEventHandler_OnAbilityAnimEvent()
		{
			AnimEventRecieved?.Invoke(this, null);
		}

		private void AnimEventHandler_OnAbilityEndAnimEvent()
		{
			_taskSource.TrySetResult(null);
		}


		protected override void Cleanup()
		{
			_animEventHandler.OnAbilityAnimEvent -= AnimEventHandler_OnAbilityAnimEvent;

			_animEventHandler.OnAbilityEndAnimEvent -= AnimEventHandler_OnAbilityEndAnimEvent;
		}
	}
}
