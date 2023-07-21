using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actors;

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

	public List<StatInitialValue> InitialStats => _stats;
	[SerializeField] private List<StatInitialValue> _stats;

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
}
