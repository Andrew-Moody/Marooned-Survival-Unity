using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/DestructableFactory")]
public class DestructibleFactory : ScriptableObject
{
	[SerializeField]
	private List<DestructibleSO> _destructibleSOs;

	private Dictionary<int, DestructibleSO> _destructibleSODict;


	[SerializeField]
	private GameObject[] _destructiblePrefabs;

	private Dictionary<int, DestructibleObject> _destructiblePrefabDict;

	private void InitializeSODict()
	{
		_destructibleSODict = new Dictionary<int, DestructibleSO>();

		for (int i = 0; i < _destructibleSOs.Count; i++)
		{
			_destructibleSODict.Add(_destructibleSOs[i].ID, _destructibleSOs[i]);
		}

		_destructiblePrefabDict = new Dictionary<int, DestructibleObject>();

		foreach (GameObject pf in _destructiblePrefabs)
		{
			if (pf.TryGetComponent(out DestructibleObject dObjPF))
			{
				_destructiblePrefabDict.Add(dObjPF.DestructibleSO.ID, dObjPF);
			}
		}
	}

	public DestructibleSO GetDestructible(int destructibleID)
	{
		if (_destructibleSODict == null)
		{
			InitializeSODict();
		}

		_destructibleSODict.TryGetValue(destructibleID, out DestructibleSO destructibleSO);

		return destructibleSO;
	}


	public DestructibleObject GetPrefab(int destructibleID)
	{
		if (_destructiblePrefabDict == null)
		{
			InitializeSODict();
		}

		_destructiblePrefabDict.TryGetValue(destructibleID, out DestructibleObject prefab);

		return prefab;
	}

	private void OnValidate()
	{
		for (int i = 0; i < _destructiblePrefabs.Length; i++)
		{
			if (_destructiblePrefabs[i] != null && _destructiblePrefabs[i].GetComponent<DestructibleObject>() == null)
			{
				_destructiblePrefabs[i] = null;

				Debug.LogWarning("Mob prefab must have a DestructibleObject component");
			}
		}
	}
}
