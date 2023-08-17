using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;
using AsyncTasks;
using FishNet.Connection;

namespace AbilitySystem
{
	public class AbilityActor : NetworkBehaviour
	{
		public event TaskEventHandler TaskEventRecieved;

		public event System.Action<float> ActorTicked;

		public Actor Actor => _actor;
		private Actor _actor;

		public bool IsAbilityActive => _activeAbility != null;

		[Tooltip("An Effect must have these traits to be applied")]
		[SerializeField] private AbilityTraits _requiredTraits;

		[Tooltip("An Effect can't have these traits to be applied")]
		[SerializeField] private AbilityTraits _blockingTraits;

		// Allow intial abilities to be defined in the inspector
		[SerializeField] private List<AbilityInputBinding> _startingAbilities;

		private AbilitySet _abilitySet;

		private List<EffectHandle> _activeEffects = new List<EffectHandle>();

		[Tooltip("Allows Cues to be overridden")]
		[SerializeField] private List<Cue> _cues;
		private Dictionary<int, Cue> _cueOverrides;

		private AbilityHandle _activeAbility;


		public override void OnStartNetwork()
		{
			base.OnStartNetwork();

			_actor = Actor.FindActor(gameObject);


			Debug.LogWarning($"AbilityActor OnStartNetwork {_actor.gameObject.name}, {gameObject.name}");

			SetupStartingAbilities();
			SetupCueOverrides();
		}


		[Server]
		public void GiveAbility(AbilityInput abilityInput, Ability ability)
		{
			AbilityHandle handle = new AbilityHandle(ability, this, abilityInput);

			if (abilityInput == AbilityInput.UseImmediate)
			{
				if (handle.CanActivate())
				{
					if (_activeAbility != null)
					{
						_activeAbility.Cancel();
					}

					_activeAbility = handle;

					handle.Activate();
				}
			}
			else
			{
				_abilitySet.SetAbilityOverride(handle);
			}
		}


		public void HandleEvent(object sender, TaskEventData data)
		{
			OnTaskEventRecieved(sender, data);
		}


		public void ApplyEffect(EffectHandle effectHandle)
		{
			Effect effect = effectHandle.Effect;

			if (effect == null)
			{
				Debug.LogError("Effect was null");
				return;
			}


			// First check that the effect can be applied based on traits
			if (!CanApply(effectHandle))
			{
				Debug.LogError("Can't apply effect");
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

					// For now only on server
					HandleAppliedAbilities(effectHandle);
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


		public void ActivateAbility(AbilityInput abilityInput, AbilityEventHandler abilityEndedCallback = null)
		{
			if (TryActivateFromInput(abilityInput, abilityEndedCallback))
			{
				if (IsServer)
				{
					ActivateAbilityORPC(abilityInput);
				}
				else if (IsOwner)
				{
					ActivateAbilitySRPC(abilityInput);
				}
				else
				{
					Debug.LogError("Ability Activated on Non Owning Client");
				}
			}
		}


		private void AbilityHandle_AbilityEnded(AbilityHandle handle, AbilityEventData data)
		{
			// Ending the ability directly by the client would allow the client to then fire the next ability
			// which would then play on the client but not on the server if the server has not finished by the time it recieves
			// the request for the next ability

			// Waiting for the server to relay back that the ability has ended may result in dropped input but that seems better than
			// than having the client think their input has been recieved and only finding out several seconds later that it hasn't
			
			// I have heard of some people using a que to process input. that might help add some forgiveness

			if (IsServer)
			{
				_activeAbility = null;

				if (OwnerId != -1) // don't try to send TRPC to mobs
				{
					EndAbilityTRPC(Owner);
				}
			}
		}


		private bool TryActivateAbility(AbilityHandle abilityHandle, AbilityEventHandler abilityEndCallback = null)
		{
			if (!abilityHandle.CanActivate())
			{
				Debug.LogWarning("Ability failed CanActivate()");
				return false;
			}

			
			if (_activeAbility != null)
			{
				// Need to check if the new ability is allowed to cancel the old ability (using traits?)

				if (abilityHandle == _activeAbility)
				{
					// For now at least block repeated abilities from cancelling
					return false;
				}

				_activeAbility.Cancel();
			}

			_activeAbility = abilityHandle;

			abilityHandle.AbilityEnded += AbilityHandle_AbilityEnded;

			if (abilityEndCallback != null)
			{
				abilityHandle.AbilityEnded += abilityEndCallback;
			}

			abilityHandle.Activate();

			return true;
		}


		private bool TryActivateFromInput(AbilityInput abilityInput, AbilityEventHandler abilityEndedCallback = null)
		{
			if (_abilitySet.TryFindAbility(abilityInput, out AbilityHandle abilityHandle))
			{
				return TryActivateAbility(abilityHandle, abilityEndedCallback);
			}

			Debug.LogWarning("Failed to find ability with ID: " + abilityInput);
			return false;
		}


		// Request the server to activate the ability
		[ServerRpc]
		private void ActivateAbilitySRPC(AbilityInput abilityInput)
		{
			if (TryActivateFromInput(abilityInput))
			{
				ActivateAbilityORPC(abilityInput);
			}
		}


		// Activate the ability on observers in order to play cues (but not localhost)
		// may be preferable to activate only the cues but they typically need extra data
		// this will save a lot of bandwidth unless I find a better way to activate cues over the network
		[ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
		private void ActivateAbilityORPC(AbilityInput abilityInput)
		{
			if (_abilitySet.TryFindAbility(abilityInput, out AbilityHandle abilityHandle))
			{
				abilityHandle.Activate();
			}
		}


		[TargetRpc]
		private void EndAbilityTRPC(NetworkConnection connection)
		{
			if (_activeAbility != null)
			{
				// May need other cleanup
				_activeAbility = null;
			}
		}


		private void SetupStartingAbilities()
		{
			_abilitySet = new AbilitySet(this, _startingAbilities, Actor.Inventory);
		}


		private void SetupCueOverrides()
		{
			_cueOverrides = new Dictionary<int, Cue>();

			foreach (Cue cue in _cues)
			{
				_cueOverrides.Add(cue.Trait.GetTraitKey(), cue);
			}
		}

		
		private void OnTick(float deltaTime)
		{
			ActorTicked?.Invoke(deltaTime);

			TickEffects(deltaTime);
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
			if (!_requiredTraits.MeetsAllRequirements(effectHandle.Effect.Traits))
			{
				Debug.Log($"Effect does not contain all required traits");
				return false;
			}


			foreach (AbilityTrait trait in effectHandle.Effect.Traits)
			{

				if (_blockingTraits.ContainsTrait(trait))
				{
					Debug.Log($"Effect contains blocked trait: {trait.ToString()}");
					return false;
				}
			}

			return true;
		}


		private void HandleCues(EffectHandle effectHandle, CueEventType cueEventType)
		{
			//Debug.LogError("HandleCues");

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
			Debug.LogError("HandleStatChanges");

			foreach (StatModifier modifier in effectHandle.Effect.Modifiers)
			{
				modifier.ApplyModifier(effectHandle.EventData.Source.Actor, _actor);
			}
		}


		private void HandleAppliedAbilities(EffectHandle effectHandle)
		{
			foreach (AbilityInputBinding abilityBinding in effectHandle.Effect.AppliedAbilities)
			{
				GiveAbility(abilityBinding.Input, abilityBinding.Ability);
			}
		}


		private void OnTaskEventRecieved(object sender, TaskEventData data)
		{
			TaskEventRecieved?.Invoke(sender, data);
		}


		// Handle in update for now but may move to TimeManager eventually
		private void Update()
		{
			OnTick(Time.deltaTime);
		}
	}
}
