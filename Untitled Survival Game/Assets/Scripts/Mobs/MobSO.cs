using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MobSO")]
public class MobSO : ScriptableObject
{
	[SerializeField]
	private string Name;

	public int ID;

	[SerializeField]
	private GameObject _mobPrefab;


	public List<StatValue> _stats;


	public GameObject InstantiatePrefab(Transform parent)
	{
		GameObject obj = Instantiate(_mobPrefab, parent, false);

		return obj;
	}
}
