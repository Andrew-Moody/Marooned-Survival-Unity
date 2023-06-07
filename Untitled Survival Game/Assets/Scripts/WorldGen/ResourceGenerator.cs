using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
	[SerializeField][Min(1)]
	private int _height;

	[SerializeField][Min(1)]
	private int _width;

	[SerializeField]
	private float _tileSize;

	[SerializeField]
	private int _seed;

	[SerializeField][Min(1)]
	private float _noiseScale;

	[SerializeField]
	[Min(1)]
	private int _octaves;

	[SerializeField]
	[Range(0, 1)]
	private float _persistance;

	[SerializeField]
	[Min(1)]
	private float _lacunarity;

	[SerializeField][Range(0, 1)]
	private float _spawnThreshold;

	[SerializeField]
	private Renderer _visualization;

	[SerializeField]
	private Color _fieldColor;

	[SerializeField]
	private Color[] _spawnColors;

	[SerializeField]
	private AnimationCurve _rarityCurve;

	[SerializeField]
	private ResourceOption[] _resourceOptions;

	private int _prevHeight;

	private int _prevWidth;

	private Texture2D _texture;

	private Color[] _colorMap;

	private float[,] _noiseMap;

	private int[,] _spawnMap;

	private LayerMask _spawnLayer;

	private void Awake()
	{
		_spawnLayer = LayerMask.NameToLayer("Ground");

		ResizeMaps();

		GenerateSpawnMap();
	}

	public void SpawnResources()
	{
		float halfWidth = (_width - 1) * _tileSize * 0.5f;
		float halfHeight = (_height - 1) * _tileSize * 0.5f;

		for (int i = 0; i < _width; i++)
		{
			for (int j = 0; j < _height; j++)
			{
				if (_noiseMap[i, j] > _spawnThreshold)
				{
					Vector3 position = new Vector3(i * _tileSize - halfWidth, 10f, j * _tileSize - halfHeight);

					int id = _spawnMap[i, j];

					SpawnResource(id, position);
				}
			}
		}
	}


	private void SpawnResource(int id, Vector3 position)
	{
		Physics.Raycast(position, Vector3.down, out RaycastHit hitInfo, 20f);

		if (hitInfo.collider.gameObject.layer == _spawnLayer)
		{
			Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

			DestructibleManager.Instance.SpawnDestructable(id, hitInfo.point, rotation, transform);
		}
		else
		{
			//Debug.Log("RayCast hit wrong layer " + hitInfo.collider.gameObject.name + " " + hitInfo.collider.gameObject.layer + " " + _spawnLayer.value);
		}
	}



	protected override void OnValidate()
	{
		if (_height != _prevHeight || _width != _prevWidth)
		{
			ResizeMaps();
		}

		GenerateSpawnMap();

		GenerateTexture();
	}

	private void GenerateSpawnMap()
	{
		_noiseMap = NoiseFunctions.GetNoiseMap(_width, _height, _seed, _noiseScale, _octaves, _persistance, _lacunarity, Vector2.zero);

		if (_spawnMap == null)
		{
			ResizeMaps();
		}

		UnityEngine.Random.InitState(_seed);

		float invMaxDist = 2f / Mathf.Sqrt(_width * _width + _height * _height);

		for (int i = 0; i < _width; i++)
		{
			for (int j = 0; j < _height; j++)
			{
				if (_noiseMap[i, j] > _spawnThreshold)
				{
					int index = PickResource(i, j, invMaxDist);

					_spawnMap[i, j] = _resourceOptions[index].ID;

					_colorMap[i + j * _width] = _spawnColors[index];
				}
				else
				{
					_spawnMap[i, j] = 0;

					_colorMap[i + j * _width] = _fieldColor;
				}
			}
		}
	}

	private void ResizeMaps()
	{
		_spawnMap = new int[_width, _height];

		_texture = new Texture2D(_width, _height);

		_texture.filterMode = FilterMode.Point;

		_colorMap = new Color[_width * _height];

		_prevHeight = _height;
		_prevWidth = _width;

		_visualization.sharedMaterial.mainTexture = _texture;
	}


	private void GenerateTexture()
	{
		_texture.SetPixels(_colorMap);
		_texture.Apply();
	}


	private int PickResource(int i, int j, float invMaxDist)
	{
		float xDist = i - 0.5f * _width;
		float yDist = j - 0.5f * _height;
		float normalizedDist = Mathf.Sqrt(xDist * xDist + yDist * yDist) * invMaxDist;

		int maxTier = 0;

		for (int tier = 0; tier < _spawnColors.Length; tier++)
		{
			if (_resourceOptions[tier].Rarity > _rarityCurve.Evaluate(normalizedDist))
			{
				maxTier = tier;

				if (tier == 9)
				{
					//Debug.Log($"max");
				}
			}
			else
			{
				
				break;
			}
		}

		int minTier = maxTier - 3;
		if (minTier < 0)
		{
			minTier = 0;
		}

		int index = UnityEngine.Random.Range(minTier, maxTier + 1);

		return index;
	}
}

[System.Serializable]
public struct ResourceOption
{
	public int ID;
	public float Rarity;
}
