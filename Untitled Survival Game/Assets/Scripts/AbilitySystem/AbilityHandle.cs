using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor = Actors.Actor;
using ITaskUser = AsyncTasks.ITaskUser;

namespace AbilitySystem
{
	// The runtime representation of an ability
	// Allows the ability scriptable object to define default values and behavior.
	// Per instance runtime data (cooldown remaining for example) is held separatly so that
	// a new instance of ability SO does not need to be instantiated
	public class AbilityHandle : ITaskUser
	{
		public Actor Actor => _actor;
		private Actor _actor;

		public AbilityActor User => _user;
		private AbilityActor _user;

		public AbilityInput InputBinding => _inputBinding;
		private AbilityInput _inputBinding;

		public AbilityInstanceData AbilityData => _abilityData;
		private AbilityInstanceData _abilityData;

		public bool IsOnCooldown => _coolDownRemaining > 0f;
		private float _coolDownRemaining;

		private Ability _ability;

		public AbilityHandle(Ability ability, AbilityActor user, AbilityInput inputBinding)
		{
			_user = user;

			_actor = user.Actor;

			_ability = ability;

			_abilityData = _ability.CreateInstanceData(user);

			_inputBinding = inputBinding;
		}

		public AbilityHandle(Ability ability, AbilityActor user, AbilityInput inputBinding, AbilityEventData data)
			: this(ability, user, inputBinding)
		{
			_abilityData.AbilityEventData = data;
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
			}
		}
	}
}
