using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPlottable : Plottable
{
	[SerializeField]
	private Transform _target;

	[SerializeField]
	private Direction _direction;

	private enum Direction
	{
		X,
		Y,
		Z
	}

	public override float GetValue()
	{
		switch (_direction)
		{
			case Direction.X:
			{
				return _target.position.x;
			}
			case Direction.Y:
			{
				return _target.position.y;
			}
			case Direction.Z:
			{
				return _target.position.z;
			}
			default:
			{
				return 0f;
			}
		}
	}
}
