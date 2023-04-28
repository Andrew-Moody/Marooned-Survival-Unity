using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/DestructableFactory")]
public class DestructibleFactory : ScriptableObject
{
	[SerializeField]
	private List<DestructibleSO> _destructibleSOs;

	private Dictionary<int, DestructibleSO> _destructibleSODict;


	private void InitializeSODict()
	{
		_destructibleSODict = new Dictionary<int, DestructibleSO>();

		for (int i = 0; i < _destructibleSOs.Count; i++)
		{
			_destructibleSODict.Add(_destructibleSOs[i].ID, _destructibleSOs[i]);
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
}
