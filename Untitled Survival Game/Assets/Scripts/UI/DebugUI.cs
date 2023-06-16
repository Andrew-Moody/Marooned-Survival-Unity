using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugUI : UIPanel
{
	[SerializeField]
	private TextMeshProUGUI _fpsText;

	private float _timesSinceFPS;

	private int _frames;

	void Start()
	{
		
	}


	void Update()
	{
		_timesSinceFPS += Time.deltaTime;
		_frames++;
		if (_timesSinceFPS >= 1f)
		{
			float fps = _frames / _timesSinceFPS;
			_fpsText.text = "FPS: " + fps.ToString("#.##");
			_timesSinceFPS = 0f;
			_frames = 0;
		}
	}


	public void OnStartGame()
	{
		GameManager.Instance.OnDebugStartGamePressed();
	}
}
