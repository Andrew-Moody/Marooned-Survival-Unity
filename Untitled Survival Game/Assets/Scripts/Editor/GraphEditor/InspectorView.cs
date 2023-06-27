using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
	new public class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

	private Editor _editor;

	public InspectorView()
	{

	}


	public void UpdateSelection(GraphAssetView assetView)
	{
		Clear();

		UnityEngine.Object.DestroyImmediate(_editor);

		if (assetView != null)
		{
			_editor = Editor.CreateEditor(assetView.Asset);

			IMGUIContainer container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });

			Add(container);
		}
	}
}
