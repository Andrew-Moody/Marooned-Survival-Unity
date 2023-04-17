using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EditorHelper
{
	//   public static object GetObjectFromProperty(SerializedProperty property)
	//{
	//	object obj = null;



	//	object objOnPath = property.serializedObject.targetObject;

	//	string path = property.propertyPath;

	//	string[] propsOnPath = path.Split('.');

	//	int index = int.Parse(path.Split('[')[1].TrimEnd(']'));

	//	// This will remove Array.data[i] from the end
	//	if (propsOnPath.Length > 2 && propsOnPath[propsOnPath.Length - 2] == "Array")
	//	{
	//		propsOnPath = propsOnPath.Take(propsOnPath.Length - 2).ToArray();
	//	}

	//	// it seems you have to start at target and keep getting the next prop in the path till you get to the end
	//	// This is probably not very fast
	//	foreach (string prop in propsOnPath)
	//	{
	//		objOnPath = GetValueFromTarget(objOnPath, prop);
	//	}

	//	// objOnPath should now point to the last property unless it is in an array/list

	//	if (objOnPath is List<EditorState>)
	//	{
	//		objOnPath = ((List<EditorState>)objOnPath)[index];
	//	}






	//	return obj;
	//}


	public static object GetObjectFromProperty(SerializedProperty property)
	{
		object obj = property.serializedObject.targetObject;

		// Potential gotcha with this. Need to check if in multi-target mode otherwise targetObject points only to the first object

		if (property.serializedObject.isEditingMultipleObjects)
		{
			Debug.LogWarning("Target is in Multi Edit Mode, GetObjectFromProperty may fail");
		}

		//Debug.Log(obj.ToString() + " Path: " + property.propertyPath);

		string[] propNames = property.propertyPath.Split('.');

		foreach (string propName in propNames)
		{
			if (propName == "Array")
			{
				//Debug.Log("Skip Array");
				continue;
			}

			if (propName.Contains("data"))
			{
				char[] trimChars = { 'd', 'a', 't', '[', ']' };
				int index = int.Parse(propName.Trim(trimChars));

				IEnumerable<object> objList = obj as IEnumerable<object>;

				obj = objList.ElementAt(index);

				//Debug.Log("index: " + index + " Object: " + obj.ToString());

				continue;
			}

			obj = GetValueFromTarget(obj, propName);

			//Debug.Log("PropName: " + propName + " Object: " + obj.ToString());
		}




		return obj;
	}


	public static object GetValueFromTarget(object target, string name)
	{
		if (target == null)
			return null;


		FieldInfo fieldinfo = target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

		if (fieldinfo == null)
		{
			Debug.Log("Failed to get fieldinfo for: " + name);
			return null;
		}


		object obj = fieldinfo.GetValue(target);

		return obj;
	}
}
