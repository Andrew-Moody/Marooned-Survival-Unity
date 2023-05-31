using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	[SerializeField]
	private int _height;

	[SerializeField]
	private int _width;

	[SerializeField]
	private float _tileSize;

	/// <summary>
	/// The number of tiles to be covered by a single texture
	/// </summary>
	[SerializeField]
	private int _textureTileSize;


	[SerializeField]
	private int _seed;


	[SerializeField][Min(1f)]
	private float _noiseScale;

	/// <summary>
	/// How many layers of noise to use
	/// </summary>
	[SerializeField][Min(1)]
	private int _octaves;

	/// <summary>
	/// How much influence each successive layer has on the amplitude
	/// </summary>
	[SerializeField][Range(0, 1)]
	private float _persistance;

	/// <summary>
	/// How much influence each successive layer has on the frequency
	/// </summary>
	[SerializeField][Min(1)]
	private float _lacunarity;


	[SerializeField]
	private Vector2 _offset;

	[SerializeField]
	private float _falloffStart;

	[SerializeField]
	private float _falloffSlope;

	private Mesh _mesh;

	private Vector3[] _vertices;

	private int _prevHeight;
	private int _prevWidth;
	

	void Start()
	{
		if (_mesh == null)
		{
			CreateMesh();

			//DistortMesh();
		}

		GetComponent<MeshCollider>().sharedMesh = _mesh;

		GetComponent<NavMeshSurface>().BuildNavMesh();
	}


	public void CreateMesh()
	{
		_mesh = new Mesh();

		_mesh.name = "Procedural Mesh";

		_prevHeight = _height;
		_prevWidth = _width;


		transform.position = new Vector3(-_width * 0.5f * _tileSize, 0f, -_height * 0.5f * _tileSize);


		int vertexCount = (_height + 1) * (_width + 1);

		_vertices = new Vector3[vertexCount];


		for (int i = 0; i <= _height; i++)
		{
			for (int j = 0; j <= _width; j++)
			{
				int index = j + i * (_height + 1);

				_vertices[index] = new Vector3(j * _tileSize, 0f, i * _tileSize);
			}
		}



		int triangleCount = 6 * _height * _width;

		int[] triangles = new int[triangleCount];

		for (int i = 0; i < _height; i++)
		{
			for (int j = 0; j < _width; j++)
			{
				int index = 6 * (j + i * (_height));

				triangles[index] = j + i * (_height + 1);
				triangles[index + 1] = j + (i + 1) * (_height + 1);
				triangles[index + 2] = (j + 1) + i * (_height + 1);

				triangles[index + 3] = j + (i + 1) * (_height + 1);
				triangles[index + 4] = (j + 1) + (i + 1) * (_height + 1);
				triangles[index + 5] = (j + 1) + i * (_height + 1);
			}
		}


		// Could use RecalculateNormals but this gives more control
		// Note that normals will need to be calculated again after deforming mesh by height



		float uConstant = 1f / (float)_textureTileSize;

		float vConstant = 1f / (float)_textureTileSize;


		Vector2[] uvcoords = new Vector2[vertexCount];

		for (int i = 0; i <= _height; i++)
		{
			for (int j = 0; j <= _width; j++)
			{
				int index = j + i * (_height + 1);

				uvcoords[index] = new Vector2((float)j * uConstant, (float)i * vConstant);
			}
		}



		_mesh.vertices = _vertices;

		_mesh.triangles = triangles;

		//_mesh.normals = normals;
		_mesh.RecalculateNormals();

		_mesh.uv = uvcoords;
		


		GetComponent<MeshFilter>().mesh = _mesh;

		DistortMesh();
	}



	private void DistortMesh()
	{
		float[,] map = NoiseFunctions.GetNoiseMap(_width + 1, _height + 1, _seed, _noiseScale, _octaves, _persistance, _lacunarity, _offset);

		NoiseFunctions.ApplyFalloff(map, _falloffStart, _falloffSlope);

		for (int i = 0; i <= _height; i++)
		{
			for (int j = 0; j <= _width; j++)
			{
				int index = j + i * (_height + 1);

				_vertices[index].y = map[i, j];
			}
		}

		_mesh.vertices = _vertices;

		_mesh.RecalculateNormals();

		_mesh.RecalculateBounds(); // Fixes issues with mesh being culled when it should be visible
	}



	private void OnValidate()
	{
		if (_mesh == null)
		{
			return;
		}

		if (_height != _prevHeight || _width != _prevWidth)
		{
			// Yep this still causes the sendmessage cant be called in OnValidate
			//CreateMesh();

			// Manually resizing isnt a huge issue because resizing doesnt happen often
			// I just wanted live updates when changing noise parameters so this works
			Debug.LogWarning("Mesh size settings have changed but the mesh has not been resized");
			return;
		}

		DistortMesh();
	}
}
