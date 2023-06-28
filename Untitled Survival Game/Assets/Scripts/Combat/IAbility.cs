using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IAbility
{
	public string GetText();
}


[System.Serializable]
public class DerivedAbility : IAbility
{
	public string Text = "DerivedAbility";

	public DerivedAbility()
	{

	}


	public DerivedAbility(IAbility ability)
	{
		Text += ability.GetText();
	}


	public string GetText()
	{
		return Text;
	}
}


public class DerivedAbility2 : IAbility
{
	public string Text = "DerivedAbility2";

	public string OtherText = " - More Text";


	public DerivedAbility2()
	{

	}


	public DerivedAbility2(IAbility ability)
	{
		Text += ability.GetText();
	}


	public DerivedAbility2(DerivedAbility2 derivedAbility)
	{
		Text = "Copied from DerivedAbility2";
	}


	public string GetText()
	{
		return Text + OtherText;
	}
}


public class EmptyAbility : IAbility
{
	public string Text = "EmptyAbility";
	public string GetText() => "EmptyAbility";

	public EmptyAbility() { }

	public EmptyAbility(IAbility ability) { }
}