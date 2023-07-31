using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AsyncTasks
{
	public class TransformAnimTask : AsyncTask
	{
		public event TaskResultHandler AnimEventRecieved;

		private TransformAnimator _animator;

		private string _trigger;

		private AnimEventHandler _animEventHandler;


		public TransformAnimTask(ITaskUser taskOwner, TransformAnimator animator, string trigger)
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

			_animator.AnimationEnded += Animator_AnimationEnded;

			_animator.SetTrigger(_trigger);

			return await _taskSource.Task;
		}


		private void Animator_AnimationEnded()
		{
			_taskSource.TrySetResult(null);
		}


		protected override void Cleanup()
		{
			_animator.AnimationEnded -= Animator_AnimationEnded;
		}
	}
}
