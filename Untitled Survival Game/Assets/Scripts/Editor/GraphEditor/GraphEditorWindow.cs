using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class GraphEditorWindow : EditorWindow
{
	private GraphEditableView _editorView;
	private InspectorView _inspectorView;

	[MenuItem("Window/MoodyMakes/GraphEditor")]
	public static void OpenGraphEditor()
	{
		GraphEditorWindow graphEditor = GetWindow<GraphEditorWindow>();

		Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Textures/GraphEditorIcon.png");

		graphEditor.titleContent = new GUIContent("GraphEditor", icon);
	}

	public void CreateGUI()
	{
		VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIToolkit/GraphEditor.uxml");
		visualTreeAsset.CloneTree(rootVisualElement);
		
		StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UIToolkit/GraphEditor.uss");
		rootVisualElement.styleSheets.Add(styleSheet);

		_editorView = rootVisualElement.Q<GraphEditableView>();
		_inspectorView = rootVisualElement.Q<InspectorView>();

		_editorView.AssetViewSelectedEvent += _inspectorView.UpdateSelection;

		PopulateSelection();
	}


	private void OnSelectionChange()
	{
		PopulateSelection();
	}


	private void PopulateSelection()
	{
		if (_editorView != null && Selection.activeObject is GraphEditable editable)
		{
			_editorView.PopulateView(editable);
		}
	}
}