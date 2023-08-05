using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoints : MonoBehaviour
{
	private Dictionary<string, AttachPoint> _overrideAttachPoints;

	private Dictionary<string, AttachPoint> _attachPoints;

	private void Awake()
	{
		InitializeAttachPoints();
	}


	public Transform FindAttachPoint(string name)
	{
		if (!_overrideAttachPoints.TryGetValue(name, out AttachPoint attachPoint))
		{
			_attachPoints.TryGetValue(name, out attachPoint);
		}

		return attachPoint.transform;
	}


	public void AddOverride(string name, AttachPoint attachPoint)
	{
		_overrideAttachPoints[name] = attachPoint;
	}


	public void RemoveOverride(string name)
	{
		_overrideAttachPoints.Remove(name);
	}


	private void InitializeAttachPoints()
	{
		AttachPoint[] points = GetComponentsInChildren<AttachPoint>();

		_attachPoints = new Dictionary<string, AttachPoint>();

		foreach (AttachPoint point in points)
		{
			_attachPoints.Add(point.name, point);
		}

		_overrideAttachPoints = new Dictionary<string, AttachPoint>();
	}
}
