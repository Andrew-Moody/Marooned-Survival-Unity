using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DestructibleSO")]
public class DestructibleSO : ScriptableObject
{
	[SerializeField]
	private string _name;
	public string Name => _name;

	[SerializeField]
	private int _id;
	public int ID => _id;

	[SerializeField]
	private DestructibleObject _basePrefab;
	public DestructibleObject BasePrefab => _basePrefab;

	[SerializeField]
	private GameObject _graphicPrefab;
	public GameObject GraphicPrefab => _graphicPrefab;

	[SerializeField]
	private Mesh _mesh;
	public Mesh Mesh => _mesh;

	[SerializeField]
	private Material _material;
	public Material Material => _material;


	[SerializeField]
	private int _itemID;
	public int ItemID => _itemID;


	[SerializeField]
	private float _deathTime;
	public float DeathTime => _deathTime;

	[SerializeField]
	private ToolType _toolType;
	public ToolType ToolType => _toolType;

	[SerializeField] private float _toolPower;
	public float ToolPower => _toolPower;

	[SerializeField]
	private ParticleEffectData[] _particleEffects;
	public ParticleEffectData[] ParticleEffects => _particleEffects;

	[SerializeField]
	private AbilitySystem.Cue[] _cueOverrides;
	public AbilitySystem.Cue[] CueOverrides => _cueOverrides;
}
