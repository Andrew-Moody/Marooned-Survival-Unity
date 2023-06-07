using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlock : MonoBehaviour
{
	[SerializeField]
	private Vector3 _velocity;


	void Start()
	{
		
	}


	void Update()
	{
		transform.position += Time.deltaTime * _velocity;
	}
}
