using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	public class EffectHandle
	{
		public Effect Effect => _effect;

		public EffectEventData EventData => _effectEventData;

		private Effect _effect;

		private EffectEventData _effectEventData;


		public EffectHandle(Effect effect, EffectEventData data)
		{
			_effect = effect;
			_effectEventData = data;
		}


		public void Tick(float deltaTime)
		{

		}
	}
}
