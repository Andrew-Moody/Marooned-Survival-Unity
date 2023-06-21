using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoinOptionsUI : UIPanel
{
	[SerializeField]
	private ServerEntryUI _entryPF;

	[SerializeField]
	private Transform _entryHolder;

	[SerializeField]
	private GameObject _editWindow;

	[SerializeField]
	private TMP_InputField _editName;

	[SerializeField]
	private TMP_InputField _editAddress;

	private List<ServerEntryUI> _entries = new List<ServerEntryUI>();

	private ServerEntryUI _selected;

	private ServerEntryUI _entryToEdit;

	private const string SAVE_PATH = "ServerList.JSON";


	public override void Show(UIPanelData data)
	{
		base.Show(data);

		LoadServerList();

		if (_selected != null)
		{
			_selected.SetSelected(false);
		}

		if (_entries != null && _entries.Count > 0)
		{
			_selected = _entries[0];

			_selected.SetSelected(true);
		}
	}


	public void OnAddServerPressed()
	{
		_editWindow.SetActive(true);

		_editName.text = "";

		_editAddress.text = "";

		_entryToEdit = null;
	}


	public void OnEditServerPressed()
	{
		if (_selected == null)
		{
			return;
		}

		_entryToEdit = _selected;

		_editWindow.SetActive(true);

		_editName.text = _selected.ServerName;

		_editAddress.text = _selected.ServerAddress;
	}


	public void OnRemoveServerPressed()
	{
		if (_selected == null)
		{
			return;
		}

		ConfirmationUIData data = new ConfirmationUIData
		{
			Message = "Are you sure you want to delete this entry?",

			OnCancelPressed = new System.Action(OnCancelRemove),

			OnConfirmPressed = new System.Action(OnConfirmRemove)
		};

		UIManager.ShowPanel("ConfirmationUI", data, true);
	}


	public void OnJoinServerPressed()
	{
		if (_selected == null)
		{
			return;
		}

		GameManager.Instance.OnJoinServerPressed(_selected.ServerAddress);
	}

	public void OnCancelEdit()
	{
		_editWindow.SetActive(false);
	}


	public void OnConfirmEdit()
	{
		_editWindow.SetActive(false);

		if (_entryToEdit == null)
		{
			AddEntry(_editName.text, _editAddress.text);
		}
		else
		{
			_entryToEdit.SetEntry(_editName.text, _editAddress.text);
		}

		SaveServerList();
	}


	public void OnBackPressed()
	{
		GameManager.Instance.OnMainMenuPressed();
	}


	private void OnCancelRemove()
	{
		// For now it's fine to do nothing
	}


	private void OnConfirmRemove()
	{
		_entries.Remove(_selected);

		Destroy(_selected.gameObject);

		_selected = null;

		SaveServerList();
	}


	private void OnEntryClicked(PointerEventData eventData)
	{
		if (eventData.pointerClick == null)
		{
			return;
		}

		if (eventData.pointerClick.TryGetComponent(out ServerEntryUI entry))
		{
			if (_selected != null)
			{
				_selected.SetSelected(false);
			}

			_selected = entry;

			_selected.SetSelected(true);
		}
	}


	private void AddEntry(string name, string address)
	{
		if (_selected != null)
		{
			_selected.SetSelected(false);
		}

		_selected = Instantiate(_entryPF, _entryHolder, false);

		_entries.Add(_selected);

		_selected.EntryClickedEvent += OnEntryClicked;

		_selected.SetEntry(name, address);

		_selected.SetSelected(true);
	}

	private void SaveServerList()
	{
		ServerEntryData[] data = new ServerEntryData[_entries.Count];

		for (int i = 0; i < _entries.Count; i++)
		{
			data[i] = new ServerEntryData
			{
				ServerName = _entries[i].ServerName,

				ServerAddress = _entries[i].ServerAddress
			};
		}

		if (!JsonFileIO.SaveToFile(SAVE_PATH, data))
		{
			Debug.LogError("Failed to save server list");
		}
	}


	private void LoadServerList()
	{
		ServerEntryData[] data = null;

		if (!JsonFileIO.LoadFromFile(SAVE_PATH, ref data))
		{
			Debug.LogError("failed to load server list");
		}

		if (data != null)
		{
			_selected = null;
			_entryToEdit = null;

			for (int i = 0; i < _entries.Count; i++)
			{
				Destroy(_entries[i].gameObject);
			}

			_entries.Clear();

			for (int i = 0; i < data.Length; i++)
			{
				AddEntry(data[i].ServerName, data[i].ServerAddress);
			}
		}
	}
}

public struct ServerEntryData
{
	public string ServerName;

	public string ServerAddress;
}