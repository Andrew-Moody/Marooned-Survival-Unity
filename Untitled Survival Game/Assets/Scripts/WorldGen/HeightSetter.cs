using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightSetter : MonoBehaviour
{
	private const float _originHeight = 15f;

	private const float _maxDistance = 30f;

	//LayerMask.GetMask("Ground") wont work here (even if not const);
	private const int _layerMask = 1 << 6; 


	void Start()
	{
		Debug.LogWarning("HeightSetter Start");

		WorldGenManager.Instance.WorldGenEnded += WorldGenManager_WorldGenEnded;
	}


	void OnDestroy()
	{
		WorldGenManager.Instance.WorldGenEnded -= WorldGenManager_WorldGenEnded;
	}


	private void WorldGenManager_WorldGenEnded()
	{
		Debug.LogWarning("HeightSetter Recieved WorldGenEnded");

		Vector3 origin = transform.position;

		origin.y = _originHeight;

		Vector3 direction = Vector3.down;

		if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, _maxDistance, _layerMask))
		{
			transform.position = hitInfo.point;
		}
		else
		{
			transform.position = origin;
		}
	}
}
