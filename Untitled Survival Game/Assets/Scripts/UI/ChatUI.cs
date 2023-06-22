using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatUI : UIPanel
{
	private static ChatUI _instance;

	[SerializeField]
	private TextMeshProUGUI _chatTMP;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		
	}


	public static void SendChat(string message)
	{
		if (_instance != null)
		{
			_instance._chatTMP.text = message;
		}
		
	}
}
