using UnityEngine.UIElements;

/// <summary>
/// Adds TwoPaneSplitView to UIBuilder
/// </summary>
public class SplitView : TwoPaneSplitView
{
	new public class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }
}
