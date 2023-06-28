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


		// Draws the default inspector
		// InspectorElement.FillDefaultInspector(root, serializedObject, this);
		
		return _root;
	}


	private void PopulateContextMenu(ContextualMenuPopulateEvent evt)
	{
		//evt.menu.AppendAction("Test", HandleContextAction);
		//evt.menu.AppendAction("DerivedAbility", HandleContextAction);
		//evt.menu.AppendAction("DerivedAbility2", HandleContextAction);

		evt.menu.AppendAction("Test", HandleContextAction, GetStatus, new AbilityMenuData(null, 0));
		evt.menu.AppendAction("EmptyAbility", HandleContextAction, GetStatus, new AbilityMenuData(typeof(EmptyAbility), 1));
		evt.menu.AppendAction("DerivedAbility", HandleContextAction, GetStatus, new AbilityMenuData(typeof(DerivedAbility), 2));
		evt.menu.AppendAction("DerivedAbility2", HandleContextAction, GetStatus, new AbilityMenuData(typeof(DerivedAbility2), 3));
	}


	private DropdownMenuAction.Status GetStatus(DropdownMenuAction act) { return DropdownMenuAction.Status.Normal; }


	private void HandleContextAction(DropdownMenuAction act)
	{
		serializedObject.Update();

		AbilityMenuData data = act.userData as AbilityMenuData;

		if (data == null)
		{
			Debug.LogError("ContextualMenu Action was not given AbilityMenuData");
			return;
		}

		SerializedProperty prop = serializedObject.FindProperty(ABILITY_PROPNAME);

		if (prop == null)
		{
			Debug.LogError($"Failed to find property for {ABILITY_PROPNAME} on {target.name}");
			return;
		}


		SerializedProperty testInt = serializedObject.FindProperty("_test");
		testInt.intValue = data.TestInt;


		if (data.AbilityType == null)
		{
			prop.managedReferenceValue = null;
			serializedObject.ApplyModifiedProperties();
			ForceDraw();
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

		ForceDraw();

		return;
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

		private int _testInt;
		public int TestInt => _testInt;

		public AbilityMenuData(Type type, int testInt)
		{
			_abilityType = type;
			_testInt = testInt;
		}
	}
}
