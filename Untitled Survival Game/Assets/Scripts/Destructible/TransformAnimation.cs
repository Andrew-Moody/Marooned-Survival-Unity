using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformAnimation
{
	public event System.Action AnimationEnded;


	[SerializeField]
	private string _name;
	public string Name { get { return _name; } }


	[SerializeField]
	private AnimationCurve _animationCurve;

	[SerializeField]
	private float _duration;


	private Transform _target;

	private float _time;

	private bool _playing;

	public bool IsPlaying { get { return _playing; } }

	
	public void SetTarget(Transform target)
	{
		_target = target;
	}


	public IEnumerator Animate()
	{
		_playing = true;

		_time = 0f;

		while (_time < _duration)
		{
			_time += Time.deltaTime;

			_target.localScale = Vector3.one * _animationCurve.Evaluate(_time / _duration);

			yield return null;
		}

		_playing = false;

		AnimationEnded?.Invoke();
	}
}