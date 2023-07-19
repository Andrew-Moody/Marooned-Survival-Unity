using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LegacyAbility.Effect))]
public class EffectDrawer : PropertyDrawer
{
	private const float PADDING = 2f;

	private const float SPACE = 10f;


	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{

		float height = 8f;

		if (property.isExpanded)
		{
			IEnumerator enumerator = property.GetEnumerator();

			SerializedProperty currentProp;

			while (enumerator.MoveNext())
			{
				currentProp = enumerator.Current as SerializedProperty;
				height += EditorGUI.GetPropertyHeight(currentProp) + PADDING;
			}
		}
		else
		{
			height += 20f;
		}


		return height;
	}


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		int indentCache = EditorGUI.indentLevel;

		SerializedProperty copy = property.Copy();


		if (!copy.hasVisibleChildren)
		{
			if (copy.hasChildren)
			{
				Debug.LogWarning("Effect has no visible children");
			}
			else
			{
				Debug.LogWarning("Effect has no children");
			}


			Debug.Log(copy.name + " at " + copy.propertyPath + " IsExpanded: " + copy.isExpanded);
			return;
		}

		Rect propRect = new Rect(position.x, position.y, position.width, 18f);

		IEnumerator enumerator = copy.GetEnumerator();

		enumerator.MoveNext(); // Have to do this first or current will be null

		SerializedProperty currentProp = enumerator.Current as SerializedProperty;

		EditorGUI.PropertyField(propRect, currentProp);
		property.isExpanded = EditorGUI.Foldout(propRect, property.isExpanded, GUIContent.none);

		propRect.y += 20f;

		if (property.isExpanded)
		{
			while (enumerator.MoveNext())
			{
				//Debug.Log("Doing Something");
				currentProp = enumerator.Current as SerializedProperty;
				propRect.height = EditorGUI.GetPropertyHeight(currentProp);
				EditorGUI.PropertyField(propRect, currentProp);
				propRect.y += propRect.height + PADDING;
			}
		}


		EditorGUI.indentLevel = indentCache;


		EditorGUI.EndProperty();
	}
}


//public class EffectListDrawer : PropertyDrawer
//{
//	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//	{
//		float height = 20f;

//		SerializedProperty effects = property.FindPropertyRelative("_effects");

//		if (effects.isExpanded)
//		{
//			for (int i = 0; i < effects.arraySize; i++)
//			{
//				height += EditorGUI.GetPropertyHeight(effects.GetArrayElementAtIndex(i));
//			}
//		}

//		height += 24f; // Add room for buttons

//		return height;
//	}


//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//	{
//		EditorGUI.BeginProperty(position, label, property);

//		int indentCache = EditorGUI.indentLevel;


//		SerializedProperty effects = property.FindPropertyRelative("_effects");
		
//		Debug.Log(property.name + " at " + property.propertyPath);
//		Debug.Log(effects.name + " at " + effects.propertyPath + " Size: " + effects.arraySize);


//		EditorGUI.PropertyField(position, effects, true);


//		//Rect addButtonRect = new Rect(position.x, position.y, position.width, 20f);

//		//if (GUI.Button(addButtonRect, "+"))
//		//{

//		//}



//		EditorGUI.indentLevel = indentCache;


//		EditorGUI.EndProperty();
//	}
//}