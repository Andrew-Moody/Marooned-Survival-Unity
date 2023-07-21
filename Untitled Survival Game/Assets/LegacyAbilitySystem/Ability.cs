using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace LegacyAbility
{
	// null = no change, I hope empty string = global namespace
	[MovedFrom(false, "", null, null)]
	[System.Serializable]
	public class Ability
	{
		[SerializeField]
		private string _abilityName;
		public string AbilityName { get { return _abilityName; } }

		[SerializeField]
		protected AbilityType _abilityType;
		public AbilityType AbilityType { get { return _abilityType; } }

		[SerializeField]
		private ToolType _toolType;
		public ToolType ToolType { get { return _toolType; } }

		[SerializeField]
		private float _toolPower;
		public float ToolPower { get { return _toolPower; } }

		[SerializeField]
		private float _coolDown;

		[SerializeField]
		private bool _requireTarget;

		[SerializeField]
		private StatRequirement[] _requirements;

		[SerializeReference]
		private List<Effect> _userEffects = new List<Effect>();

		[SerializeReference]
		private List<Effect> _targetEffects = new List<Effect>();

		[SerializeReference]
		private List<Effect> _itemEffects = new List<Effect>();

		protected List<Effect>[] _effectLists;

		private float _coolDownRemaining = 0f;

		private const float _coolDownThreshold = 0.001f;


		public Ability()
		{
			//Debug.LogWarning($"Ability Constructor called for {this}");
			_effectLists = new List<Effect>[] { _userEffects, _targetEffects, _itemEffects };
		}


		// Copy constructor
		public Ability(Ability ability)
		{
			//Debug.LogWarning("Ability Copy Constructor");
			_abilityName = ability._abilityName;

			_abilityType = ability._abilityType;

			_toolType = ability._toolType;

			_toolPower = ability._toolPower;

			_coolDown = ability._coolDown;

			_requireTarget = ability._requireTarget;

			_requirements = ability._requirements;

			_userEffects = ability._userEffects;

			_targetEffects = ability._targetEffects;

			_effectLists = ability._effectLists;
		}


		/// <summary>
		/// Check if the Ability is off cooldown and the user has sufficient energy
		/// </summary>
		/// <param name="user"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public virtual bool Useable(bool asServer, AbilityActor user, AbilityActor target = null)
		{
			if (_requireTarget && target == null)
			{
				return false;
			}


			if (asServer && _coolDownRemaining > _coolDownThreshold)
			{
				//Debug.LogError("On CoolDown, Time remaining: " + _coolDownRemaining);
				return false;
			}
			else if (!asServer && _coolDownRemaining > 0f)
			{
				return false;
			}


			if (user != null)
			{
				foreach (StatRequirement requirement in _requirements)
				{
					if (user.GetStatValue(requirement.StatType) < requirement.Value)
					{
						//Debug.Log($"User does not have enough {requirement.StatType} to use this ability");
						return false;
					}
				}
			}

			return true;
		}


		public virtual AbilityActor[] FindTargets(Vector3 userPosition, LayerMask targetMask)
		{
			Debug.LogError("Using base Ability.FindTargets()");
			return null;
		}


		public void StartCoolDown()
		{
			_coolDownRemaining = _coolDown;
		}


		public void UseAbility(AbilityActor user, AbilityActor item, AbilityActor target, EffectTiming effectTiming, bool asServer)
		{
			if (CheckAbilitySuccess(user, item, target, effectTiming, asServer))
			{
				ApplyEffects(user, item, target, effectTiming, asServer);
			}
			else
			{
				ApplyEffects(user, item, target, EffectTiming.OnFail, asServer);
			}
		}


		private void ApplyEffects(AbilityActor user, AbilityActor item, AbilityActor target, EffectTiming effectTiming, bool asServer)
		{
			Debug.Log($"Applying {effectTiming} Effects, asServer: {asServer}");

			AbilityActor effected;

			for (EffectTarget i = EffectTarget.User; i <= EffectTarget.Item; i++)
			{
				switch (i)
				{
					case EffectTarget.User:
					{
						effected = user;
						break;
					}
					case EffectTarget.Item:
					{
						effected = item;
						break;
					}
					case EffectTarget.Target:
					{
						effected = target;
						break;
					}
					default:
					{
						effected = null;
						break;
					}
				}

				if (effected == null)
				{
					continue;
				}


				foreach (Effect effect in _effectLists[((int)i - 1)])
				{
					if (effect.EffectTiming != effectTiming)
					{
						continue;
					}


					if (effect.EffectType == EffectType.Stat && !effect.ServerOnly)
					{
						Debug.Log("Falied to set ServerOnly true on effect of type Stat");
					}


					if (!effect.ServerOnly || asServer)
					{
						effect.ApplyEffect(this, user, effected);
					}
				}
			}
		}


		private bool CheckAbilitySuccess(AbilityActor user, AbilityActor item, AbilityActor target, EffectTiming effectTiming, bool asServer)
		{
			if (target != null)
			{
				if (target.ToolType != ToolType.None && target.ToolType != _toolType)
				{
					Debug.Log($"{_abilityName} with Tooltype {_toolType} can't effect target {target.name} with Tooltype {target.ToolType} ");
					return false;
				}

				if (target.ToolPower > _toolPower)
				{
					Debug.Log($"{_abilityName} with ToolPower {_toolPower} can't effect target {target.name} with ToolPower {target.ToolPower} ");
					return false;
				}
			}

			return true;
		}


		/// <summary>
		/// Update the ability with the current deltaTime
		/// </summary>
		/// <param name="deltaTime"></param>
		public virtual void TickAbility(float deltaTime)
		{
			if (_coolDownRemaining > 0f)
			{
				_coolDownRemaining -= deltaTime;
			}
		}


		public void ValidateEffects()
		{
			//Debug.LogError($"_abilityType: {_abilityType}, type {this.GetType()}");

			for (int listIdx = 0; listIdx < _effectLists.Length; listIdx++)
			{
				List<Effect> effectList = _effectLists[listIdx];

				for (int i = 0; i < effectList.Count; i++)
				{
					Effect effect = effectList[i];

					if (effect == null)
					{
						effect = new Effect();
					}

					if (effect.EffectType.ToString() + "Effect" != effect.GetType().ToString())
					{
						effect = EffectFactory.CreateEffect(effect.EffectType);
					}


					effectList[i] = effect;
				}
			}
		}

		[System.Serializable]
		private struct StatRequirement
		{
			public StatType StatType;

			public float Value;
		}
	}
}
