using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_pursuit : MonoBehaviour
{
    [SerializeField]
    private Transform _target = null;

    [SerializeField]
    private float _maxSpeed = 0.01f;

    private bool _isPursuit = false;

    private Vector3 _velocity = Vector3.zero;
     public Vector3 AgentVelocity
    {
        get { return _velocity; }
    }


    RaycastHit hit;

    private Vector3 SteeringForce = Vector3.zero;

    float MaxDistance = 0.1f;
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _isPursuit = true;
        }

        if (Input.GetKey(KeyCode.V))
        {
            _isPursuit = false;
        }
        if (_isPursuit)
        {
            // pursuit
            // 3: 적당히 빠르게 해주기 위함.
            _velocity = _velocity + pursuit(_target) + Wall_Avoidance();

            // 속도를 기반으로 새로운 위치 계산.
            transform.position = transform.position + _velocity;
        }
    }

    private Vector3 pursuit(Transform target_agent)
    {
        return seek(target_agent.position);
    }

    private Vector3 seek(Vector3 target_pos)
    {
        // 방향 변경을 위함.
        Vector3 distance = target_pos - transform.position;
        if (distance.sqrMagnitude > 0.005f)
        {
            transform.forward = distance.normalized;
        }

        Vector3 desired_velocity = distance.normalized * _maxSpeed;

        return (desired_velocity - _velocity);
    }

    private Vector3 Wall_Avoidance()
    {

        Vector3 m_feeler = transform.position + (transform.forward * MaxDistance);


        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance))
        {
            Vector3 overShoot = m_feeler - hit.point;             //촉수-지점
            SteeringForce = hit.normal * overShoot.magnitude;  //조종힘 계산

            return SteeringForce;

        }

        if (Physics.Raycast(transform.position, transform.right, out hit, MaxDistance))
        {
            Vector3 overShoot = m_feeler - hit.point;
            SteeringForce = hit.normal * overShoot.magnitude;
        }

        if (Physics.Raycast(transform.position, -transform.right, out hit, MaxDistance))
        {
            Vector3 overShoot = m_feeler - hit.point;
            SteeringForce = hit.normal * overShoot.magnitude;
        }

        return Vector3.zero;

    }

}
