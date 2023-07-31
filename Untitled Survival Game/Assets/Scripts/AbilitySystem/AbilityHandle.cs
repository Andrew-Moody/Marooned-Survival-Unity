using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ITaskUser = AsyncTasks.ITaskUser;

namespace AbilitySystem
{
	// The runtime representation of an ability
	// Allows the ability scriptable object to define default values and behavior.
	// Per instance runtime data (cooldown remaining for example) is held separatly so that
	// a new instance of ability SO does not need to be instantiated
	public class AbilityHandle : ITaskUser
	{
		private Ability _ability;

		public AbilityInput InputBinding { get; set; }

		public AbilityInstanceData AbilityData => _abilityData;
		private AbilityInstanceData _abilityData;

		public bool IsOnCooldown => _coolDownRemaining > 0f;

		private float _coolDownRemaining;

		public AbilityHandle(Ability ability, AbilityActor user)
		{
			_ability = ability;

			_abilityData = _ability.CreateInstanceData(user);
		}

		public AbilityHandle(Ability ability, AbilityActor user, AbilityEventData data)
		{
			_ability = ability;

			_abilityData = _ability.CreateInstanceData(user);

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
		/// <returns>true if cooldown has ended or false otherwise</returns>
		public bool TickCooldown(float deltaTime)
		{
			_coolDownRemaining -= deltaTime;

			if (_coolDownRemaining < 0f)
			{
				_coolDownRemaining = 0f;

				return true;
			}

			return false;
		}
	}
}
