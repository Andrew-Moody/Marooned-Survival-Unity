using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

namespace AbilitySystem
{
	public class AbilityActor : NetworkBehaviour
	{
		[SerializeField] private Stats _stats;

		public Actor Actor { get; set; }
		[SerializeField] private Actor _actor;

		[Header("Optional")]

		[SerializeField]
		private AbilityActor _parentActor;

		[SerializeField]
		private Transform _viewTransform;
		public Transform ViewTransform { get { return _viewTransform; } }

		
		public override void OnStartNetwork()
		{
			base.OnStartNetwork();

			SetupStartingAbilities();
			SetupCueOverrides();
		}


		#region AbilitySystem

		public bool AsOwner => IsOwner;

		public bool AsServer => IsServer;

		[Tooltip("An Effect must have these traits to be applied")]
		[SerializeField] private AbilityTraits _requiredTraits;

		[Tooltip("An Effect can't have these traits to be applied")]
		[SerializeField] private AbilityTraits _blockingTraits;

		// Allow intial abilities to be defined in the inspector
		[SerializeField] private List<Ability> _startingAbilities;

		// Instantiated ability instances
		private List<AbilityHandle> _abilities;

		private List<EffectHandle> _activeEffects = new List<EffectHandle>();

		[Tooltip("Allows Cues to be overridden")]
		[SerializeField] private List<Cue> _cues;
		private Dictionary<int, Cue> _cueOverrides;


		[Server]
		public void GiveAbility(Ability ability)
		{

		}


		public void ApplyEffect(EffectHandle effectHandle)
		{
			Effect effect = effectHandle.Effect;

			if (effect == null)
			{
				return;
			}


			// First check that the effect can be applied based on traits
			if (!CanApply(effectHandle))
			{
				return;
			}


			if (effect.DurationType == DurationType.Instant)
			{
				// Fire off any cues if this is a client
				if (IsClient)
				{
					HandleCues(effectHandle, CueEventType.OnExecute);
				}


				// Handle Stat changes only on server
				if (IsServer)
				{
					HandleStatChanges(effectHandle);
				}
			}
		}


		public bool TryHandleCue(AbilityTrait trait, CueEventType eventType, CueEventData data)
		{
			if (_cueOverrides != null && _cueOverrides.TryGetValue(trait.GetTraitKey(), out Cue cue))
			{
				Debug.LogWarning($"handling cue override: {cue.name}");
				cue.HandleCue(eventType, data);
				return true;
			}

			return false;
		}


		public void ActivateAbility(int abilityID)
		{
			if (IsServer)
			{
				ActivateAbilityAsServer(abilityID);
			}
			else if (IsOwner)
			{
				ActivateAbilityAsClient(abilityID);
			}
		}


		private void ActivateAbilityAsClient(int abilityID)
		{
			if (!TryFindAbilityFromID(abilityID, out AbilityHandle abilityHandle))
			{
				return;
			}


			// Activate the ability locally and request the server activate remotely
			if (abilityHandle.CanActivate())
			{
				abilityHandle.Activate();

				ActivateAbilitySRPC(abilityID);
			}
		}


		[Server]
		private void ActivateAbilityAsServer(int abilityID)
		{
			if (!TryFindAbilityFromID(abilityID, out AbilityHandle abilityHandle))
			{
				return;
			}

			if (abilityHandle.CanActivate())
			{
				abilityHandle.Activate();

				ActivateAbilityORPC(abilityID);
			}
		}


		// Request the server to activate the ability
		[ServerRpc]
		private void ActivateAbilitySRPC(int abilityID)
		{
			ActivateAbilityAsServer(abilityID);
		}


		// Activate the ability on observers in order to play cues (but not localhost)
		// may be preferable to activate only the cues but they typically need extra data
		// this will save a lot of bandwidth unless I find a better way to activate cues over the network
		[ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
		private void ActivateAbilityORPC(int abilityID)
		{
			if (!TryFindAbilityFromID(abilityID, out AbilityHandle abilityHandle))
			{
				return;
			}

			abilityHandle.Activate();
		}


		// The underlying relationship between ID and ability handle will most likely be more complex
		// This will suffice for testing
		private bool TryFindAbilityFromID(int abilityID, out AbilityHandle abilityHandle)
		{
			if (abilityID > -1 && abilityID < _abilities.Count)
			{
				abilityHandle = _abilities[abilityID];
				return true;
			}

			abilityHandle = null;
			return false;
		}


		private void SetupStartingAbilities()
		{
			_abilities = new List<AbilityHandle>();

			foreach (Ability ability in _startingAbilities)
			{
				_abilities.Add(new AbilityHandle(ability, this));
			}
		}


		private void SetupCueOverrides()
		{
			_cueOverrides = new Dictionary<int, Cue>();

			foreach (Cue cue in _cues)
			{
				_cueOverrides.Add(cue.Trait.GetTraitKey(), cue);
			}
		}

		// Handle in update for now but may move to TimeManager eventually
		private void Tick(float deltaTime)
		{
			TickAbilities(deltaTime);

			TickEffects(deltaTime);
		}

		// If cooldown ends up being the only thing updated here I may find a different way to handle cooldowns
		private void TickAbilities(float deltaTime)
		{
			foreach (AbilityHandle handle in _abilities)
			{
				handle.Tick(deltaTime);
			}
		}


		private void TickEffects(float deltaTime)
		{
			foreach (EffectHandle effect in _activeEffects)
			{
				effect.Tick(deltaTime);
			}
		}


		private bool CanApply(EffectHandle effectHandle)
		{
			foreach (AbilityTrait trait in effectHandle.Effect.Traits)
			{
				if (!_requiredTraits.ContainsTrait(trait) || _blockingTraits.ContainsTrait(trait))
				{
					return false;
				}
			}

			return true;
		}


		private void HandleCues(EffectHandle effectHandle, CueEventType cueEventType)
		{
			AbilityTrait[] traits = effectHandle.Effect.Cues;

			CueEventData data = new CueEventData()
			{
				Target = this,
			};

			foreach(AbilityTrait trait in traits)
			{
				CueManager.HandleCue(trait, cueEventType, data);
			}
		}


		private void HandleStatChanges(EffectHandle effectHandle)
		{
			Debug.LogError($"HandleStatChanges");

			foreach (StatModifier modifier in effectHandle.Effect.Modifiers)
			{
				modifier.ApplyModifier(_stats);
			}
		}


		#endregion

		private void Update()
		{
			Tick(Time.deltaTime);
		}
	}
}
