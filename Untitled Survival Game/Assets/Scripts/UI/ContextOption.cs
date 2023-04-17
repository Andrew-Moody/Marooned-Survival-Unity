using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ContextOption
{
	public string OptionText;

	public int SlotIndex;

	public Action<int> Callback;


	public ContextOption(string optionText, int slotIndex, Action<int> callback)
	{
		OptionText = optionText;

		SlotIndex = slotIndex;

		Callback = callback;
	}

}


public enum Options
{
	Examine,
	Use,
	Take,
	Drop,
	Equip,
	Unequip,
	Teleport,
}