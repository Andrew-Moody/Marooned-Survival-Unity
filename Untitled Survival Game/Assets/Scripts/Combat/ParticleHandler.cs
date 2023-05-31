using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
	[SerializeField]
	private ParticleEffectData[] _particleEffects;

	private Dictionary<string, ParticleEffectData> _particleDict;


	public void PlayParticles(string name)
	{
		if (_particleDict.TryGetValue(name, out ParticleEffectData effect))
		{
			effect.ParticleSystem.Play();
		}

	}


	public void OverrideParticleEffects(ParticleEffectData[] effects)
	{
		for (int i = 0; i < effects.Length; i++)
		{
			ParticleEffectData newEffect = effects[i];

			if (_particleDict.TryGetValue(newEffect.Name, out ParticleEffectData oldEffect))
			{
				newEffect.Anchor = oldEffect.Anchor;
			}
			else
			{
				newEffect.Anchor = transform;
			}

			newEffect.ParticleSystem = Instantiate(newEffect.ParticleSystem, newEffect.Anchor, false);

			_particleDict[newEffect.Name] = newEffect;
		}
	}


	private void Awake()
	{
		_particleDict = new Dictionary<string, ParticleEffectData>();

		for (int i = 0; i < _particleEffects.Length; i++)
		{

			ParticleEffectData effect = _particleEffects[i];

			if (effect.ParticleSystem.gameObject.scene.name == null)
			{
				effect.ParticleSystem = Instantiate(effect.ParticleSystem, effect.Anchor, false);
			}

			_particleDict.Add(effect.Name, effect);
		}
	}
}


[System.Serializable]
public struct ParticleEffectData
{
	public string Name;

	public Transform Anchor;

	public ParticleSystem ParticleSystem;
}