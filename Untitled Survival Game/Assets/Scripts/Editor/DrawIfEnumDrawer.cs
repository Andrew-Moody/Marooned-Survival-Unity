using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawIfEnumAttribute))]
public class DrawIfEnumDrawer : PropertyDrawer
{
	private DrawIfEnumAttribute _drawIfEnumAttribute;

	private SerializedProperty _enumValue;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (EnumMatch(property))
		{
			return base.GetPropertyHeight(property, label);
		}

		return 0f;
	}



	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (EnumMatch(property))
		{
			EditorGUI.PropertyField(position, property, label);
		}

	}


	private bool EnumMatch(SerializedProperty property)
	{
		_drawIfEnumAttribute = attribute as DrawIfEnumAttribute;

		string path = _drawIfEnumAttribute.PropertyName;

		if (path.Contains("."))
		{
			path = System.IO.Path.ChangeExtension(property.propertyPath, path);
		}

		_enumValue = property.serializedObject.FindProperty(path);

		if (_enumValue == null)
		{
			Debug.LogError("Failed to find EnumValue for path: " + path);
		}
		else
		{
			return (_enumValue.enumValueIndex) == (int)_drawIfEnumAttribute.EnumValue;
		}

		return false;
	}
}
