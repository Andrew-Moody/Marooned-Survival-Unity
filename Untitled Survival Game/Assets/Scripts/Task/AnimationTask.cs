using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Actors;

namespace AsyncTasks
{
	public class AnimationTask : AsyncTask
	{
		public event TaskResultHandler AnimEventRecieved;

		private Actor _actor;

		private string _trigger;

		private AnimEventHandler _animEventHandler;


		public AnimationTask(ITaskUser taskOwner, Actor actor, string trigger)
			: base(taskOwner)
		{
			_actor = actor;
			_trigger = trigger;
		}


		protected override async Task<TaskResultData> PerformTaskAsync(CancellationToken token)
		{
			_taskSource = new TaskCompletionSource<TaskResultData>();

			// Allow the task to be canceled externally
			token.Register(() => _taskSource.TrySetCanceled());

			_actor.AnimEventForwarder.AnimEventRecieved += AnimEventForwarder_AnimEventRecieved;

			_actor.Animator.SetTrigger(_trigger);

			return await _taskSource.Task;
		}


		private void AnimEventForwarder_AnimEventRecieved(AnimEventData data)
		{
			Debug.LogWarning($"{_actor.name} AnimEventKind: {data.EventKind}, Param: {data.Param}, {GetActiveClips()}");

			switch (data.EventKind)
			{
				case AnimEventKind.AbilityCue:
				{
					// Foward the animation event to the task user
					AnimEventRecieved?.Invoke(this, null);
					break;
				}
				case AnimEventKind.AbilityEnd:
				{
					// Animation task completed successfully
					_taskSource.TrySetResult(null);
					break;
				}
				default:
				{
					Debug.LogError("Animation Task Error: anim event recieved with unknown AnimEventKind");
					_taskSource.TrySetResult(null);
					break;
				}
			}
		}


		protected override void Cleanup()
		{
			Debug.LogWarning($"Animation Task Cleanup:");

			if (_actor != null)
			{
				Debug.LogWarning(GetActiveClips());

				_actor.AnimEventForwarder.AnimEventRecieved -= AnimEventForwarder_AnimEventRecieved;

				AnimatorStateInfo info = _actor.Animator.GetCurrentAnimatorStateInfo(1);

				// Transition out of USE_ABILITY state if it hasn't finished
				if (info.IsName("USE_ABILITY"))
				{
					Debug.LogWarning($"Normalized time: {info.normalizedTime}");
					Debug.LogWarning($"Setting Trigger: CANCEL_ABILITY");
					_actor.Animator.SetTrigger("CANCEL_ABILITY");
				}
			}
			else
			{
				Debug.Log("Actor was null while cleaning up animation task");

				if (TaskOwner is AbilitySystem.AbilityHandle handle)
				{
					if (handle.Actor)
					{
						Debug.Log($"Found actor through taskowner: {handle.Actor.name}");
					}
					else if (handle.User)
					{
						Debug.Log($"Found user through taskowner: {handle.User.name}");
					}
				}
			}
			
			
		}


		private string GetActiveClips()
		{
			string active = "Active clips: ";

			if (_actor != null)
			{
				var clipInfo = _actor.Animator.GetCurrentAnimatorClipInfo(1);

				for (int i = 0; i < clipInfo.Length; i++)
				{
					if (i != 0)
					{
						active += ", ";
					}

					active += clipInfo[i].clip.name;
				}
			}
			else
			{
				active += "null actor";
			}

			return active;
		}
	}
}
