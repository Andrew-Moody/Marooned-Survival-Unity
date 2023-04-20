using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability
{
	[SerializeField]
	private string _abilityName;
	public string AbilityName {  get { return _abilityName; } }

	[SerializeField]
	private AbilityType _abilityType;

	[SerializeField]
	private ToolType _toolType;

	[SerializeField]
	private float _coolDown;

	[SerializeField]
	private bool _requireTarget;

	[SerializeField]
	private StatRequirement[] _requirements;

	[SerializeField]
	private NamedValue[] _namedValues;

	//[SerializeField] Serialize field actually serializes non unity classes as value types rather than by reference
	// Doesn't work well with arrays unless you have a fixed sized know ahead of time
	//[SerializeReference]
	//private Effect[] _effects;

	[SerializeReference]
	private List<Effect> _userEffects = new List<Effect>();

	[SerializeReference]
	private List<Effect> _targetEffects = new List<Effect>();

	[SerializeReference]
	private List<Effect> _itemEffects = new List<Effect>();


	private List<Effect>[] _effectLists;


    private float _coolDownRemaining = 0f;

	private Dictionary<string, float> _namedValueDict;


	public Ability()
	{
		_effectLists = new List<Effect>[] { _userEffects, _targetEffects, _itemEffects };
	}


	// Copy constructor
	public Ability(Ability ability)
	{
		_abilityName = ability._abilityName;

		_abilityType = ability._abilityType;

		_toolType = ability._toolType;

		_coolDown = ability._coolDown;

		_requireTarget = ability._requireTarget;

		_requirements = ability._requirements;

		_namedValues = ability._namedValues;

		_userEffects = ability._userEffects;

		_targetEffects = ability._targetEffects;

		_effectLists = ability._effectLists;

	}


	public AbilityType GetAbilityType()
	{
		return _abilityType;
	}


	public float GetValue(string name)
	{
		if (_namedValueDict == null)
		{
			_namedValueDict = new Dictionary<string, float>();

			foreach (NamedValue namedValue in _namedValues)
			{
				_namedValueDict.Add(namedValue.Name, namedValue.Value);
			}
		}


		if (!_namedValueDict.TryGetValue(name, out float value))
		{
			Debug.LogWarning($"{_abilityName} does not have NamedValue: {name}");
		}

		return value;
	}


	/// <summary>
	/// Check if the Ability is off cooldown and the user has sufficient energy
	/// </summary>
	/// <param name="user"></param>
	/// <param name="target"></param>
	/// <returns></returns>
    public virtual bool Useable(AbilityActor user, AbilityActor target = null)
	{
		if (_requireTarget && target == null)
		{
			return false;
		}


		if (_coolDownRemaining > 0)
		{
			//Debug.Log("On CoolDown, Time remaining: " + _coolDownRemaining);
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
		Debug.LogWarning("Using base Ability.FindTargets()");
		return null;
	}


	public bool IsOffCoolDown()
	{
		return _coolDownRemaining <= 0f;
	}


	public bool RequiresTarget()
	{
		return _requireTarget;
	}

	public void StartCoolDown()
	{
		_coolDownRemaining = _coolDown;
	}


	public void ApplyEffects(AbilityActor user, AbilityActor item, AbilityActor target, EffectTiming effectTiming, bool asServer)
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


			if (effected != null && effected.GetToolType() != ToolType.None && effected.GetToolType() != _toolType)
			{
				// The ability does not have the correct tool type to effect this AbilityActor

				// Could apply failure effects like sound and particles
				if (effected != null)
				{
					Debug.LogWarning($"{_abilityName} with tooltype {_toolType} can't effect target {effected.name} with tooltype {effected.GetToolType()} ");
				}

				return;
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
					effect.ApplyEffect(user, effected);
				}
			}

		}

		
	}


	/// <summary>
	/// Update the ability with the current deltaTime
	/// </summary>
	/// <param name="deltaTime"></param>
	public virtual void TickAbility(float deltaTime)
	{
		if (_coolDownRemaining > 0)
		{
			_coolDownRemaining -= deltaTime;
		}
	}


	public void OnValidate()
	{
		for ( int listIdx = 0; listIdx < _effectLists.Length; listIdx++)
		{
			if (_effectLists[listIdx] == null)
			{
				_effectLists[listIdx] = new List<Effect>();
			}

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


				effectList[i] = effect; // Wow I am dumb
			}
		}
		
	}
}


[System.Serializable]
public struct StatRequirement
{
	public StatType StatType;

	public float Value;
}


[System.Serializable]
public struct NamedValue
{
	public string Name;

	public float Value;
}


public enum AbilityType
{
	None,
	Melee,
	Range,
	Magic
}


public enum ToolType
{
	None,
	Mining,
	Logging
}