using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cue : ScriptableObject
{
	public AbilityTag Tag => _tag;
	[SerializeField] protected AbilityTag _tag;


	public virtual CueNotifyType GetNotifyType() => CueNotifyType.None;


	public virtual void HandleCue(CueEventType eventType, CueEventData data)
	{
		switch (eventType)
		{
			case CueEventType.OnExecute:
				OnExecute(data);
				break;
			case CueEventType.OnActivate:
				OnActivate(data);
				break;
			case CueEventType.OnRemove:
				OnRemove(data);
				break;
			case CueEventType.OnVisible:
				OnVisible(data);
				break;
		}
	}

	/// <summary>
	/// Called when the Cue is triggered instantly
	/// </summary>
	public virtual void OnExecute(CueEventData data)
	{

	}


	/// <summary>
	/// Called when the Cue is first triggered
	/// </summary>
	public virtual void OnActivate(CueEventData data)
	{

	}


	/// <summary>
	/// Called when the Cue is removed or when a client is no longer an observer
	/// </summary>
	public virtual void OnRemove(CueEventData data)
	{

	}


	/// <summary>
	/// Called when a client becomes an observer
	/// Pontentially will implement with an RPC with bufferlast = true
	/// </summary>
	public virtual void OnVisible(CueEventData data)
	{

	}


	public enum CueNotifyType
	{
		None,
		NonInstantiated,
		Instantiated,
	}
}


public enum CueEventType
{
	OnExecute,
	OnActivate,
	OnRemove,
	OnVisible
}


public class CueEventData
{
	public AbilityActor Target;

	public Vector3 Position;

	public Quaternion Rotation;
}