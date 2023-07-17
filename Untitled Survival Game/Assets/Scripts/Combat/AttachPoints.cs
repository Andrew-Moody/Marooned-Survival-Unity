using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoints : MonoBehaviour
{
	private Dictionary<string, AttachPoint> _attachPoints;


	public Transform FindAttachPoint(string name)
	{
		if (_attachPoints == null)
		{
			InitializeAttachPoints();
		}

		if (_attachPoints.TryGetValue(name, out AttachPoint attachPoint))
		{
			return attachPoint.transform;
		}

		return null;
	}


	private void InitializeAttachPoints()
	{
		AttachPoint[] points = GetComponentsInChildren<AttachPoint>();

		_attachPoints = new Dictionary<string, AttachPoint>();

		foreach (AttachPoint point in points)
		{
			_attachPoints.Add(point.name, point);
		}
	}
}
