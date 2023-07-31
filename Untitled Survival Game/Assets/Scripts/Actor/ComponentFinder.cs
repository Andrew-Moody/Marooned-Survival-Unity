using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;
using AbilityActor = AbilitySystem.AbilityActor;
using FishNet.Component.Transforming;

public class ComponentFinder : MonoBehaviour
{
	public NetworkTransform NetTransform => _netTransform;
	[SerializeField] private NetworkTransform _netTransform;

	public Stats Stats => _stats;
	[SerializeField] private Stats _stats;

	public Animator Animator => _animator;
	[SerializeField] private Animator _animator;

	public AudioSource AudioSource => _audioSource;
	[SerializeField] private AudioSource _audioSource;

	public AttachPoints AttachPoints => _attachPoints;
	[SerializeField] private AttachPoints _attachPoints;

	public TransformAnimator TransformAnimator => _transformAnimator;
	[SerializeField] private TransformAnimator _transformAnimator;

	public ViewTransform ViewTransform => _viewTransform;
	[SerializeField] private ViewTransform _viewTransform;

	public AbilityActor AbilityActor => _abilityActor;
	[SerializeField] private AbilityActor _abilityActor;

	public Agent Agent => _agent;
	[SerializeField] private Agent _agent;

	// This looks bad, but keep in mind it is only ever run at edit time
	public void SearchComponents()
	{
		Debug.Log("Searching");

		_netTransform = GetComponentInChildren<NetworkTransform>();

		_stats = GetComponentInChildren<Stats>();

		_animator = GetComponentInChildren<Animator>();

		_audioSource = GetComponentInChildren<AudioSource>();

		_attachPoints = GetComponentInChildren<AttachPoints>();

		_transformAnimator = GetComponentInChildren<TransformAnimator>();

		_viewTransform = GetComponentInChildren<ViewTransform>();

		_abilityActor = GetComponentInChildren<AbilityActor>();

		_agent = GetComponentInChildren<Agent>();
	}
}
