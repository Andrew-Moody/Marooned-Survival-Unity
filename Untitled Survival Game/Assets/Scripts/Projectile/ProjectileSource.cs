using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSource : MonoBehaviour
{
	[SerializeField]
	private Transform _target;
	public Transform Target => _target;
}
