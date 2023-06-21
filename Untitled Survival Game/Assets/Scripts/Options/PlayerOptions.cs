using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOptions : MonoBehaviour
{
	private static PlayerOptions _instance;
	public static PlayerOptions Instance => _instance;

	private Resolution[] _resolutions;
	private List<string> _resolutionOptions;

	private PlayerOptionsData _optionsData = new PlayerOptionsData();

	private const string OPTIONS_PATH = "PlayerOptions.JSON";

	private const bool EDITOR_UNIQUE = true; // Save a seperate file for the editor if true


	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			_instance.LoadSettings();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public static void SetVolume(float volume)
	{
		_instance._optionsData.MasterVolume = volume;

		Debug.LogError($"Volume changed to {volume}");

		AudioListener.volume = volume;
	}


	public static void SetResolution(int choice)
	{
		if (choice >= Instance._resolutions.Length)
		{
			choice = 0;
		}

		_instance._optionsData.ResolutionChoice = choice;

		Resolution res = Instance._resolutions[choice];

		Screen.SetResolution(res.width, res.height, Screen.fullScreen);

		Debug.LogError($"Resolution changed to {res.width} x {res.height}");
	}


	public static void SetFullscreen(bool fullscreen)
	{
		_instance._optionsData.FullscreenMode = fullscreen;

		Screen.fullScreen = fullscreen;

		Debug.LogError($"Fullscreen Mode changed to {fullscreen}");
	}


	public static void SetUIScale(float scale)
	{
		_instance._optionsData.UIScale = scale;

		UIManager.Instance.UIScale = scale;
	}


	public void LoadSettings()
	{
		Debug.LogError("Loading Player Settings");

		int currentChoice = LoadResolutions();

		if (!JsonFileIO.LoadFromFile(OPTIONS_PATH, ref _optionsData, EDITOR_UNIQUE))
		{
			Debug.LogError("Failed to Load player options");
		}

		if (_optionsData == null)
		{
			_optionsData = new PlayerOptionsData();
		}

		// keep the current screen resolution if the resolution hasn't been set 
		if (_optionsData.ResolutionChoice == -1)
		{
			_optionsData.ResolutionChoice = currentChoice;
		}

		SetVolume(_optionsData.MasterVolume);

		SetResolution(_optionsData.ResolutionChoice);

		SetFullscreen(_optionsData.FullscreenMode);

		SetUIScale(_optionsData.UIScale);
	}


	public void SaveSettings()
	{
		Debug.LogError("Saving Player Settings");

		if (!JsonFileIO.SaveToFile(OPTIONS_PATH, _optionsData, EDITOR_UNIQUE))
		{
			Debug.LogError("Failed to save player options");
		}
	}


	public static SettingsUIData GetSettingsData()
	{
		PlayerOptionsData optionsData = Instance._optionsData;

		SettingsUIData data = new SettingsUIData
		{
			MasterVolume = optionsData.MasterVolume,

			Resolutions = Instance._resolutionOptions,

			ResolutionChoice = optionsData.ResolutionChoice,

			FullscreenMode = optionsData.FullscreenMode,

			UIScale = optionsData.UIScale,
		};

		return data;
	}


	private int LoadResolutions()
	{
		_resolutions = Screen.resolutions;

		_resolutionOptions = new List<string>();

		int resChoice = 0;

		for (int i = 0; i < _resolutions.Length; i++)
		{
			int width = _resolutions[i].width;
			int height = _resolutions[i].height;

			_resolutionOptions.Add($"{width} x {height}");

			// Find the choice that matches the current screen resolution
			if (width == Screen.currentResolution.width && height == Screen.currentResolution.height)
			{
				resChoice = i;
			}
		}

		return resChoice;
	}
}


public class PlayerOptionsData
{
	public float MasterVolume = 0.5f;

	public int ResolutionChoice = -1;

	public bool FullscreenMode = false;

	public float UIScale = 2f;
}
