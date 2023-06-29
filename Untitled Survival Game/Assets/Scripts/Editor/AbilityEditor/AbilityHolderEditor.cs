using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(AbilityHolder))]
public class AbilityHolderEditor : Editor
{
	private const string ABILITY_PROPNAME = "_ability";

	private VisualTreeAsset _visualTreeAsset;

	private VisualElement _root;

	private VisualElement _toolbar;

	private ListView _listView;

	private SerializedProperty _abilitiesProp;

	private List<IAbility> _abilities;


	public override VisualElement CreateInspectorGUI()
	{
		Debug.Log("CreateInspectorGUI");

		_root = new VisualElement();

		_visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolkit/AbilityHolderEditor.uxml");
		_visualTreeAsset.CloneTree(_root);


		_toolbar = _root.Q<Toolbar>("ability-toolbar");

		_toolbar.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
		{
			PopulateContextMenu(evt);
		}));


		_abilitiesProp = serializedObject.FindProperty("_abilities");

		FieldInfo fieldInfo = target.GetType().GetField("_abilities", BindingFlags.NonPublic | BindingFlags.Instance);

		_abilities = fieldInfo.GetValue(target) as List<IAbility>;



		_listView = _root.Q<ListView>("abilities");

		_listView.makeItem = MakeItem;
		_listView.bindItem = BindItem;
		_listView.itemsSource = _abilities;

		// Draws the default inspector
		//InspectorElement.FillDefaultInspector(_root, serializedObject, this);
		
		return _root;
	}


	private VisualElement MakeItem()
	{
		return new PropertyField();
	}


	private void BindItem(VisualElement element, int index)
	{
		if (index < _abilities.Count)
		{
			SerializedProperty prop = serializedObject.FindProperty("_abilities");
			prop.GetArrayElementAtIndex(index);

			(element as PropertyField).BindProperty(prop.GetArrayElementAtIndex(index));
		}
	}

	private void PopulateContextMenu(ContextualMenuPopulateEvent evt)
	{
		//evt.menu.AppendAction("Test", HandleContextAction);
		//evt.menu.AppendAction("DerivedAbility", HandleContextAction);
		//evt.menu.AppendAction("DerivedAbility2", HandleContextAction);

		evt.menu.AppendAction("Test", ChangeAbilityType, GetStatus, new AbilityMenuData(null, 0));
		evt.menu.AppendAction("EmptyAbility", AddAbility, GetStatus, new AbilityMenuData(typeof(EmptyAbility), 1));
		evt.menu.AppendAction("DerivedAbility", AddAbility, GetStatus, new AbilityMenuData(typeof(DerivedAbility), 2));
		evt.menu.AppendAction("DerivedAbility2", AddAbility, GetStatus, new AbilityMenuData(typeof(DerivedAbility2), 3));
	}


	private DropdownMenuAction.Status GetStatus(DropdownMenuAction act) { return DropdownMenuAction.Status.Normal; }


	private void ChangeAbilityType(DropdownMenuAction act)
	{
		

		if (!(act.userData is AbilityMenuData data) || data.AbilityType == null)
		{
			Debug.LogError("ContextualMenu Action was not given valid AbilityMenuData");
			return;
		}

		SerializedProperty prop = serializedObject.FindProperty(ABILITY_PROPNAME);

		if (prop == null)
		{
			Debug.LogError($"Failed to find property for {ABILITY_PROPNAME} on {target.name}");
			return;
		}

		FieldInfo fieldInfo = target.GetType().GetField(ABILITY_PROPNAME, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

		if (fieldInfo == null)
		{
			Debug.LogError($"Failed to get field for {ABILITY_PROPNAME} on {target.name}");
			return;
		}

		IAbility ability = fieldInfo.GetValue(target) as IAbility;

		if (ability == null)
		{
			IAbility newAbility = Activator.CreateInstance(data.AbilityType) as IAbility;
			prop.managedReferenceValue = newAbility;
		}
		else
		{
			IAbility newAbility = Activator.CreateInstance(data.AbilityType, ability) as IAbility;
			prop.managedReferenceValue = newAbility;
		}


		serializedObject.ApplyModifiedProperties();
		serializedObject.Update();

		ForceDraw();
	}


	private void AddAbility(DropdownMenuAction act)
	{
		if (!(act.userData is AbilityMenuData data) || data.AbilityType == null)
		{
			Debug.LogError("ContextualMenu Action was not given valid AbilityMenuData");
			return;
		}

		(target as AbilityHolder).AddAbility(data.AbilityType);
	}


	private void ForceDraw()
	{
		//serializedObject.Update();

		// I tried several ways to get the editor to repaint after changing the values but most info did not apply to visual elements or just didnt work
		// EditorUtility.SetDirty on the target, serializedObject etc
		// MarkDirtyRepaint
		// AssetDatabase.SaveAssets (maybe was smart enough to know there werent any changes to apply)

		// Even manually redrawing the whole thing seems to not work

		// Finally! turns out clearing the visual element also clears any data binding
		// CreateInspectorGUI does the binding for you but anywhere else you have to handle yourself

		_root.Clear();
		_visualTreeAsset.CloneTree(_root);

		_toolbar = _root.Q<Toolbar>("ability-toolbar");

		_toolbar.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
		{
			PopulateContextMenu(evt);
		}));

		_root.Bind(serializedObject); // Must rebind if changing elements outside of CreateInspectorGUI
	}


	private class AbilityMenuData
	{
		private Type _abilityType;
		public Type AbilityType => _abilityType;

		public AbilityMenuData(Type type, int testInt)
		{
			_abilityType = type;
		}
	}
}
