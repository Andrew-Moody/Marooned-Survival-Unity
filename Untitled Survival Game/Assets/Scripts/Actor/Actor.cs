using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

namespace Actors
{
	public class Actor : NetworkBehaviour, IActor
	{
		public event ActorEventHandler DeathStarted;

		public event ActorEventHandler DeathFinished;


		#region ComponentFinder

		public Transform NetTransform => Components.NetTransform.transform;

		public Stats Stats => Components.Stats;

		public Agent Agent => Components.Agent;

		public Animator Animator => Components.Animator;

		public AudioSource AudioSource => Components.AudioSource;

		public AttachPoints AttachPoints => Components.AttachPoints;

		public TransformAnimator TransformAnimator => Components.TransformAnimator;

		public ViewTransform ViewTransform => Components.ViewTransform;

		public AbilityActor AbilityActor => Components.AbilityActor;

		public Inventory Inventory => Components.Inventory;


		public ComponentFinder Components
		{
			get
			{
				if (_components == null)
				{
					_components = GetComponent<ComponentFinder>();
				}

				return _components;
			}
		}

		private ComponentFinder _components;

		#endregion

		public LayerMask HostilityMask => _hostilityMask;
		[SerializeField] private LayerMask _hostilityMask;

		//private ActorState _actorState;

		private Vector3 _spawnPosition;
		private Quaternion _spawnRotation;


		public override void OnStartNetwork()
		{
			base.OnStartNetwork();

			SetupServerAndClient();
		}


		public override void OnStartServer()
		{
			base.OnStartServer();

			Stats.StatEmptied += Stats_StatEmptied;
		}


		public override void OnStopServer()
		{
			base.OnStopServer();

			Stats.StatEmptied -= Stats_StatEmptied;
		}


		private void StartDeath()
		{
			//_actorState = ActorState.Dying;

			DeathStarted?.Invoke(this, new ActorEventData());

			DoDeathStart();
		}


		private void FinishDeath()
		{
			//_actorState = ActorState.Dead;

			DeathFinished?.Invoke(this, new ActorEventData());

			DoDeathFinish();

			Despawn();
		}


		private void AbilityHandle_AbilityEnded(AbilityHandle handle, AbilityEventData data)
		{
			FinishDeath();
		}


		/// <summary>
		/// Intended to be used only by StartDeath Template method
		/// </summary>
		protected virtual void DoDeathStart()
		{
			// By default attempt to activate a death ability
			if (AbilityActor != null)
			{
				AbilityActor.ActivateAbility(AbilitySystem.AbilityInput.UseOnDeath, AbilityHandle_AbilityEnded);
			}
		}


		/// <summary>
		/// Intended to be used only by FinishDeath Template method
		/// </summary>
		protected virtual void DoDeathFinish()
		{
			// Derived actors might use this to spawn items
		}


		private void Stats_StatEmptied(StatKind kind)
		{
			if (kind == StatKind.Health)
			{
				StartDeath();
			}
		}


		private void SetupServerAndClient()
		{
			if (!TryGetComponent(out _components))
			{
				Debug.LogError($"{gameObject.name} Missing Component Finder");
			}

			// Reset root to origin and displace net transform to spawn position
			_spawnPosition = transform.position;
			_spawnRotation = transform.rotation;

			// Root must be reset on client and server since root is not a network transform
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;

			// Set the net transform to the original spawn position on server
			if (IsServer)
			{
				NetTransform.position = _spawnPosition;
				NetTransform.rotation = _spawnRotation;
			}
		}


		public static Actor FindActor(GameObject gameObject)
		{
			Actor actor;

			Transform trans = gameObject.transform;

			while(trans != null)
			{
				actor = trans.GetComponent<Actor>();

				if (actor != null)
				{
					return actor;
				}

				trans = trans.parent;
			}

			Debug.LogError("Failed to find Actor");

			return null;
		}


		private enum ActorState
		{
			Alive,
			Dying,
			Dead
		}
	}
}
