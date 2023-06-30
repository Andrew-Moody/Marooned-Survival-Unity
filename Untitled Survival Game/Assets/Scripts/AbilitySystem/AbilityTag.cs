using UnityEngine;

[System.Serializable]
public class AbilityTag
{
	private AbilityTagValue _value;

	public bool CheckTag(AbilityTag tag)
	{
		return _value == tag._value;
	}

	private enum AbilityTagValue
	{
		None,
		Melee,
		Range,
		Magic
	}
}
