using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class EditorState
{
	[FSMChoice(FSMChoiceType.State)]
	public string StateChoice;

	public FSMList<EditorTransition> Transitions;
}


[CustomPropertyDrawer(typeof(EditorState))]
public class StateDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		SerializedProperty stateChoice = property.FindPropertyRelative("StateChoice");

		float height = EditorGUI.GetPropertyHeight(stateChoice);

		SerializedProperty transitionContent = property.FindPropertyRelative("Transitions");

		if (property.isExpanded)
		{
			height += EditorGUI.GetPropertyHeight(transitionContent);

			Debug.Log(transitionContent.isExpanded + " " + EditorGUI.GetPropertyHeight(transitionContent));

			if (!transitionContent.isExpanded)
				height += 2f;
		}

		return height;
	}


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Needed to handle prefabs
		EditorGUI.BeginProperty(position, label, property);

		int initialIndent = EditorGUI.indentLevel;

		EditorGUI.indentLevel++;


		// Draw State Choice
		SerializedProperty stateChoice = property.FindPropertyRelative("StateChoice");
		Rect choiceRect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(stateChoice));
		EditorGUI.PropertyField(choiceRect, stateChoice, true);

		// Draw the fold out with the same rect as Choice and no label to use the property as the label
		property.isExpanded = EditorGUI.Foldout(choiceRect, property.isExpanded, "", false);
		if (property.isExpanded)
		{
			EditorGUI.indentLevel++;

			// Draw Transitions
			SerializedProperty transitions = property.FindPropertyRelative("Transitions");
			Rect transRect = new Rect(position.x, position.y + choiceRect.height, position.width, EditorGUI.GetPropertyHeight(transitions));
			EditorGUI.PropertyField(transRect, transitions, true);
		}

		EditorGUI.indentLevel = initialIndent;

		EditorGUI.EndProperty();
	}
}


[System.Serializable]
public class EditorTransition
{
	public string Name;

	[FSMChoice(FSMChoiceType.Condition)]
	public string Condition;

	public float Value;
	
	[FSMChoice(FSMChoiceType.State)]
	public string StateChoice;
}


[CustomPropertyDrawer(typeof(EditorTransition))]
public class TransitonDrawer : PropertyDrawer
{
	public const float CONDITION_HEIGHT = 20f;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		//SerializedProperty name = property.FindPropertyRelative("Name");

		//float height = EditorGUI.GetPropertyHeight(name);

		//SerializedProperty condition = property.FindPropertyRelative("Condition");
		//height += EditorGUI.GetPropertyHeight(condition);

		float height = CONDITION_HEIGHT + 2;

		return height;
	}


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		int indent = EditorGUI.indentLevel;

		Rect indentPos = EditorGUI.IndentedRect(position);

		float quarterWidth = indentPos.width / 4f;

		EditorGUI.indentLevel = 0;

		SerializedProperty name = property.FindPropertyRelative("Name");
		Rect nameRect = new Rect(indentPos.x, indentPos.y, quarterWidth, CONDITION_HEIGHT);
		//EditorGUI.PropertyField(nameRect, name, null, true);

		name.stringValue = EditorGUI.TextField(nameRect, name.stringValue);


		SerializedProperty condition = property.FindPropertyRelative("Condition");
		Rect condRect = new Rect(indentPos.x + quarterWidth, indentPos.y, quarterWidth, CONDITION_HEIGHT);
		EditorGUI.PropertyField(condRect, condition);


		SerializedProperty value = property.FindPropertyRelative("Value");
		Rect valueRect = new Rect(indentPos.x + 2 * quarterWidth, indentPos.y, quarterWidth, CONDITION_HEIGHT);
		value.floatValue = EditorGUI.FloatField(valueRect, value.floatValue);


		SerializedProperty state = property.FindPropertyRelative("StateChoice");
		Rect stateRect = new Rect(indentPos.x + 3 * quarterWidth, indentPos.y, quarterWidth, CONDITION_HEIGHT);
		EditorGUI.PropertyField(stateRect, state);

		//Rect indentRect = EditorGUI.IndentedRect(condRect);
		//Debug.Log(condRect.width + " vs " + indentRect.width);

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}




public interface IFSMList
{
	public void AddNewElement();

	public void RemoveLastElement();
}


[System.Serializable]
public class FSMList<T> : IFSMList
{
	public List<T> _items = new List<T>();

	public T this[int i]
	{
		get { return _items[i]; }
	}


	public int Count
	{
		get { return _items.Count; }
	}


	public void AddNewElement()
	{
		_items.Add(Activator.CreateInstance<T>());
	}


	public void RemoveLastElement()
	{
		if (_items.Count > 0)
		{
			_items.RemoveAt(_items.Count - 1);
		}
	}

}

// Need one of these for each type of list to draw
[CustomPropertyDrawer(typeof(FSMList<EditorState>))]
public class FSMSateListDrawer : FSMListDrawer<EditorState> { }

[CustomPropertyDrawer(typeof(FSMList<EditorTransition>))]
public class FSMTransitionListDrawer : FSMListDrawer<EditorTransition> { }


// This does work but you have to derive one for
public class FSMListDrawer<T> : PropertyDrawer
{
	private const float FOLDOUT_HEIGHT = 20f;

	private const float LINE_HEIGHT = 16f;

	private const float BUTTON_HEIGHT = 16f;

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		SerializedProperty elements = property.FindPropertyRelative("_items");

		float height = FOLDOUT_HEIGHT;

		if (property.isExpanded)
		{
			// Unfortunately it doesnt seem to know its expanded height (even with includeChildren true) so have to loop over elements
			for (int i = 0; i < elements.arraySize; i++)
			{
				height += EditorGUI.GetPropertyHeight(elements.GetArrayElementAtIndex(i));
			}

			height += BUTTON_HEIGHT; // Make room for the buttons
		}

		return height;
	}


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		Rect foldRect = new Rect(position.x, position.y, position.width, FOLDOUT_HEIGHT);

		property.isExpanded = EditorGUI.Foldout(foldRect, property.isExpanded, label);


		if (property.isExpanded)
		{
			Rect buttonRect = new Rect(position.width * 0.8f, position.y, position.width * 0.125f, BUTTON_HEIGHT);

			if (GUI.Button(buttonRect, "+"))
			{
				FSMList<T> list = EditorHelper.GetObjectFromProperty(property) as FSMList<T>;

				if (list != null)
				{
					list.AddNewElement();
				}
			}

			buttonRect.x += position.width * 0.125f;

			if (GUI.Button(buttonRect, "-"))
			{
				FSMList<T> list = EditorHelper.GetObjectFromProperty(property) as FSMList<T>;

				if (list != null)
				{
					list.RemoveLastElement();
				}
			}

			float yOffset = FOLDOUT_HEIGHT;

			SerializedProperty elements = property.FindPropertyRelative("_items");

			for (int i = 0; i < elements.arraySize; i++)
			{
				Rect rect = new Rect(position.x, position.y + yOffset, position.width, EditorGUI.GetPropertyHeight(elements.GetArrayElementAtIndex(i)));

				// Label is overridden if the drawn property has a custom drawer
				EditorGUI.PropertyField(rect, elements.GetArrayElementAtIndex(i), new GUIContent("Item " + i), true);

				// move the offset to the end of the current rect
				yOffset += rect.height;

				Debug.Log("Type: " + typeof(T) + " Idx: " + i + " Height: " + rect.height);
			}
		}

		EditorGUI.EndProperty();
	}
}


public enum FSMChoiceType
{
	None,
	State,
	Condition
}


public class FSMChoiceAttribute : PropertyAttribute
{
	public FSMChoiceType ChoiceType;

	public FSMChoiceAttribute(FSMChoiceType choiceType)
	{
		ChoiceType = choiceType;
	}
}

[CustomPropertyDrawer(typeof(FSMChoiceAttribute))]
public class FSMChoiceDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		float height = 20f;

		return height;
	}


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType != SerializedPropertyType.String)
		{
			EditorGUI.LabelField(position, property.displayName, "FSMChoiceAttribute can only be applied to Strings");
			return;
		}

		FSMChoiceType choiceType = (attribute as FSMChoiceAttribute).ChoiceType;

		switch (choiceType)
		{
			case FSMChoiceType.State:
			{
				DrawStringChoice(position, label, property, SMFactorySO.Options);
				break;
			}
			case FSMChoiceType.Condition:
			{
				DrawStringChoice(position, label, property, SMFactorySO.Conditions);
				break;
			}
			default:
			{
				break;
			}
		}
	}


	public void DrawStringChoice(Rect position, GUIContent label, SerializedProperty property, string[] options)
	{
		EditorGUI.BeginProperty(position, label, property);

		int index = Array.IndexOf(options, property.stringValue);

		// Draw State Selection Popup and update selection if changed
		EditorGUI.BeginChangeCheck();
		index = EditorGUI.Popup(position, index, options);
		if (EditorGUI.EndChangeCheck())
		{
			property.stringValue = options[index];
		}

		EditorGUI.EndProperty();
	}
}