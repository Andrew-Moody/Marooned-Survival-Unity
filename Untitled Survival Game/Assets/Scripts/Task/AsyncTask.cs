using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AsyncTasks
{
	public class AsyncTask : IAsyncTask
	{
		public event TaskResultHandler TaskCompleted;

		public event TaskResultHandler TaskCanceled;

		protected TaskEventHandler _taskEventRecieved;

		protected TaskCompletionSource<TaskResultData> _taskSource;

		private CancellationTokenSource _cancellationTokenSource;

		public ITaskUser TaskOwner => _taskOwner;
		private ITaskUser _taskOwner;
		

		public AsyncTask(ITaskUser taskOwner) { _taskOwner = taskOwner; }


		public void Start()
		{
			RunTaskAsync();
		}


		public void Stop()
		{
			_cancellationTokenSource?.Cancel();
		}


		// Unsure of the naming convention since this raises an event (well a delegate) but it is intended to be subscribed to external events
		public void HandleTaskEvent(object sender, TaskEventData data)
		{
			_taskEventRecieved?.Invoke(sender, data);
		}


		private async void RunTaskAsync()
		{
			// An attempt at a "Template Method" pattern for async tasks

			_cancellationTokenSource = new CancellationTokenSource();

			TaskResultData result = null;

			try
			{
				result = await PerformTaskAsync(_cancellationTokenSource.Token);
			}
			catch (TaskCanceledException)
			{
				TaskCanceled?.Invoke(this, result);
				return;
			}
			finally
			{
				// Need to cleanup token after task ends
				_cancellationTokenSource.Dispose();
				_cancellationTokenSource = null;

				Cleanup();
			}

			TaskCompleted?.Invoke(this, result);
		}


		// An example override implementation that does not use TaskCompletionSource
		// If you want to just use built in tasks or some other async method
		private async Task<TaskResultData> ExampleTask(CancellationToken token)
		{
			await Task.Delay(10000, token); // perform whatever async task you want

			return null; // or return new TaskResultData() if you want to return empty data instead of null
		}
		

		// Override this to create new tasks
		protected virtual async Task<TaskResultData> PerformTaskAsync(CancellationToken token)
		{
			_taskSource = new TaskCompletionSource<TaskResultData>();

			// Allow the task to be canceled externally
			token.Register(() => _taskSource.TrySetCanceled());

			// Register a handler for TaskEventRecieved to allow the task user to choose the source of incoming events
			_taskEventRecieved += Task_TaskEventRecieved;


			// Could also register to whatever events you want. Maybe an event on the user for example
			// (Changed base class to not hold a reference to the task user but a derived task might have a ref)

			//if (_user is TestTaskUser taskUser)
			//{
			//	taskUser.InputRecieved += (user, data) => taskSource.TrySetResult(null);
			//}

			// Or some other event
			// GamePlay.Instance.TimeChangedEvent += () => taskSource.TrySetResult(null);

			// Note that if registering to an external event, its not easy to unsubscribe lambda functions so either
			// store it in a delegate for later unsubscription or just make a new method


			// It's considered a bad idea to return a task from a syncronous method for reasons explained below
			return await _taskSource.Task;

			// Rather than tie Task completion to the result of a function TCS can be used to manually control completion, cancellation, etc

			// A task can be canceled through TCS i.e. _taskCompletionSource.TrySetCanceled();
			// but it seems to me that is more often used internally by the task rather than for outside cancellation
			// A task can still be canceled by token even if it is controlled by a TCS and that seems to be what is generally exposed
			// In either case cancellation throws an exception that must be handled

			// cancelationToken.Register(() => tcs.TrySetCanceled); Example of tying TCS cancellation to a token

			// Or the task can be canceled by event
			// someObject.OperationFailed += (obj, args) => tcs.TrySetCanceled();

			// Something will have to produce a result to complete the task (could be passed through args or really anything)
			// someObject.OperationSucceeded += (obj, args) =>
			// {
			//    T result = SomeFunction();
			//    tcs.TrySetResult(result);
			// }

			// In general it seems most sources tend to not hold or pass around a reference to the TCS but rather rely on registering callbacks with
			// whatever underlying operation they are interested in

			// Could be wrong but it would seem that the object you register with would need to make sure to invoke the cancel callbacks before being disposed
			// or the TCS could go out of scope while the task continues to run with no way to cancel or complete? (unless cancellation token is used perhaps)

			
			// There seems to be quite a few potential gotchas especially around exceptions
			// For example, the compiler is perfectly happy to return a Task from a syncronous method wich can then be awaited by an async caller
			// (which is what I initially did) the danger is that if the task returned in a syncronous method, exceptions are thrown when called not when awaited
			// which means if the method is called to create the task outside the try block where the task is awaited, the exceptions will not be caught

		}

		private void Task_TaskEventRecieved(object sender, TaskEventData data)
		{
			_taskSource.TrySetResult(null);
		}


		protected virtual void Cleanup()
		{
			// May need to unsubscribe or other cleanup tasks

			_taskEventRecieved -= Task_TaskEventRecieved;
		}
	}
}
