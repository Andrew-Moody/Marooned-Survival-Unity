using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CompactEnumAttribute))]
public class CompactEnumDrawer : PropertyDrawer
{


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{

		//EditorGUI.PropertyField(position, property, label);

		EditorGUI.PropertyField(position, property, GUIContent.none);
	}
}
