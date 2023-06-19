using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOptions : MonoBehaviour
{
	private static PlayerOptions Instance;

	private float _masterVolume;

	private Resolution[] _resolutions;
	private List<string> _resolutionOptions;

	private int _resolutionChoice;

	private bool _fullscreenMode;





	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			LoadSettings();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public static void SetVolume(float volume)
	{
		Instance._masterVolume = volume;

		Debug.LogError($"Volume changed to {volume}");

		AudioListener.volume = volume;
	}


	public static void SetResolution(int choice)
	{
		Instance._resolutionChoice = choice;

		Resolution res = Instance._resolutions[choice];

		Screen.SetResolution(res.width, res.height, Screen.fullScreen);

		Debug.LogError($"Resolution changed to {res.width} x {res.height}");
	}


	public static void SetFullscreen(bool fullscreen)
	{
		Instance._fullscreenMode = fullscreen;

		Screen.fullScreen = fullscreen;

		Debug.LogError($"Fullscreen Mode changed to {fullscreen}");
	}


	public static void LoadSettings()
	{
		Debug.LogError("Loading Player Settings");

		if (Instance != null)
		{
			Instance.Load();
		}
		else
		{
			Debug.LogError("Player Settings Failed to Saved");
		}
	}


	public static void SaveSettings()
	{
		Debug.LogError("Saving Player Settings");

		if (Instance != null)
		{
			Instance.Save();
		}
		else
		{
			Debug.LogError("Player Settings Failed to Saved");
		}
	}


	public static SettingsUIData GetSettingsData()
	{
		SettingsUIData data = new SettingsUIData
		{
			MasterVolume = Instance._masterVolume,

			Resolutions = Instance._resolutionOptions,

			ResolutionChoice = Instance._resolutionChoice,

			FullscreenMode = Instance._fullscreenMode
		};

		return data;
	}


	private void Load()
	{
		if (PlayerPrefs.HasKey("MasterVolume"))
		{
			_masterVolume = PlayerPrefs.GetFloat("MasterVolume");

			Debug.LogError($"Found PlayerPrefs key: MasterVolume {_masterVolume}");
		}
		else
		{
			Debug.LogError("PlayerPrefs key not found: MasterVolume");
		}


		_resolutions = Screen.resolutions;

		_resolutionOptions = new List<string>();

		for (int i = 0; i < _resolutions.Length; i++)
		{
			int width = _resolutions[i].width;
			int height = _resolutions[i].height;

			_resolutionOptions.Add($"{width} x {height}");

			if (width == Screen.currentResolution.width && height == Screen.currentResolution.height)
			{
				_resolutionChoice = i;
			}
		}

		if (_resolutionChoice == -1)
		{
			Debug.LogError("Failed to find screen resolution match");
		}

		if (PlayerPrefs.HasKey("ResolutionChoice"))
		{
			_resolutionChoice = PlayerPrefs.GetInt("ResolutionChoice");

			Debug.LogError($"Found PlayerPrefs key: ResolutionChoice {_resolutionChoice}");
		}
		else
		{
			Debug.LogError("PlayerPrefs key not found: ResolutionChoice");
		}

		if (PlayerPrefs.HasKey("FullscreenMode"))
		{
			_fullscreenMode = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenMode"));

			Debug.LogError($"Found PlayerPrefs key: FullscreenMode {_fullscreenMode}");
		}
		else
		{
			Debug.LogError("PlayerPrefs key not found: FullscreenMode");
		}


		SetVolume(_masterVolume);

		SetResolution(_resolutionChoice);

		SetFullscreen(_fullscreenMode);
	}


	private void Save()
	{
		Debug.LogError($"Vol {_masterVolume}, Res {_resolutionChoice}, FS {_fullscreenMode}");

		PlayerPrefs.SetFloat("MasterVolume", _masterVolume);

		PlayerPrefs.SetInt("ResolutionChoice", _resolutionChoice);

		PlayerPrefs.SetInt("FullscreenMode", System.Convert.ToInt32(_fullscreenMode));

		PlayerPrefs.Save();
	}
}
