using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldStatDisplay : MonoBehaviour
{
	[SerializeField][Tooltip("Must implement IUIEventPublisher")]
	private GameObject _targetObject;

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

	private IUIEventPublisher _target;

	private readonly Dictionary<string, StatBar> _statBarDict = new Dictionary<string, StatBar>();


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
			_statBarDict.Add(_statBars[i].StatName, _statBars[i]);
		}


		if (_targetObject != null)
		{
			_target = _targetObject.gameObject.GetComponent<IUIEventPublisher>();
		}

		if (_target != null)
		{
			_target.UIEvent += StatChangeHandler;
		}
		else
		{
			Debug.LogWarning("WorldStatDisplay failed to find component that implements IUIEventPublisher");
		}

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
		if (_target != null)
		{
			_target.UIEvent -= StatChangeHandler;
		}
	}


	private void StatChangeHandler(UIEventData data)
	{
		if (_statBarDict.TryGetValue(data.TagString, out StatBar statBar))
		{
			statBar.StatChangeHandler(data);
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


	private void OnValidate()
	{
		if (_targetObject != null)
		{
			if (_targetObject.GetComponent<IUIEventPublisher>() == null)
			{
				_targetObject = null;

				Debug.LogError("TargetObject must implement IUIEventPublisher");
			}
		}
	}
}
