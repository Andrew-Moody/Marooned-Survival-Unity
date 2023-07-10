using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCueable : MonoBehaviour, IAnimCueable
{
	[SerializeField]
	private Animator _animator;

	public void SetTrigger(string trigger)
	{
		if (_animator != null)
		{
			_animator.SetTrigger(trigger);
		}
	}
}
