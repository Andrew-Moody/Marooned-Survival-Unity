using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/DestructableFactory")]
public class DestructibleFactory : ScriptableObject
{
    [SerializeField]
    private List<DestructibleObject> _prefabs;


    private Dictionary<int, DestructibleObject> _prefabDict;


	private void OnValidate()
	{
		InitializePrefabDict();
	}


	public DestructibleObject GetPrefab(int destructibleID)
	{
		if (_prefabDict == null)
		{
			InitializePrefabDict();
		}

		if (!_prefabDict.TryGetValue(destructibleID, out DestructibleObject destructible))
		{
			Debug.LogWarning("Destructible factory does not have definition for ID: " + destructibleID);
		}

		return destructible;
	}


	private void InitializePrefabDict()
	{
		_prefabDict = new Dictionary<int, DestructibleObject>();

		for (int i = 0; i < _prefabs.Count; i++)
		{
			_prefabDict.Add(i, _prefabs[i]);
		}
	}
}
