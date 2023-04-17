using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _optionTMP;

    private Button _button;

    private ContextOption _option;

    void Start()
    {
        Debug.Log("Adding Listener");
        _button = gameObject.GetComponent<Button>();
        _button.onClick.AddListener(OnSelectOption);
    }


	private void OnDestroy()
	{
		if (_button != null)
		{
            _button.onClick.RemoveListener(OnSelectOption);
        }
	}


    public void SetOption(ContextOption option)
	{
        _option = option;
        _optionTMP.text = option.OptionText;
    }


    public void OnSelectOption()
	{
        _option.Callback?.Invoke(_option.SlotIndex);

        gameObject.GetComponentInParent<ContextUI>().Hide();
	}
}
