using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionPlotter : MonoBehaviour
{
	public string Label;

	[SerializeField]
	private Plottable _target;

	[SerializeField]
	private Mode _mode;

	[SerializeField]
	private float _bufferTime;

	[SerializeField]
	private bool Freeze;


	public AnimationCurve Curve { get; private set; }

	private float _timeSinceBuffer;

	private List<float> _dataPoints = new List<float>();
	private List<float> _timePoints = new List<float>();


	private enum Mode
	{
		Update,
		FixedUpdate,
		LateUpdate,
		External
	}


	private void Start()
	{
		_target = GetComponent<Plottable>();

		if (_target == null)
		{
			Debug.LogError(gameObject + " Has a function plotter but no plottable target");

			enabled = false;
		}
	}


	private void Update()
	{
		if (_mode == Mode.Update)
		{
			Tick(Time.deltaTime);
		}
	}


	private void FixedUpdate()
	{
		if (_mode == Mode.FixedUpdate)
		{
			Tick(Time.fixedDeltaTime);
		}
	}


	private void LateUpdate()
	{
		if (_mode == Mode.LateUpdate)
		{
			Tick(Time.deltaTime);
		}
	}


	public void ExternalTick(float deltaTime)
	{
		if (_mode == Mode.External)
		{
			Tick(deltaTime);
		}
	}


	private void Tick(float deltaTime)
	{
		if (!Freeze)
		{
			_timeSinceBuffer += deltaTime;

			if (_timeSinceBuffer >= _bufferTime)
			{
				PlotBuffer();
				_timeSinceBuffer = 0f;
			}


			_dataPoints.Add(_target.GetValue());
			_timePoints.Add(Time.realtimeSinceStartup);

			
		}
	}


	private void PlotBuffer()
	{
		Curve = new AnimationCurve();

		for (int i = 0; i < _dataPoints.Count; i++)
		{
			Curve.AddKey(_timePoints[i], _dataPoints[i]);
		}

		_dataPoints = new List<float>();
		_timePoints = new List<float>();

		Debug.Log("PlotBuffer");
	}
}
