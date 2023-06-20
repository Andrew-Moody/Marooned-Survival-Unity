using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ServerEntryUI : MonoBehaviour, IPointerClickHandler
{
	public string ServerName => _serverName.text;

	public string ServerAddress => _serverAddress.text;


	public event System.Action<PointerEventData> EntryClickedEvent;


	[SerializeField]
	private Image _selection;

	[SerializeField]
	private TextMeshProUGUI _serverName;

	[SerializeField]
	private TextMeshProUGUI _serverAddress;


	public void SetEntry(string name, string address)
	{
		_serverName.text = name;

		_serverAddress.text = address;
	}


	public void SetSelected(bool selected)
	{
		_selection.enabled = selected;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		EntryClickedEvent?.Invoke(eventData);
	}
}
