using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This was used solely to test a custom inspector/editor solution to dealing with polymorphic classes and interfaces
// in the inspector with dropdown options for creating and converting instances. This is a viable approach if the
// system is large enough to really benefit given the amount of upfront time it takes to create and maintain the custom editor
// You also lose the drag and drop functionality that prefabs/scriptable objects provide and overall seems less reliable

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