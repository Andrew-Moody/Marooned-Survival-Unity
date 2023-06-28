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
		
		_property.serializedObject.Update();

		AbilityMenuData data = act.userData as AbilityMenuData;

		if (data == null)
		{
			Debug.LogError("ContextualMenu Action was not given AbilityMenuData");
			return;
		}

		if (data.AbilityType == null)
		{
			_property.managedReferenceValue = null;
			_property.serializedObject.ApplyModifiedProperties();
			ForceDraw();
			return;
		}

		FieldInfo fieldInfo = _property.serializedObject.targetObject.GetType().GetField(
			ABILITY_PROPNAME, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

		if (fieldInfo == null)
		{
			Debug.LogError($"Failed to get field for {ABILITY_PROPNAME} on {_property.serializedObject.targetObject.name}");
			return;
		}

		IAbility ability = fieldInfo.GetValue(_property.serializedObject.targetObject) as IAbility;

		if (ability == null)
		{
			IAbility newAbility = Activator.CreateInstance(data.AbilityType) as IAbility;
			_property.managedReferenceValue = newAbility;
		}
		else
		{
			IAbility newAbility = Activator.CreateInstance(data.AbilityType, ability) as IAbility;
			_property.managedReferenceValue = newAbility;
		}


		_property.serializedObject.ApplyModifiedProperties();

		ForceDraw();

		return;
	}


	private void ForceDraw()
	{
		_root.Clear();
		_visualTreeAsset.CloneTree(_root);

		SetupToolbar();

		// Doesn't work for properties ie not the target object
		//_root.BindProperty(_property.serializedObject); // Must rebind if changing elements outside of CreateInspectorGUI

		PropertyField ability = _root.Q<PropertyField>("ability");

		// not that either
		//ability.Bind(_property.serializedObject);

		ability.Unbind();
		ability.Clear();
		ability.BindProperty(_property); // works but have to unbind the old property
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