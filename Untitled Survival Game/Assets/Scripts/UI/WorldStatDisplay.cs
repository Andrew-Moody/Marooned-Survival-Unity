using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Actors;

public class WorldStatDisplay : MonoBehaviour
{
	[SerializeField]
	private List<StatIndicator> _statIndicators;

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

	private readonly Dictionary<string, StatIndicator> _statIndicatorDict = new Dictionary<string, StatIndicator>();


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
		for (int i = 0; i < _statIndicators.Count; i++)
		{
			_statIndicatorDict.Add(_statIndicators[i].StatName, _statIndicators[i]);
		}


		Actor actor = Actor.FindActor(gameObject);

		actor.DeathFinished += Actor_DeathFinished;
		actor.NetworkStarted += Actor_NetworkStarted;

		_target = actor.Stats;

		if (_target != null)
		{
			_target.UIEvent += UIEvent_StatChanged;
		}
		else
		{
			Debug.LogWarning("WorldStatDisplay failed to find component that implements IUIEventPublisher");
		}

		foreach (StatIndicator stat in _statIndicators)
		{
			stat.SetAlpha(0f);
		}
	}

	private void Actor_NetworkStarted(IActor actor, ActorEventData data)
	{
		if (actor is Actor act)
		{
			if (act.Owner.IsLocalClient)
			{
				Show(false);
			}
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
		if (_statIndicatorDict.TryGetValue(data.TagString, out StatIndicator statIndicator))
		{
			statIndicator.StatChangeHandler(data);
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

			foreach (StatIndicator stat in _statIndicators)
			{
				stat.SetAlpha(alpha);
			}
		}
	}
}
