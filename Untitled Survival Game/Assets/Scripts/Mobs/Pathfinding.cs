using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    //[SerializeField]
    private float _viewRange;

    //[SerializeField]
    private LayerMask _viewMask;

    [SerializeField]
    private bool _useThreshold;

    [SerializeField]
    private float _moveThreshold;

    [SerializeField]
    private float _standoffDistance;

    [SerializeField]
    private float _backoffFactor;

    [SerializeField]
    private float _rotateSpeed;

    [SerializeField]
    private NavMeshAgent _agent;

    private Vector3 _lastTargetPosition;

    private Vector3 _targetDirection;

    private bool _onApproach;

    private Vector3 _destination;

    private bool _followTarget;

    private bool _toClose;

    public bool Flipped { get; private set; }


    // Stun and KnockBack
    private CharacterController _characterController;

    private bool _isStunned;

    private bool _isServer;

    private float _stunTime;

    private Vector3 _velocity;

    private Vector3 _prevPosition;

    private Vector3 _deltaPos;

    [SerializeField]
    private float _deceleration;

    [SerializeField]
    private float _gravity;

	private void Awake()
	{
        _characterController = GetComponent<CharacterController>();
        //_characterController.enabled = false;
	}


	/// <summary>
	/// Agent will attempt to follow the target transform while maintaining StandoffDistance.
	/// Path is updated whenever the target moves more than the MoveThreshold.
	/// </summary>
	/// <param name="target">The transform to follow</param>
	public void SetTarget(Transform target)
    {
        _target = target;
        _followTarget = true;
    }


    /// <summary>
    /// Set the Agent destination to a world position.
    /// </summary>
    /// <param name="destination">The desired destination in world space</param>
    public void SetDestination(Vector3 destination)
	{
        _agent.SetDestination(destination);
        _followTarget = false;
	}

    
    /// <summary>
    /// Returns true if the Agent has reached its current destination within stopping distance
    /// </summary>
    /// <returns></returns>
    public bool Arrived()
	{
        return _agent.remainingDistance <= _agent.stoppingDistance;
	}


    public void EnableNMAgent(bool enable)
	{
        _agent.enabled = enable;
	}


    public void SetIsServer(bool isServer)
	{
        _isServer = isServer;
	}

    /// <summary>
    /// Returns the normalized velocity of the navmeshagent relative to the transform forward direction
    /// </summary>
    /// <returns></returns>
    public Vector3 GetNormalisedVelocity()
	{
        float speedZ = Vector3.Dot(_agent.velocity, transform.forward) / _agent.speed;
        float speedX = Vector3.Dot(_agent.velocity, transform.right) / _agent.speed;

        return new Vector3(speedX, 0f, speedZ);
        //return _agent.velocity / _agent.speed;
	}


    public float GetDistanceToTarget()
	{
        if (_target != null)
		{
            return (_target.position - transform.position).magnitude;
        }

        return 0f;
    }


    public void KnockBack(Vector3 direction, float strength, bool asServer)
	{
        Debug.Log("KnockBack called on client");

        if (_isServer)
		{
            EnableNMAgent(false);
        }
        

        _isStunned = true;
        _stunTime = 1.5f;

        _characterController.enabled = true;

        _velocity = direction * strength;
	}



    void Update()
    {
        //Tick(Time.deltaTime);

        if (transform.position != _prevPosition)
		{
            _deltaPos = transform.position - _prevPosition;

            //Debug.Log(_deltaPos.x);

            _prevPosition = transform.position;
        }
        
    }

    public void Tick(float deltaTime)
	{

        if (_isStunned)
		{
            _characterController.Move(_velocity * deltaTime);

            _velocity -= _velocity * _deceleration * deltaTime;
            _velocity.y -= _gravity * deltaTime;

            _stunTime -= deltaTime;


			if (_stunTime <= 0f)
			{
				_isStunned = false;

                if (_isServer)
				{
                    EnableNMAgent(true);
                }
				
				//_characterController.enabled = false;
			}

            _prevPosition = transform.position;

			return;
		}


        if (_isServer && _followTarget)
        {
            FollowTarget(deltaTime);
        }


        _prevPosition = transform.position;
    }


    private void FollowTarget(float deltaTime)
    {
        if (_target == null)
            return;

        float targetDelta = (_target.position - _lastTargetPosition).magnitude;

        Vector3 vectorToTarget = _target.position - transform.position;
        float distance = vectorToTarget.magnitude;

        if (_useThreshold)
        {
            if (targetDelta > _moveThreshold)
			{
                _lastTargetPosition = _target.position;

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
                    _destination = transform.position - vectorToTarget * _backoffFactor * (_standoffDistance - distance);
                    _agent.SetDestination(_destination);

                    //Debug.Log("Setting backup dest");
                }
            }
        }
		else
		{
            _lastTargetPosition = _target.position;

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
                _destination = transform.position - vectorToTarget * _backoffFactor * (_standoffDistance - distance);
                _agent.SetDestination(_destination);

                //Debug.Log("Setting backup dest");
            }
        }


        if (distance <= 2 * _standoffDistance)
        {
            _agent.updateRotation = false;
            RotateTowardTarget(deltaTime);

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
    }


    private void RotateTowardTarget(float deltaTime)
    {
        _targetDirection = _target.position - transform.position;
        _targetDirection.y = 0f;

        if (_targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotateSpeed * deltaTime);
        }
    }


	private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;

        Vector3 pos = transform.position;
        //pos.y += 2f;
        Gizmos.DrawLine(pos, pos + _agent.velocity);


        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, pos + transform.forward);

        //Gizmos.DrawSphere(_destination, 0.1f);
    }

}
