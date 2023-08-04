using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "SequenceAbility", menuName = "AbilitySystem/Ability/SequenceAbility")]
	public class SequenceAbility : Ability
	{
		[SerializeField] private List<Ability> _abilities;


		public override void Activate(AbilityHandle handle)
		{
			ActivateNextAbility(handle);
		}


		public override AbilityInstanceData CreateInstanceData(AbilityActor user)
		{
			List<AbilityHandle> handles = new List<AbilityHandle>();

			foreach (Ability ability in _abilities)
			{
				AbilityHandle handle = new AbilityHandle(ability, user, AbilityInput.None);

				handle.AbilityEnded += AbilityHandle_AbilityEnded;

				handles.Add(handle);

				
			}

			return new SequenceAbilityData()
			{
				SequenceIndex = 0,
				AbilityHandles = handles
			};
		}


		private void ActivateNextAbility(AbilityHandle handle)
		{
			if (handle.AbilityData is SequenceAbilityData data)
			{
				if (data.SequenceIndex < data.AbilityHandles.Count)
				{
					Debug.Log($"Activating handle in sequence: {data.SequenceIndex}");

					AbilityHandle abilityHandle = data.AbilityHandles[data.SequenceIndex];

					abilityHandle.ActivationData = new SequenceActivateEventData() { ParentHandle = handle };

					abilityHandle.Activate();

					data.SequenceIndex++; // Very important, do not forget!
				}
				else
				{
					data.SequenceIndex = 0;
					End(handle);
					return;
				}
				
			}
		}


		private void AbilityHandle_AbilityEnded(AbilityHandle ability, AbilityEventData data)
		{
			if (ability.ActivationData is SequenceActivateEventData activationData)
			{
				ActivateNextAbility(activationData.ParentHandle);
			}
		}
	}
}
