using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AsyncTasks
{
	public class WaitForEventTask : AsyncTask
	{
		public WaitForEventTask(ITaskUser taskOwner) : base(taskOwner) { }

		protected override async Task<TaskResultData> PerformTaskAsync(CancellationToken token)
		{
			_taskSource = new TaskCompletionSource<TaskResultData>();

			// Allow the task to be canceled externally
			token.Register(() => _taskSource.TrySetCanceled());

			// Register a handler for TaskEventRecieved to allow the task user to choose the source of incoming events
			_taskEventRecieved += Task_TaskEventRecieved;

			return await _taskSource.Task;
		}


		protected override void Cleanup()
		{
			_taskEventRecieved -= Task_TaskEventRecieved;
		}


		private void Task_TaskEventRecieved(object sender, TaskEventData data)
		{
			_taskSource.TrySetResult(null);
		}
	}
}
