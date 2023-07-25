using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AsyncTasks;

public class TestTaskUser : MonoBehaviour, ITaskUser
{
	public event TaskEventHandler InputRecieved;

	private IAsyncTask _task;


	void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			if (_task != null)
			{
				_task.Stop();

				InputRecieved -= _task.HandleTaskEvent;
			}

			_task = new WaitForEventTask(this);

			_task.TaskCompleted += (task, result) => Debug.Log("Task Completed");

			_task.TaskCanceled += (task, result) => Debug.Log("Task Canceled");

			InputRecieved += _task.HandleTaskEvent;

			_task.Start();

			Debug.Log("Starting Task");
		}
		else if (Input.GetKeyDown(KeyCode.C))
		{
			if (_task != null)
			{
				_task.Stop();
			}
		}
		else if (Input.GetKeyDown(KeyCode.F))
		{
			InputRecieved?.Invoke(this, null);
		}
	}
}
