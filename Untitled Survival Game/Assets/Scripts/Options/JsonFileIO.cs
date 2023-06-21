using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class JsonFileIO
{
	public static bool LoadFromFile<T>(string relativePath, ref T data, bool editorUnique = false)
	{
		string path = Application.persistentDataPath;

#if UNITY_EDITOR
		if (editorUnique)
		{
			path += "/Editor_" + relativePath;
		}
		else
		{
			path += "/" + relativePath;
		}
#else
		path += "/" + relativePath;
#endif


		if (!File.Exists(path))
		{
			Debug.LogError("PlayerOptions file not found at: " + path);
			return false;
		}


		try
		{
			data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));

			return true;
		}
		catch(System.Exception e)
		{
			Debug.LogError(e.Message);
			return false;
		}
	}



	public static bool SaveToFile<T>(string relativePath, T data, bool editorUnique = false)
	{

		string finalPath = Application.persistentDataPath;

#if UNITY_EDITOR
		
		if (editorUnique)
		{
			finalPath += "/Editor_" + relativePath;
		}
		else
		{
			finalPath += "/" + relativePath;
		}
#else
		finalPath += "/" + relativePath;
#endif

		string tempPath = finalPath + "_temp";

		try
		{
			// Opens and closes automatically, creates a new file if one does not exist and overwrites if it does
			File.WriteAllText(tempPath, JsonConvert.SerializeObject(data, Formatting.Indented));

			// Prefer StreamWriter/Reader to FileStream when you need read-only or write-only
			// with any of them you need to dispose at the end or use with "using" to automatically dispose

			// Delete the previous file if it exists
			File.Delete(finalPath);

			// Rename the new file to remove temp label
			File.Move(tempPath, finalPath);

			return true;
		}
		catch (System.Exception e)
		{
			Debug.LogError(e.Message);

			return false;
		}
	}
}
