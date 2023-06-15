using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeUI : UIPanel
{

	[SerializeField]
	private TextMeshProUGUI _dayTMP;

	[SerializeField]
	private TextMeshProUGUI _hourTMP;

	[SerializeField]
	private Transform _hourAnchor;


	[SerializeField]
	private Transform _tallyGroupAnchor;

	[SerializeField]
	private GameObject _tallyGroupPF;

	[SerializeField]
	private GameObject[] _tallyMarks;

	private int _tally;


	void Start()
	{
		GamePlay.Instance.TimeChangedEvent += OnTimeChanged;
	}


	void Update()
	{
		
	}

	private void OnTimeChanged()
	{
		if (GamePlay.Instance.CurrentHour == 0)
		{
			_tally = GamePlay.Instance.CurrentDay % 5;

			if (_tally == 0)
			{
				GameObject group = Instantiate(_tallyGroupPF, _tallyGroupAnchor, false);
				group.SetActive(true);
			}

			for (int i = 0; i < _tallyMarks.Length; i++)
			{
				_tallyMarks[i].SetActive(i < _tally);
			}
		}
		
		float angle = -GamePlay.Instance.CurrentHour * 15;

		_hourAnchor.rotation = Quaternion.Euler(0f, 0f, angle);
	}
}
