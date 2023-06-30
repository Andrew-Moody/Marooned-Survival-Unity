using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(IAbility))]
public class AbilityDrawer : PropertyDrawer
{
	private VisualTreeAsset _visualTreeAsset;

	private VisualElement _root;

	private ToolbarMenu _toolbarMenu;

	private SerializedProperty _property;

	private const string ABILITY_PROPNAME = "_ability";

	public override VisualElement CreatePropertyGUI(SerializedProperty property)
	{
		_property = property;

		_visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolkit/AbilityDrawer.uxml");

		_root = new VisualElement();

		_visualTreeAsset.CloneTree(_root);

		SetupToolbar();

		//PropertyField ability = _root.Q<PropertyField>("ability");



		return _root;
	}


	private void SetupToolbar()
	{
		_toolbarMenu = _root.Q<ToolbarMenu>("ability-toolbar");

		_toolbarMenu.menu.AppendAction("Test", ToolbarMenuAction, GetStatus, new AbilityMenuData(null));
		_toolbarMenu.menu.AppendAction("EmptyAbility", ToolbarMenuAction, GetStatus, new AbilityMenuData(typeof(EmptyAbility)));
		_toolbarMenu.menu.AppendAction("DerivedAbility", ToolbarMenuAction, GetStatus, new AbilityMenuData(typeof(DerivedAbility)));
		_toolbarMenu.menu.AppendAction("DerivedAbility2", ToolbarMenuAction, GetStatus, new AbilityMenuData(typeof(DerivedAbility2)));
	}


	private void ToolbarMenuAction(DropdownMenuAction act)
	{
		if (_property == null)
		{
			Debug.LogError($"Ability Property was null");
			return;
		}
		
		if (!(act.userData is AbilityMenuData data))
		{
			Debug.LogError("ContextualMenu Action was not given valid AbilityMenuData");
			return;
		}

		IAbility oldAbility;

		if (_property.propertyPath.Contains("."))
		{
			string[] pathArray = _property.propertyPath.Split('.');

			string listPath = pathArray[0];

			string indexString = pathArray[pathArray.Length - 1].Trim('d', 'a', 't', 'a', '[', ']');

			int.TryParse(indexString, out int index);

			FieldInfo fieldInfo = _property.serializedObject.targetObject.GetType().GetField(
				listPath, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			IList list = fieldInfo.GetValue(_property.serializedObject.targetObject) as IList;

			oldAbility = list[index] as IAbility;
		}
		else
		{
			FieldInfo fieldInfo = _property.serializedObject.targetObject.GetType().GetField(
				ABILITY_PROPNAME, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			oldAbility = fieldInfo.GetValue(_property.serializedObject.targetObject) as IAbility;
		}


		IAbility newAbility = null;

		if (data.AbilityType != null)
		{
			newAbility = Activator.CreateInstance(data.AbilityType, oldAbility) as IAbility;
		}
		
		//if (oldAbility == null)
		//{
		//	// Create new ability
		//	newAbility = Activator.CreateInstance(data.AbilityType) as IAbility;
		//}
		//else
		//{
		//	// Copy from old ability
		//	newAbility = Activator.CreateInstance(data.AbilityType, oldAbility) as IAbility;

		//}

		// using fieldInfo may be the more general approach if making extension methods to get and set fields
		// This may be required for nested classes, lists, arrays etc.
		//fieldInfo.SetValue(_property.serializedObject.targetObject, newAbility);

		// This is essential if setting the value through field info rather with managedReferenceValue
		// ApplyModifiedProperties must be called instead in that case
		//_property.serializedObject.Update();


		// This can work but it is then required to call ApplyModifiedProperties instead of Update()
		_property.managedReferenceValue = newAbility;

		// Only needed if the setting changing the property without using fieldInfo
		_property.serializedObject.ApplyModifiedProperties();

		
		// Either way the PropertyField UIElement will not rebind to the new reference value automatically

		// It seems SerializeReference is still not a first class citizen in unity
		// The PropertyField needs to be rebound after changing the reference which
		// being a PropertyDrawer with its own uxml means the parent of the root is needed
		// Should be safe considering you can't embed UIelements inside an IMGUI inspector
		// in later versions the default editor is actually a UI Toolkit inspector
		// but in this version it will work as long as I am using a UI Toolkit inspector on any
		// object that holds an ability or it will revert back to the default property drawer
		if (_root.parent is PropertyField propfield)
		{
			propfield.BindProperty(_property);
		}
		else
		{
			Debug.LogError("Failed to find PropertyField on parent VisualElement");
		}

		return;
	}


	private DropdownMenuAction.Status GetStatus(DropdownMenuAction act) { return DropdownMenuAction.Status.Normal; }


	private class AbilityMenuData
	{
		private Type _abilityType;
		public Type AbilityType => _abilityType;


		public AbilityMenuData(Type type)
		{
			_abilityType = type;
		}
	}
}


[CustomPropertyDrawer(typeof(EmptyAbility))]
public class EmptyAbilityDrawer : AbilityDrawer { }

[CustomPropertyDrawer(typeof(DerivedAbility))]
public class DerivedAbilityDrawer : AbilityDrawer { }

[CustomPropertyDrawer(typeof(DerivedAbility2))]
public class DerivedAbility2Drawer : AbilityDrawer { }