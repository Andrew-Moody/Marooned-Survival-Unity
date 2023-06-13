using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MobSO")]
public class MobSO : ScriptableObject
{
	[SerializeField]
	private string _name;
	public string Name => _name;

	[SerializeField]
	private int _id;
	public int ID => _id;

	[SerializeField]
	private Mob _baseMobPrefab;
	public Mob BaseMobPrefab => _baseMobPrefab;


	[SerializeField]
	private GameObject _graphicVariantPrefab;
	public GameObject GraphicVariantPrefab => _graphicVariantPrefab;

	[SerializeField]
	private List<StatValue> _stats;
	public List<StatValue> Stats => _stats;

	[SerializeField]
	private List<AbilitySO> _abilities;
	public List<AbilitySO> Abilities => _abilities;

	[SerializeField]
	private MobAISO _mobAISO;
	public MobAISO MobAISO => _mobAISO;


	[SerializeField]
	private int _itemToDrop;
	public int ItemToDrop => _itemToDrop;


	public GameObject InstantiatePrefab(Transform parent)
	{
		GameObject mob = Instantiate(_graphicVariantPrefab, parent, false);

		return mob;
	}


#if UNITY_EDITOR
	// This could be used to open a custom editor window when double clicking the asset
	// But cant be left in the build as it is editor only
	[UnityEditor.Callbacks.OnOpenAssetAttribute]
	public static bool Test(int instanceID, int line)
	{
		Debug.Log("Test " + instanceID + " " + line);

		return false;
	}
#endif
}
