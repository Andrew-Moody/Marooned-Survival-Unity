using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DestructibleSO")]
public class DestructibleSO : ScriptableObject
{
	public string Name => _name;
	[SerializeField] private string _name;
	

	public int ID => _id;
	[SerializeField] private int _id;
	

	public DestructibleObject BasePrefab => _basePrefab;
	[SerializeField] private DestructibleObject _basePrefab;
	

	public GameObject GraphicPrefab => _graphicPrefab;
	[SerializeField] private GameObject _graphicPrefab;
	

	public Mesh Mesh => _mesh;
	[SerializeField] private Mesh _mesh;
	

	public Material Material => _material;
	[SerializeField] private Material _material;
	

	public int ItemID => _itemID;
	[SerializeField] private int _itemID;
}
