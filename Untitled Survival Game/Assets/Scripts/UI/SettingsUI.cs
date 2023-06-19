using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : UIPanel
{
	[SerializeField]
	private Slider _masterVolume;

	[SerializeField]
	private TMP_Dropdown _resolution;

	[SerializeField]
	private Toggle _fullscreen;


	[SerializeField]
	private Button _mainMenuButton;

	public void OnVolumeChanged(float volume)
	{
		PlayerOptions.SetVolume(volume);
	}


	public void OnResolutionChanged(int choice)
	{
		PlayerOptions.SetResolution(choice);
	}


	public void OnFullscreenChanged(bool fullscreen)
	{
		PlayerOptions.SetFullscreen(fullscreen);
	}


	public void OnBackPressed()
	{
		UIManager.HideStackTop(true);
	}


	public void OnExitPressed()
	{
		GameManager.Instance.OnMainMenuPressed();
	}


	public override void Hide()
	{
		base.Hide();

		PlayerOptions.SaveSettings();
	}


	public override void Show(UIPanelData data)
	{
		base.Show(data);

		if (data is SettingsUIData settings)
		{
			_masterVolume.SetValueWithoutNotify(settings.MasterVolume);

			_resolution.ClearOptions();
			_resolution.AddOptions(settings.Resolutions);

			_resolution.SetValueWithoutNotify(settings.ResolutionChoice);

			_fullscreen.SetIsOnWithoutNotify(settings.FullscreenMode);
		}

		_mainMenuButton.gameObject.SetActive(!FishNet.InstanceFinder.IsOffline);
	}
}


public class SettingsUIData : UIPanelData
{
	public float MasterVolume;

	public List<string> Resolutions;

	public int ResolutionChoice;

	public bool FullscreenMode;
}