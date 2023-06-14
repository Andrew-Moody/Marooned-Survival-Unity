using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawIfEnumAttribute : PropertyAttribute
{
	public string PropertyName { get; private set; }
	public object EnumValue { get; private set; }


	public DrawIfEnumAttribute(string propertyName, object enumValue)
	{
		PropertyName = propertyName;

		EnumValue = enumValue;
	}
}
