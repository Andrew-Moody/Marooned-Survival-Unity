using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Actors;

public class WorldStatDisplay : MonoBehaviour
{
	[SerializeField]
	private List<StatBar> _statBars;

	[SerializeField]
	private TextMeshProUGUI _infoTMP;

	[SerializeField]
	private Transform _followTarget;

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


		Actor actor = Actor.FindActor(gameObject);

		actor.DeathFinished += Actor_DeathFinished;


		_target = actor.Stats;

		if (_target != null)
		{
			_target.UIEvent += UIEvent_StatChanged;
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

	private void Actor_DeathFinished(IActor actor, ActorEventData data)
	{
		actor.DeathFinished -= Actor_DeathFinished;
		Destroy(gameObject);
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
			_target.UIEvent -= UIEvent_StatChanged;
		}
	}


	private void UIEvent_StatChanged(UIEventData data)
	{
		if (_statBarDict.TryGetValue(data.TagString, out StatBar statBar))
		{
			statBar.StatChangeHandler(data);
		}
	}


	private void LateUpdate()
	{
		if (_followTarget != null)
		{
			transform.position = _followTarget.position;
		}

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
