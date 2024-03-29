using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformAnimator : MonoBehaviour
{
	public event System.Action AnimationEnded;

	[SerializeField]
	private List<TransformAnimation> _animations;

	private Dictionary<string, TransformAnimation> _animDict;

	//private float _time = 0f;

	void Awake()
	{
		_animDict = new Dictionary<string, TransformAnimation>();

		foreach (TransformAnimation animation in _animations)
		{
			animation.SetTarget(transform);

			_animDict.Add(animation.Name, animation);

			animation.AnimationEnded += Animation_AnimationEnded;
		}
	}


	public void SetTrigger(string name)
	{
		if (_animDict.TryGetValue(name, out TransformAnimation animation))
		{
			if (!animation.IsPlaying)
			{
				StartCoroutine(animation.Animate());
			}
		}
		else
		{
			Debug.LogWarning("Failed to play transform animation: " + name);
		}
	}


	private void Animation_AnimationEnded()
	{
		AnimationEnded?.Invoke();
	}
}
