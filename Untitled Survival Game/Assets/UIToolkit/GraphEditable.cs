using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "GraphEditor/GraphEditable")]
public class GraphEditable : ScriptableObject
{
	[SerializeField]
	private List<GraphSubAsset> _subAssets = new List<GraphSubAsset>() ;
	public List<GraphSubAsset> Assets => _subAssets;

	public GraphSubAsset CreateSubAsset(System.Type type)
	{
		GraphSubAsset asset = ScriptableObject.CreateInstance(type) as GraphSubAsset;
		asset.name = type.Name;
		asset.guid = GUID.Generate().ToString();
		_subAssets.Add(asset);

		AssetDatabase.AddObjectToAsset(asset, this);
		AssetDatabase.SaveAssets();

		return asset;
	}

	public void DeleteSubAsset(GraphSubAsset asset)
	{
		_subAssets.Remove(asset);

		AssetDatabase.RemoveObjectFromAsset(asset);
		AssetDatabase.SaveAssets();
	}
}
