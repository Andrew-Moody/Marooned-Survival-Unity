using UnityEngine;
using UnityEngine.UIElements;

public class GraphAssetView : UnityEditor.Experimental.GraphView.Node
{
	//new public class UxmlFactory : UxmlFactory<GraphAssetView, UnityEditor.Experimental.GraphView.Node.UxmlTraits> { }

	public event System.Action<GraphAssetView> AssetViewSelectedEvent;

	private GraphSubAsset _asset;
	public GraphSubAsset Asset => _asset;

	public GraphAssetView(GraphSubAsset asset)
	{
		_asset = asset;
		title = asset.name;
		viewDataKey = asset.guid;

		style.left = asset.Position.x;
		style.top = asset.Position.y;
	}


	public override void SetPosition(Rect newPos)
	{
		base.SetPosition(newPos);
		_asset.Position.x = newPos.xMin;
		_asset.Position.y = newPos.yMin;
	}


	public override void OnSelected()
	{
		base.OnSelected();

		AssetViewSelectedEvent?.Invoke(this);
	}
}
