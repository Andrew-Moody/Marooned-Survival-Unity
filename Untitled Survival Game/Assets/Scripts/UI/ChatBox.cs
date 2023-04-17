using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatBox : MonoBehaviour
{
	public static ChatBox Instance;

	[SerializeField]
	private TextMeshProUGUI _chatTMP;

	private void Awake()
	{
		Instance = this;
	}


	public void SendChat(string message)
	{
		_chatTMP.text = message;
	}
}
