using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFunctions
{
    public static float[,] GetNoiseMap(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
	{
		float[,] map = new float[width, height];

		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];

		float midWidth = width / 2f;
		float midHeight = height / 2f;

		for (int i = 0; i < octaves; i++)
		{
			float offsetX = prng.Next(-100000, 100000) + offset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y;

			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;


		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				float amplitude = 1f;
				float frequency = 1f;
				float noiseheight = 0f;

				for (int octave = 0; octave < octaves; octave++)
				{
					float x = (i - midWidth) / scale * frequency + octaveOffsets[octave].x;
					float y = (j - midHeight) / scale * frequency + octaveOffsets[octave].y;

					float sample = Mathf.PerlinNoise(x, y);

					noiseheight += amplitude * sample;




					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseheight > maxNoiseHeight)
				{
					maxNoiseHeight = noiseheight;
				}
				else if (noiseheight < minNoiseHeight)
				{
					minNoiseHeight = noiseheight;
				}

				map[i, j] = noiseheight;
			}
		}



		// Have to remap the values back to -1 to 1
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				map[i, j] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[i, j]) * 2f - 1f; // remap from 0 to 1 to -1 to 1
			}
		}

		return map;
	}


	public static void CombineMaps(float[,] mapA, float[,] mapB)
	{
		if (mapA.GetLength(0) != mapB.GetLength(0) || mapA.GetLength(1) != mapB.GetLength(1))
		{
			return;
		}

		for (int i = 0; i < mapA.GetLength(0); i++)
		{
			for (int j = 0; j < mapA.GetLength(1); j++)
			{
				mapA[i, j] += mapB[i, j];
			}
		}
	}


	public static void ApplyFalloff(float[,] map, float start, float slope)
	{
		float halfWidth = map.GetLength(0) / 2f;
		float halfHeight = map.GetLength(1) / 2f;

		float squareStart = start * start;

		for (int i = 0; i < map.GetLength(0); i++)
		{
			for (int j = 0; j < map.GetLength(1); j++)
			{

				float x = i - halfWidth;
				float y = j - halfHeight;

				float distance = x * x + y * y - squareStart;

				if (distance > 0)
				{
					map[i, j] -= distance * slope;
				}
			}
		}
	}
}
