using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DestViewer : MonoBehaviour
{
	public NavMeshAgent agent;


	[SerializeField]
	private Mode _mode;

	private enum Mode
	{
		FixedUpdate,
		Update,
		LateUpdate
	}

	
	void Update()
	{
		if (_mode == Mode.Update)
		{
			UpdateTransform();
		}
	}


	private void LateUpdate()
	{
		if (_mode == Mode.LateUpdate)
		{
			UpdateTransform();
		}
	}


	private void FixedUpdate()
	{
		if (_mode == Mode.FixedUpdate)
		{
			UpdateTransform();
		}
	}

	private void UpdateTransform()
	{
		if (agent.hasPath)
		{
			transform.position = agent.destination;
		}
	}

}
