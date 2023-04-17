using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSPObject : NetworkBehaviour
{
	protected Vector3 _velocity;
	protected Vector3 _angularVelocity;

	
	public virtual ReconcileData GetReconcileData()
	{
		return new ReconcileData(transform.position, transform.rotation.eulerAngles, _velocity, _angularVelocity);
	}
    
	public virtual void Replicate(InputData data, bool asServer)
	{
		
	}

    public virtual void Reconcile(ReconcileData data)
	{
		transform.position = data.Position;
		transform.rotation = Quaternion.Euler(data.Rotation);
		_velocity = data.Velocity;
		_angularVelocity = data.AngularVelocity;
	}
}
