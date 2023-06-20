using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfirmationUI : UIPanel
{
	[SerializeField]
	private TextMeshProUGUI _prompt;

	private System.Action _cancelPressed;

	private System.Action _confirmPressed;


	public void OnConfirmPressed()
	{
		_confirmPressed?.Invoke();

		UIManager.HideStackTop(true);
	}


	public void OnCancelPressed()
	{
		_cancelPressed?.Invoke();

		UIManager.HideStackTop(true);
	}


	public override void Show(UIPanelData data)
	{
		base.Show(data);

		if (data is ConfirmationUIData confirmData)
		{
			_prompt.text = confirmData.Message;

			_cancelPressed = confirmData.OnCancelPressed;

			_confirmPressed = confirmData.OnConfirmPressed;
		}
	}
}


public class ConfirmationUIData : UIPanelData
{
	public string Message;

	public System.Action OnCancelPressed;

	public System.Action OnConfirmPressed;
}