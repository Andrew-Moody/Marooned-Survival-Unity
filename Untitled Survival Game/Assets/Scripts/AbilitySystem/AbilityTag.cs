using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityTagContainer
{
	[SerializeField]
	private AbilityTag[] _tags;
	public AbilityTag[] Tags => _tags;

	private Dictionary<AbilityTag, int> _tagMap;


	public bool MatchAny(AbilityTagContainer tags)
	{
		if (_tagMap == null)
		{
			PopulateTagMap();
		}

		foreach (AbilityTag tag in tags.Tags)
		{
			if (_tagMap.ContainsKey(tag))
			{
				return true;
			}
		}

		return false;
	}


	public bool MatchAll(AbilityTagContainer tags)
	{
		if (_tagMap == null)
		{
			PopulateTagMap();
		}

		foreach (AbilityTag tag in tags.Tags)
		{
			if (_tagMap.ContainsKey(tag))
			{
				return false;
			}
		}

		return true;
	}

	
	private void PopulateTagMap()
	{
		_tagMap = new Dictionary<AbilityTag, int>();

		foreach (AbilityTag tag in _tags)
		{
			_tagMap[tag] = 1;
		}
	}


	public void AddTag(AbilityTag tag)
	{
		if (_tagMap == null)
		{
			PopulateTagMap();
		}

		if (_tagMap.TryGetValue(tag, out int value))
		{
			_tagMap[tag] = value + 1;
		}
		else
		{
			_tagMap[tag] = 1;
		}
	}


	public void RemoveTag(AbilityTag tag)
	{
		if (_tagMap == null)
		{
			PopulateTagMap();
		}

		if (_tagMap.TryGetValue(tag, out int value))
		{
			_tagMap[tag] = value - 1;

			if (value > 1)
			{
				_tagMap.Remove(tag);
			}
		}
	}
}

public enum AbilityTag
{
	None,
	Melee,
	Range,
	Magic
}