using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCueable : BaseCueable
{
	private Animator _animator;

	private void Awake()
	{
		_animator = GetComponent<Animator>();

		if (_animator == null)
		{
			Debug.LogError($"AnimationCueable {gameObject.name} was unable to find an Animator");
		}
	}
	public override void OnCue(AbilityEvent evt)
	{
		
	}
}
