using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AnimTriggerCue", menuName = "AbilitySystem/AnimationTriggerCue")]
public class AnimationTriggerCue : CueOneShot
{
	[SerializeField] private string _trigger;

	public override void OnExecute(CueEventData data)
	{
		base.OnExecute(data);

		if (data.Target.TryGetComponent(out IAnimCueable animCueable))
		{
			animCueable.SetTrigger(_trigger);
		}
	}
}
