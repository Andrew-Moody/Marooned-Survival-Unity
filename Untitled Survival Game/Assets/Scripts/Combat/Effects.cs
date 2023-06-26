using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


[System.Serializable]
public class Effect
{
	//public string EffectName;

	[CompactEnum]
	public EffectType EffectType = EffectType.None;

	[CompactEnum]
	public EffectTiming EffectTiming;

	public readonly bool ServerOnly;


	public Effect(bool serverOnly)
	{
		ServerOnly = serverOnly;
	}


	public Effect()
	{
		ServerOnly = false;
	}

	public virtual void ApplyEffect(Ability ability, AbilityActor user, AbilityActor effected)
	{
		Debug.Log("Using Base Ability (user, target)");
	}
}


public class ServerOnlyEffect : Effect
{
	public ServerOnlyEffect() : base(true) { }
}


/// <summary>
/// The type of Effect to apply
/// </summary>
public enum EffectType
{
	None,
	Animation,
	Particle,
	Sound,
	Stat,
	KnockBack,
	Transform,
	UseItem,
	SpawnProjectile,
	LaunchProjectile
}


/// <summary>
/// Who the Effect is applied to
/// </summary>
public enum EffectTarget
{
	None,
	User,
	Target,
	Item
}

/// <summary>
/// When the Effect occurs
/// </summary>
public enum EffectTiming
{
	None,
	OnStart,
	OnAnimEvent,
	OnEnd,
	OnFail
}


public class AnimationEffect : Effect
{
	public string AnimTrigger;

	public AnimationClip Animation;


	public static Effect Create()
	{
		return new AnimationEffect();
	}


	public override void ApplyEffect(Ability ability, AbilityActor user, AbilityActor effected)
	{
		if (Animation != null && AnimTrigger != null)
		{
			effected.SetAnimOverride(AnimTrigger, Animation);

			// Allows things like weapons and abilites overriding the animation for a particular state.
			// This becomes tricky because an ability will not know which rig is used for the user and target
			// I can determine which rig is used but then do you need a separate override for all possible rigs?

			// Probably best to avoid overrides or make an animation database to look up the appropriate animation to use

		}


		if (AnimTrigger != null && AnimTrigger != "")
		{
			effected.SetAnimTrigger(AnimTrigger);
		}
	}
}


public class SoundEffect : Effect
{
	public AudioClip Sound;

	public static Effect Create()
	{
		return new SoundEffect();
	}


	public override void ApplyEffect(Ability ability, AbilityActor user, AbilityActor effected)
	{
		if (Sound != null)
		{
			effected.PlaySound(Sound);
		}
	}
}


public class ParticleEffect : Effect
{
	public string ParticleName;

	public ParticleSystem ParticleSystem;

	public static Effect Create()
	{
		return new ParticleEffect();
	}


	public override void ApplyEffect(Ability ability, AbilityActor user, AbilityActor effected)
	{
		if (ParticleName != null && ParticleName != "")
		{
			effected.PlayParticles(ParticleName);
		}


		//if (effect.ParticleSystem != null)
		//{
		//	// Play particle system on the target
		//	// very likely will want to replace this to use name/enum only unless there are a large number of special cases

		//	// i.e. weapon has a set of particles that can be triggered by name like animation states

		//	// Users and Targets are trickier as the need particles for all abilities not just the abilities available to a single weapon

		//	// Should a weapon be able to override these particles like animation overrides?

		//	// Are overrides even needed at all?

		//	// if a weapon defines a unique particle effect to apply to the target or user the particle system can be instantiated with the weapon as parent
		//	// on hold the reference on the weapon as it will likely be used regularly will the weapon is equiped;
		//	// its just a matter of getting the anchor transform from the user/target

		//	// On second thought if you hit more than one enemy and they each need a custom blood effect for example
		//	// the second hit will cancel the first unless you instantiate a system for each enemy hit

		//	// may need a pool system for particles
		//}
	}
}


public class TransformEffect : Effect
{
	public string Name;

	public static Effect Create()
	{
		return new TransformEffect();
	}


	public override void ApplyEffect(Ability ability, AbilityActor user, AbilityActor effected)
	{
		effected.PlayTransformAnimation(Name);
	}
}



public class StatEffect : ServerOnlyEffect
{
	public StatType StatType;

	public float StatAmount;

	public static Effect Create()
	{
		return new StatEffect();
	}


	public override void ApplyEffect(Ability ability, AbilityActor user, AbilityActor effected)
	{
		if (EffectType == EffectType.Stat && StatType != StatType.None)
		{
			effected.AddToStat(StatType, StatAmount);
		}
	}
}


[System.Serializable]
public class KnockBackEffect : ServerOnlyEffect
{
	public float KnockBack;

	public static Effect Create()
	{
		return new KnockBackEffect();
	}


	public override void ApplyEffect(Ability ability, AbilityActor user, AbilityActor effected)
	{
		Vector3 direction = (effected.transform.position - user.transform.position).normalized;

		Debug.Log($"User: {user.transform.parent.gameObject.name}, Effected: {effected.transform.parent.gameObject.name}, Direction: {direction}");

		direction.y += Mathf.Atan(Mathf.Deg2Rad * 30f); // add an upward component

		effected.KnockBack(direction, KnockBack);
	}
}


public class UseItemEffect : ServerOnlyEffect
{
	public static Effect Create()
	{
		return new UseItemEffect();
	}


	public override void ApplyEffect(Ability ability, AbilityActor user, AbilityActor effected)
	{
		Debug.LogError($"Attempting to use item {effected.name}");

		effected.UseItem();
	}
}


public class SpawnProjectileEffect : ServerOnlyEffect
{
	[SerializeField]
	private ProjectileBase _projectile;


	public static Effect Create()
	{
		return new SpawnProjectileEffect();
	}


	public override void ApplyEffect(Ability ability, AbilityActor user, AbilityActor effected)
	{
		effected.SpawnProjectile(_projectile);
	}
}


public class LaunchProjectileEffect : ServerOnlyEffect
{
	[SerializeField]
	private Vector3 _velocity;

	[SerializeField]
	private bool _alignWithVelocity;


	public static Effect Create()
	{
		return new LaunchProjectileEffect();
	}


	public override void ApplyEffect(Ability ability, AbilityActor user, AbilityActor effected)
	{
		effected.LaunchProjectile(_velocity, _alignWithVelocity);
	}
}



public class EffectFactory
{
	delegate Effect EffectFactoryMethod();

	private static Dictionary<EffectType, EffectFactoryMethod> _factoryMethods = new Dictionary<EffectType, EffectFactoryMethod>();


	public static Effect CreateEffect(EffectType effectType)
	{
		if (effectType == EffectType.None)
		{
			return new Effect();
		}

		if (!_factoryMethods.ContainsKey(effectType))
		{
			// Use reflection once to find the relevant FactoryMethod and cache


			// This is a temporary work around to get this working, but there are safer ways to get type information
			// Im currently using enums because thats the only way to get a dropdown without a custom editor/property drawer
			// but requires updating when you add more options
			string typeString = effectType.ToString() + "Effect";

			Type type = Type.GetType(typeString); // Should probably wrap in a namespace

			if (type != null && typeof(Effect).IsAssignableFrom(type))
			{
				MethodInfo methodInfo = type.GetMethod("Create");

				if (methodInfo != null)
				{
					// Create a delegate from the method
					EffectFactoryMethod factoryMethod = Delegate.CreateDelegate(typeof(EffectFactoryMethod), methodInfo) as EffectFactoryMethod;

					_factoryMethods.Add(effectType, factoryMethod);
				}
			}
			else
			{
				return new Effect();
			}

		}

		Effect effect = _factoryMethods[effectType].Invoke();
		effect.EffectType = effectType;

		return effect;
	}
}