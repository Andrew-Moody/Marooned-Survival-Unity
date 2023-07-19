using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LegacyAbility;

public class WorldStatDisplay : MonoBehaviour
{
	[SerializeField]
	private Stats _stats;

	[SerializeField]
	private List<StatBar> _statBars;

	[SerializeField]
	private TextMeshProUGUI _infoTMP;

	[SerializeField]
	private Transform _lookTarget;

	[SerializeField]
	private Transform _statTransform;

	[SerializeField]
	private float _near;

	[SerializeField]
	private float _far;

	[SerializeField]
	private float _nearAngle;

	[SerializeField]
	private float _farAngle;

	private readonly Dictionary<StatType, StatBar> _statBarDict = new Dictionary<StatType, StatBar>();


	public void Show(bool show)
	{
		gameObject.SetActive(show);
	}

	public void SetInfoText(string text)
	{
		_infoTMP.text = text;
	}

	private void Awake()
	{
		for (int i = 0; i < _statBars.Count; i++)
		{
			_statBarDict.Add(_statBars[i].StatType, _statBars[i]);
		}

		_stats.OnStatChange += StatChangeHandler;

		foreach (StatBar stat in _statBars)
		{
			stat.SetAlpha(0f);
		}
	}


	private void Start()
	{
		_lookTarget = Camera.main.transform;

		transform.SetParent(UIManager.Instance.WorldCanvas.transform);
	}



	private void OnDestroy()
	{
		if (_stats != null)
		{
			_stats.OnStatChange -= StatChangeHandler;
		}
		
	}


	private void StatChangeHandler(StatData statData, bool immediate)
	{
		if (_statBarDict.ContainsKey(statData.StatType))
		{
			_statBarDict[statData.StatType].StatChangeHandler(statData, immediate);
		}
	}


	private void LateUpdate()
	{
		if (_lookTarget == null)
		{
			return;
		}

		_statTransform.rotation = _lookTarget.rotation;

		Vector3 direction = _statTransform.position - _lookTarget.position;

		float distance = direction.magnitude;

		if (distance < _far + 10f)
		{
			float angle = Vector3.Angle(_lookTarget.forward, direction);

			float alpha = Mathf.InverseLerp(_far, _near, distance) * Mathf.InverseLerp(_farAngle, _nearAngle, angle);

			foreach (StatBar stat in _statBars)
			{
				stat.SetAlpha(alpha);
			}
		}

		
	}


}
