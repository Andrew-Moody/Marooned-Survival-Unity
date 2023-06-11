using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	[SerializeField]
	private Canvas _worldCanvas;
	public Canvas WorldCanvas => _worldCanvas;


	[SerializeField]
	private List<UIPanel> _panels;

	private Dictionary<string, UIPanel> _panelDict;

	private Stack<UIPanel> _panelStack = new Stack<UIPanel>();


	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			InitializeUI();

			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(gameObject);
		}
	}


	private void InitializeUI()
	{
		_panelDict = new Dictionary<string, UIPanel>();

		for (int i = 0; i < _panels.Count; i++)
		{
			_panels[i].Initialize();

			AddPanel(_panels[i]);
		}
	}


	public static void SetPlayer(GameObject player)
	{
		Debug.LogError("UIManager SetPlayer");

		for (int i = 0; i < Instance._panels.Count; i++)
		{
			Instance._panels[i].SetPlayer(player);
		}
	}


	public static void AddPanel(UIPanel panel)
	{
		if (Instance._panelDict.ContainsKey(panel.PanelName))
		{
			Debug.LogError($"Failed to add UIPanel {panel.name}: A UIPanel already exists with the PanelName {panel.PanelName}");
		}
		else
		{
			Instance._panelDict[panel.PanelName] = panel;
		}
	}


	public static void ShowPanel(string panelName, UIPanelData data = null, bool pushToStack = false)
	{
		if (Instance._panelDict.TryGetValue(panelName, out UIPanel panel))
		{
			panel.Show(data);

			if (pushToStack)
			{
				HideStackTop(false);

				Instance._panelStack.Push(panel);
			}
		}
		else
		{
			Debug.LogError($"Failed to Show UIPanel: A UIPanel with name {panelName} was not found");
		}
	}


	public static void HidePanel(string panelName)
	{
		if (Instance._panelDict.TryGetValue(panelName, out UIPanel panel))
		{
			panel.Hide();
		}
		else
		{
			Debug.LogError($"Failed to Hide UIPanel: A UIPanel with name {panelName} was not found");
		}
	}


	public static void HideStackTop(bool popFromStack)
	{
		if (Instance._panelStack.Count == 0)
		{
			return;
		}

		if (popFromStack)
		{
			Instance._panelStack.Pop().Hide();

			if (Instance._panelStack.Count > 0)
			{
				Instance._panelStack.Peek().Show(null);
			}
		}
		else
		{
			Instance._panelStack.Peek().Hide();
		}
	}


	/// <summary>
	/// Returns true if the name of the panel on the top of the stack is equal to panelName 
	/// </summary>
	/// <param name="panelName"></param>
	/// <returns></returns>
	public static bool CheckStackTop(string panelName)
	{
		if (Instance._panelStack.Count > 0)
		{
			return Instance._panelStack.Peek().PanelName == panelName;
		}

		return false;
	}


	public static void HideAll()
	{
		for (int i = 0; i < Instance._panels.Count; i++)
		{
			Instance._panels[i].Hide();
		}

		Instance._panelStack.Clear();
	}
}
