using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
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
    private Color _spawnColor;

    private int _prevHeight;

    private int _prevWidth;

    private Texture2D _texture;

    private Color[] _colorMap;

    private float[,] _noiseMap;

    private LayerMask _spawnLayer;

	private void Awake()
	{
        _spawnLayer = LayerMask.NameToLayer("Ground");

        ResizeTexture();

        GenerateSpawnMap();
	}

	public void SpawnResources()
	{
        float halfWidth = (_width - 1) * _tileSize * 0.5f;
        float halfHeight = (_height - 1) * _tileSize * 0.5f;

        UnityEngine.Random.InitState(_seed);

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_noiseMap[i, j] > _spawnThreshold)
                {
                    Vector3 position = new Vector3(i * _tileSize - halfWidth, 10f, j * _tileSize - halfHeight);

                    int id = UnityEngine.Random.Range(1, 7);

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



	private void OnValidate()
	{
		if (_height != _prevHeight || _width != _prevWidth)
		{
            ResizeTexture();
		}

        GenerateSpawnMap();

        GenerateTexture();
	}

	private void GenerateSpawnMap()
	{
        _noiseMap = NoiseFunctions.GetNoiseMap(_width, _height, _seed, _noiseScale, _octaves, _persistance, _lacunarity, Vector2.zero);
    }

	private void ResizeTexture()
	{
        _texture = new Texture2D(_width, _height);

        _texture.filterMode = FilterMode.Point;

        _colorMap = new Color[_width * _height];

        _prevHeight = _height;
        _prevWidth = _width;

        _visualization.sharedMaterial.mainTexture = _texture;
	}


    private void GenerateTexture()
	{
        for (int i = 0; i < _width; i++)
		{
            for (int j = 0; j < _height; j++)
			{
                if (_noiseMap[i, j] > _spawnThreshold)
				{
                    _colorMap[i + j * _width] = _spawnColor;
                }
				else
				{
                    _colorMap[i + j * _width] = _fieldColor;
                }
			}
		}

        _texture.SetPixels(_colorMap);
        _texture.Apply();
	}
}
