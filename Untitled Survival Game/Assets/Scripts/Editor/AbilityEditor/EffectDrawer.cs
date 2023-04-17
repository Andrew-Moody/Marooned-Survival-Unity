using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Effect))]
public class EffectDrawer : PropertyDrawer
{
	private const float PADDING = 2f;

	private const float SPACE = 10f;


	SerializedProperty _effectType;

	SerializedProperty _effectTiming;

	SerializedProperty _effectTarget;

	SerializedProperty _animTrigger;

	SerializedProperty _animation;

	SerializedProperty _sound;

	SerializedProperty _particleName;

	//SerializedProperty _particleSystem;

	SerializedProperty _statType;

	SerializedProperty _statAmount;

	SerializedProperty _knockBack;


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


		//if (property.isExpanded)
		//{

		//	//do
		//	//{
		//	//	height += EditorGUI.GetPropertyHeight(iterator) + PADDING;
		//	//}
		//	//while (iterator.Next(false));


			

		//	//height += EditorGUI.GetPropertyHeight(_effectTiming) + PADDING;

		//	//if ((EffectType)_effectType.intValue == EffectType.Animation)
		//	//{
		//	//	//height += EditorGUI.GetPropertyHeight(_animTrigger) + PADDING;
		//	//	//height += EditorGUI.GetPropertyHeight(_animation) + PADDING;
		//	//}
		//	//else if ((EffectType)_effectType.intValue == EffectType.ParticleEffect)
		//	//{
		//	//	height += EditorGUI.GetPropertyHeight(_particleName) + PADDING;
		//	//}
		//	//else if ((EffectType)_effectType.intValue == EffectType.SoundEffect)
		//	//{
		//	//	height += EditorGUI.GetPropertyHeight(_sound) + PADDING;
		//	//}
		//	//else if ((EffectType)_effectType.intValue == EffectType.StatEffect)
		//	//{
		//	//	height += EditorGUI.GetPropertyHeight(_statType) + PADDING;
		//	//	height += EditorGUI.GetPropertyHeight(_statAmount) + PADDING;
		//	//}
		//	//else if ((EffectType)_effectType.intValue == EffectType.KnockBack)
		//	//{
		//	//	height += EditorGUI.GetPropertyHeight(_knockBack) + PADDING;
		//	//}

		//	height += SPACE;
		//}
		//else
		//{
		//	//height = EditorGUI.GetPropertyHeight(_effectType) + PADDING;
		//}

		

		return height;
	}


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//GetSerializedProperties(property);

		EditorGUI.BeginProperty(position, label, property); // Supposedly these are important but not sure why

		int indentCache = EditorGUI.indentLevel;

		SerializedProperty copy = property.Copy();


		//if (!copy.hasVisibleChildren)
		//{
		//	if (copy.hasChildren)
		//	{
		//		Debug.LogWarning("Effect has no visible children");
		//	}
		//	else
		//	{
		//		Debug.LogWarning("Effect has no children");
		//	}
			

		//	Debug.Log(copy.name + " at " + copy.propertyPath + " IsExpanded: " + copy.isExpanded);
		//	return;
		//}

		Rect propRect = new Rect(position.x, position.y, position.width, 18f);

		IEnumerator enumerator = copy.GetEnumerator();

		enumerator.MoveNext(); // Have to do this first or current will be null

		// This should be EffectName otherwise will have to get fancy
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


		// Draw a Name Label that is also a foldout
		//float foldOutHeight = EditorGUI.GetPropertyHeight(_effectType);
		//Rect foldOutRect = new Rect(position.x, position.y, position.width, foldOutHeight);
		//property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, "", false);


		
		//float typeWidth = position.width / 3;
		//Rect typeRect = new Rect(position.x, position.y, typeWidth, foldOutHeight);
		//EditorGUI.PropertyField(typeRect, _effectType, new GUIContent("Type"));

		//float timingWidth = position.width / 3;
		//Rect timingRect = new Rect(position.x + typeWidth, position.y, timingWidth, foldOutHeight);
		//EditorGUI.PropertyField(timingRect, _effectTiming, new GUIContent("Timing"));

		//float targetWidth = position.width / 3;
		//Rect targetRect = new Rect(position.x + typeWidth + timingWidth, position.y, targetWidth, foldOutHeight);
		//EditorGUI.PropertyField(targetRect, _effectTarget, new GUIContent("Target"));

		//if (property.isExpanded)
		//{
		//	//position.height = EditorGUI.GetPropertyHeight(_effectTiming);

		//	//position.y += foldOutHeight + PADDING;


			
		//	//EditorGUI.PropertyField(position, _animTrigger);
		//	//	//position.y += EditorGUI.GetPropertyHeight(_animTrigger) + PADDING;


		//	//if ((EffectType)_effectType.intValue == EffectType.Animation)
		//	//{
		//	//	//EditorGUI.PropertyField(position, _animTrigger);
		//	//	//position.y += EditorGUI.GetPropertyHeight(_animTrigger) + PADDING;

		//	//	//EditorGUI.PropertyField(position, _animation);
		//	//	//position.y += EditorGUI.GetPropertyHeight(_animation) + PADDING;
		//	//}
		//	//else if ((EffectType)_effectType.intValue == EffectType.ParticleEffect)
		//	//{
		//	//	EditorGUI.PropertyField(position, _particleName);
		//	//	position.y += EditorGUI.GetPropertyHeight(_particleName) + PADDING;
		//	//}
		//	//else if ((EffectType)_effectType.intValue == EffectType.SoundEffect)
		//	//{
		//	//	EditorGUI.PropertyField(position, _sound);
		//	//	position.y += EditorGUI.GetPropertyHeight(_sound) + PADDING;
		//	//}
		//	//else if ((EffectType)_effectType.intValue == EffectType.StatEffect)
		//	//{
		//	//	EditorGUI.PropertyField(position, _statType);
		//	//	position.y += EditorGUI.GetPropertyHeight(_statType) + PADDING;

		//	//	EditorGUI.PropertyField(position, _statAmount, true);
		//	//	position.y += EditorGUI.GetPropertyHeight(_statAmount) + PADDING;
		//	//}
		//	//else if ((EffectType)_effectType.intValue == EffectType.KnockBack)
		//	//{
		//	//	EditorGUI.PropertyField(position, _knockBack);
		//	//	position.y += EditorGUI.GetPropertyHeight(_knockBack) + PADDING;
		//	//}
		//}



		EditorGUI.indentLevel = indentCache;


		EditorGUI.EndProperty();
	}


	private void UnBoxEffect(SerializedProperty property)
	{
		Effect effect = EditorHelper.GetObjectFromProperty(property) as Effect;

		if (effect == null)
		{
			Debug.LogWarning("effect was null");
		}

		if (effect.EffectType == EffectType.Animation)
		{
			effect = new AnimationEffect();
		}




	}
}



//[CustomPropertyDrawer(typeof(EffectList))]
public class EffectListDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		float height = 20f;

		SerializedProperty effects = property.FindPropertyRelative("_effects");

		if (effects.isExpanded)
		{
			for (int i = 0; i < effects.arraySize; i++)
			{
				height += EditorGUI.GetPropertyHeight(effects.GetArrayElementAtIndex(i));
			}
		}

		height += 24f; // Add room for buttons

		return height;
	}


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property); // Supposedly these are important but not sure why

		int indentCache = EditorGUI.indentLevel;


		SerializedProperty effects = property.FindPropertyRelative("_effects");
		
		Debug.Log(property.name + " at " + property.propertyPath);
		Debug.Log(effects.name + " at " + effects.propertyPath + " Size: " + effects.arraySize);


		EditorGUI.PropertyField(position, effects, true);


		//Rect addButtonRect = new Rect(position.x, position.y, position.width, 20f);

		//if (GUI.Button(addButtonRect, "+"))
		//{

		//}



		EditorGUI.indentLevel = indentCache;


		EditorGUI.EndProperty();
	}
}