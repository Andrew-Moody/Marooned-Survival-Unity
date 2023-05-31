using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
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

	public List<AbilitySO> _abilities;

	[SerializeField]
	private MobAISO _mobAISO;
	public MobAISO MobAISO => _mobAISO;


	public GameObject InstantiatePrefab(Transform parent)
	{
		GameObject obj = Instantiate(_mobPrefab, parent, false);

		return obj;
	}


	[OnOpenAssetAttribute]
	public static bool Test(int instanceID, int line)
	{
		Debug.Log("Test " + instanceID + " " + line);

		return false;
	}
}
