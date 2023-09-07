using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class LobbyPlayer : NetworkBehaviour
{
	private const float _originHeight = 15f;

	private const float _maxDistance = 30f;

	//LayerMask.GetMask("Ground") wont work here (even if not const);
	private const int _layerMask = 1 << 6;


	public override void OnStartClient()
	{
		base.OnStartClient();

		SetYPosition();
	}


	private void SetYPosition()
	{
		Vector3 origin = transform.position;

		origin.y = _originHeight;

		Vector3 direction = Vector3.down;

		if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, _maxDistance, _layerMask))
		{
			Debug.Log($"Hit: {hitInfo.collider.gameObject.name} at: {hitInfo.point}");

			Debug.LogWarning("LobbyPlayer Set Y Position: " + hitInfo.point.y);
			transform.position = hitInfo.point;
		}
		else
		{
			transform.position = origin;
		}
	}
}
