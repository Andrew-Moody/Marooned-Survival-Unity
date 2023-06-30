using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCueable : MonoBehaviour, ICueable
{
	[SerializeField]
	protected AbilityTag _tag;


	public virtual void OnCue(AbilityEvent evt) { }
}


// My god, I've implemented RAII in C#
// this is really just an experiment to see if this is a reasonable way to manage event subscription from a base monobehaviour
// without the risk that derived behaviours will overwrite Awake or Destroy without calling base versions
public class CueableHandle
{
	private AbilityTag _tag;
	public AbilityTag Tag => _tag;

	private ICueable _target;

	private AbilityActor _actor;

	public CueableHandle(ICueable target, AbilityActor actor)
	{
		_target = target;

		_actor = actor;

		if (_target == null || _actor == null)
		{
			Debug.LogError("Failed to create CueableWrapper");
			return;
		}

		// Subscribe
	}


	~CueableHandle()
	{
		// Unsubscribe
	}


	public void OnCue(AbilityEvent evt)
	{
		// if a monobehaviour is destroyed there are cases where the reference held through an interface will not be null
		// need to do more testing on this

		// Basically I dont want OnCue to be called on a destroyed monobehavior but if OnDestroy doesn't unsubscribe
		// from the actor events there is a risk that will happen

		if (_target != null && !_target.Equals(null))
		{
			_target.OnCue(evt);
		}
	}
}
