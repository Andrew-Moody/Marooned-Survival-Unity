using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsyncTasks
{
	// I dont want to put limits on what can send events as only the task implementations know what they are interested in
	public delegate void TaskEventHandler(object sender, TaskEventData data);

	public delegate void TaskResultHandler(IAsyncTask task, TaskResultData data);

	public class TaskEventData
	{

	}


	public class TaskResultData
	{

	}


	public interface IAsyncTask
	{
		public event TaskResultHandler TaskCompleted;

		public event TaskResultHandler TaskCanceled;

		public ITaskUser TaskOwner { get; }

		public void Start();

		public void Stop();

		public void HandleTaskEvent(object sender, TaskEventData data);
	}
}

