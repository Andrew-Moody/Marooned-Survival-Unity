using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "ParticleCue", menuName = "AbilitySystem/ParticleOneShotCue")]
	public class ParticleOneShotCue : CueOneShot
	{
		[SerializeField] private GameObject _particlePF;

		[SerializeField] private bool _attachToOwner;


		public override void OnExecute(CueEventData data)
		{
			base.OnExecute(data);

			Transform parent = null;

			if (_attachToOwner)
			{
				parent = data.Target.transform;
			}


			if (_particlePF == null)
			{
				Debug.LogError("ParticleOneShotCue has no particle effect specified");
				return;
			}

			GameObject particleObj = Instantiate(_particlePF, data.Position, Quaternion.identity, parent);

			if (particleObj.TryGetComponent(out ParticleSystem particle))
			{
				particle.Play();

				// For particle effects to be reused in different contexts, settings like what to do when finished need to be set in code

				ParticleSystem.MainModule main = particle.main;
				main.stopAction = ParticleSystemStopAction.Destroy;
			}

		}


		private void OnValidate()
		{
			if (_particlePF != null && !_particlePF.GetComponent<ParticleSystem>())
			{
				_particlePF = null;
				Debug.LogError("ParticlePF must be a prefab that has a ParticleSystem component");
			}
		}
	}
}
