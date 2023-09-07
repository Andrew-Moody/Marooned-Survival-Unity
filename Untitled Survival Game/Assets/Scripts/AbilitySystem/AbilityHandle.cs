using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;
using AsyncTasks;

namespace AbilitySystem
{
	// The runtime representation of an ability
	// Allows the ability scriptable object to define default values and behavior.
	// Per instance runtime data (cooldown remaining for example) is held separatly so that
	// a new instance of ability SO does not need to be instantiated
	public class AbilityHandle : ITaskUser
	{
		// This should fire when an ability stops regardless of how (completed, canceled, or failed)
		public event AbilityEventHandler AbilityEnded;

		// Still need to implement these
		//public event AbilityEventHandler AbilityCompleted;
		//public event AbilityEventHandler AbilityCanceled;
		//public event AbilityEventHandler AbilityFailed;

		// Was going to use this with item abilities but haven't needed to yet
		public event AbilityEventHandler CooldownEnded;

		public Actor Actor => _actor;
		private Actor _actor;

		public AbilityActor User => _user;
		private AbilityActor _user;

		public AbilityInput InputBinding => _inputBinding;
		private AbilityInput _inputBinding;


		// Used to store runtime data for specific instance of an ability
		public AbilityInstanceData AbilityData => _abilityData;
		private AbilityInstanceData _abilityData;

		// This assumes a given ability can only have one task. In general this is still enough flexibility
		// given the ability to make new tasks for specific scenarios
		public AsyncTask Task { get => _task; set => _task = value; }
		private AsyncTask _task;

		// This is meant for data associated with a specific activation of a task
		// In particular, items can have shared ability handles (to prevent the player spamming abilities by having multiple of the same item)
		// The particular item used in a specific activation will be stored in AbilityEventData
		public AbilityEventData ActivationData { get => _activationData; set => _activationData = value; }
		private AbilityEventData _activationData;

		public bool IsOnCooldown => _coolDownRemaining > 0f;
		private float _coolDownRemaining;

		// The ability implementation
		private Ability _ability;

		public AbilityHandle(Ability ability, AbilityActor user, AbilityInput inputBinding)
		{
			_user = user;

			_actor = user != null ? user.Actor : null;

			_ability = ability;

			_abilityData = _ability.CreateInstanceData(user);

			_inputBinding = inputBinding;
		}


		public AbilityHandle(Ability ability, AbilityActor user, AbilityInput inputBinding, AbilityEventData activationData)
			: this(ability, user, inputBinding)
		{
			_activationData = activationData;
		}

		public bool CanActivate()
		{
			if (_coolDownRemaining > 0f)
			{
				return false;
			}

			return _ability.CanActivate(this);
		}


		public void Activate()
		{
			_coolDownRemaining = _ability.Cooldown;

			if (_coolDownRemaining > 0)
			{
				User.ActorTicked += User_ActorTicked;
			}

			_ability.Activate(this);
		}


		public void Cancel()
		{
			_ability.Cancel(this);
		}


		public void OnAbilityEnded(AbilityEventData data)
		{
			AbilityEnded?.Invoke(this, data);
		}

		/// <summary>
		/// Deduct time from the abilities cooldown
		/// </summary>
		/// <param name="deltaTime"></param>
		private void User_ActorTicked(float deltaTime)
		{
			_coolDownRemaining -= deltaTime;

			if (_coolDownRemaining < 0f)
			{
				_coolDownRemaining = 0f;

				User.ActorTicked -= User_ActorTicked;

				CooldownEnded?.Invoke(this, null);
			}
		}
	}
}
