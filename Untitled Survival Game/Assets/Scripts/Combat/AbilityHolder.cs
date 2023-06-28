using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AbilityHolder")]
public class AbilityHolder : ScriptableObject
{
	[SerializeReference]
	private IAbility _ability = new DerivedAbility();

	[SerializeField]
	private int _test;

	[SerializeReference]
	private List<IAbility> _abilities;


	private void OnValidate()
	{
		for (int i = 0; i < _abilities.Count; i++)
		{
			Debug.Log($"Element {i} is null: {_abilities[i] == null}");
		}

		if (_ability != null)
		{
			Debug.LogError(_ability.GetText());
		}
		else
		{
			Debug.LogError("Ability was null");
		}
	}

	[ContextMenu("ContextMenu")]
	private void ContextMenu()
	{
		Debug.Log("ContextMenu");
	}


	private void ContextMenuItem()
	{
		Debug.Log("ContextMenuItem");
	}
}
