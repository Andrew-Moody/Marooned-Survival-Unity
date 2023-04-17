using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NMATest : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _agent;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _standoffDistance;

    [SerializeField]
    private float _backupFactor;

    [SerializeField]
    private float _rotateSpeed;


    [SerializeField]
    private float _smoothTime;

    [SerializeField]
    private float _maxSpeed;

    private Vector3 _targetDirection;
    private Vector3 _destination;
    private bool _onApproach;


    private Vector3 _velocity = Vector3.zero;

	private void Awake()
	{
        //_agent.updatePosition = false;
	}

	// Update is called once per frame
	void FixedUpdate()
    {
        FollowStandoff();

        
    }

    private void RotateTowardTarget()
    {
        _targetDirection = _target.position - transform.position;
        _targetDirection.y = 0f;

        if (_targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
        }
    }


    private void FollowSimple()
	{
        _destination = _target.position;
        _agent.SetDestination(_destination);
    }


    private void FollowSmooth()
	{
        _destination = _target.position;
        _agent.SetDestination(_destination);

        transform.position = Vector3.SmoothDamp(transform.position, _agent.nextPosition, ref _velocity, _smoothTime, _maxSpeed);
    }


    private void FollowStandoff()
	{
        Vector3 vectorToTarget = _target.position - transform.position;
        float distance = vectorToTarget.magnitude;

        if (distance > 2 * _standoffDistance)
        {
            // From far its better to path straight to target. if the path is not straight you may be
            // approaching the target at the end from a different direction than directly from start to end
            _destination = _target.position;
            _agent.SetDestination(_destination);
            _onApproach = false;

            //Debug.Log("Setting approx dest");
        }
        else if (distance > _standoffDistance)
        {
            // Agent has moved close enough to the target a spot near the target rather than the target itself
            _destination = transform.position + vectorToTarget * (distance - _standoffDistance);
            _agent.SetDestination(_destination);
            _onApproach = true;

            //Debug.Log("Setting precise dest");
        }
        else if (distance < 0.9 * _standoffDistance)
        {
            // Back away from the player
            _destination = transform.position - vectorToTarget * _backupFactor *(_standoffDistance - distance);
            _agent.SetDestination(_destination);

            //Debug.Log("Setting backup dest");
        }



        if (distance <= 2 * _standoffDistance)
        {
            _agent.updateRotation = false;
            RotateTowardTarget();

            if (distance > _standoffDistance && !_onApproach)
            {
                // Agent has moved close enough to the target a spot near the target rather than the target itself
                _destination = transform.position + vectorToTarget * (distance - _standoffDistance);
                _agent.SetDestination(_destination);
                _onApproach = true;
                //Debug.Log("Setting precise dest");
            }
        }
        else
        {
            _agent.updateRotation = true;
        }

        _velocity = _agent.velocity;
    }



    private void StandoffSmooth()
	{
        Vector3 vectorToTarget = _target.position - transform.position;
        float distance = vectorToTarget.magnitude;

        if (distance > 2 * _standoffDistance)
        {
            // From far its better to path straight to target. if the path is not straight you may be
            // approaching the target at the end from a different direction than directly from start to end
            _destination = _target.position;
            _agent.SetDestination(_destination);
            _onApproach = false;

            //Debug.Log("Setting approx dest");
        }
        else if (distance > _standoffDistance)
        {
            // Agent has moved close enough to the target a spot near the target rather than the target itself
            _destination = transform.position + vectorToTarget * (distance - _standoffDistance);
            _agent.SetDestination(_destination);
            _onApproach = true;

            //Debug.Log("Setting precise dest");
        }
        else if (distance < 0.9 * _standoffDistance)
        {
            // Back away from the player
            _destination = transform.position - vectorToTarget * _backupFactor * (_standoffDistance - distance);
            _agent.SetDestination(_destination);

            //Debug.Log("Setting backup dest");
        }



        if (distance <= 2 * _standoffDistance)
        {
            //_agent.updateRotation = false;
            //RotateTowardTarget();

            if (distance > _standoffDistance && !_onApproach)
            {
                // Agent has moved close enough to the target a spot near the target rather than the target itself
                _destination = transform.position + vectorToTarget * (distance - _standoffDistance);
                _agent.SetDestination(_destination);
                _onApproach = true;
                //Debug.Log("Setting precise dest");
            }
        }
        else
        {
            //_agent.updateRotation = true;
        }

        transform.position = Vector3.SmoothDamp(transform.position, _agent.nextPosition, ref _velocity, _smoothTime, _maxSpeed);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 pos = transform.position;
        //pos.y += 2f;
        Gizmos.DrawLine(pos, pos + _velocity);

        Gizmos.DrawSphere(_destination, 0.1f);


        Vector3[] corners = _agent.path.corners;

        Gizmos.color = Color.blue;

        for (int i = 1; i < corners.Length; i++)
		{
            Gizmos.DrawLine(corners[i - 1], corners[i]);
		}
    }
}
