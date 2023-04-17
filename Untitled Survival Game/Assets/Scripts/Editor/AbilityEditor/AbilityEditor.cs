using UnityEditor;


[CustomEditor(typeof(Ability))]
public class AbilityEditor : Editor
{
	#region SerializedProperty

	private SerializedProperty _abilityName;

	private SerializedProperty _abilityType;

	private SerializedProperty _coolDown;

	private SerializedProperty _requirements;

	private SerializedProperty _namedValues;

	private SerializedProperty _effects;

	#endregion


	public void OnEnable()
	{
		_abilityName = serializedObject.FindProperty("_abilityName");

		_abilityType = serializedObject.FindProperty("_abilityType");

		_coolDown = serializedObject.FindProperty("_coolDown");

		_requirements = serializedObject.FindProperty("_requirements");

		_namedValues = serializedObject.FindProperty("_namedValues");

		_effects = serializedObject.FindProperty("_effects");
	}


	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		// Well shit. target refers to the unity object currently being inspected not neccessarily the object it is an editor for
		// Everyone is saying to use property drawer instead of editor for non unity classes

		// In this case the target could be anything that has an ability as a serialized property (AbilitySO, AbilityItemSO)
		// Makes it tricky to use the reference as you need different logic for each different ability owner
		// Ability ability = (Ability)target; // This does not work because Ability does not derive from Unity.object

		// It gets worse. it seems like editor only ever runs if the object is actually selected
		// It falls back to the default inspector if the object is simply a property of the selected object

		// Meaning I basically can use an editor to display a class that isnt a scriptableObject or monobehaviour

		EditorGUILayout.PropertyField(_abilityName);
		EditorGUILayout.PropertyField(_abilityType);
		EditorGUILayout.PropertyField(_coolDown);
		EditorGUILayout.PropertyField(_requirements);
		EditorGUILayout.PropertyField(_namedValues);
		EditorGUILayout.PropertyField(_effects);

		serializedObject.ApplyModifiedProperties();
	}
}
