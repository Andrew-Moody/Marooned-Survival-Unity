using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class GraphEditableView : GraphView
{
	new public class UxmlFactory : UxmlFactory<GraphEditableView, GraphView.UxmlTraits> { }

	// Same as above but possible to add Traits
	//new public class UxmlFactory : UxmlFactory<GraphEditorView, UxmlTraits> { }
	//new public class UxmlTraits : GraphView.UxmlTraits { }

	public event System.Action<GraphAssetView> AssetViewSelectedEvent;

	private GraphEditable _target;


	public GraphEditableView()
	{
		VisualElement grid = new GridBackground();
		grid.name = "GridBackground";
		Insert(0, grid);

		this.AddManipulator(new ContentZoomer());
		this.AddManipulator(new ContentDragger());
		this.AddManipulator(new SelectionDragger());
		this.AddManipulator(new RectangleSelector());

		styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UIToolkit/GraphEditor.uss"));
	}


	public void PopulateView(GraphEditable editable)
	{
		_target = editable;

		graphViewChanged -= OnGraphViewChanged;

		// Clear data from previous selection
		DeleteElements(graphElements);

		graphViewChanged += OnGraphViewChanged;

		_target.Assets.ForEach(asset => CreateAssetView(asset));

		OnAssetViewSelected(null); // Clear the inspector for now when selecting a different editable
	}


	public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
	{
		//base.BuildContextualMenu(evt);

		// This will not include the base type so need to add manually if desired
		TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<GraphSubAsset>();

		evt.menu.AppendAction("[GraphSubAsset] GraphSubAsset", (dropdownMenuAction) => CreateAsset(typeof(GraphSubAsset)));

		foreach (System.Type type in types)
		{
			string menuLabel = $"[{type.BaseType.Name}] {type.Name}";

			evt.menu.AppendAction(menuLabel, (dropdownMenuAction) => CreateAsset(type));
		}
	}


	private void CreateAssetView(GraphSubAsset asset)
	{
		GraphAssetView assetView = new GraphAssetView(asset);
		assetView.AssetViewSelectedEvent += OnAssetViewSelected;
		AddElement(assetView);
	}


	private void CreateAsset(System.Type type)
	{
		if (_target == null)
		{
			Debug.LogError("Target was null");
			return;
		}

		GraphSubAsset asset = _target.CreateSubAsset(type);
		CreateAssetView(asset);
	}


	private GraphViewChange OnGraphViewChanged(GraphViewChange change)
	{
		if (change.elementsToRemove != null)
		{
			change.elementsToRemove.ForEach(elem =>
			{
				if (elem is GraphAssetView view)
				{
					view.AssetViewSelectedEvent -= OnAssetViewSelected;
					_target.DeleteSubAsset(view.Asset);
				}
			});
		}

		return change;
	}


	private void OnAssetViewSelected(GraphAssetView assetView)
	{
		AssetViewSelectedEvent?.Invoke(assetView);
	}
}
