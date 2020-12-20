using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private Vector3 _pickPos = Vector3.zero;

    [SerializeField]
    private float _maxSpeed = 1.5f;

     [SerializeField]
    private float _deceleration = 2.0f;

    private Vector3 _velocity = Vector3.zero;

    RaycastHit hit;
    public float MaxSpeed
    {
        get { return _maxSpeed; }
    }
    
    public Vector3 AgentVelocity
    {
        get { return _velocity; }
    }

    [SerializeField]
    float MaxDistance = 2.0f;

    private Vector3 SteeringForce = Vector3.zero;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mouse_pos = Input.mousePosition;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenToWorldPoint(mouse_pos), -Vector3.up, out hit, 1000))
            {
                _pickPos = hit.point;
                _pickPos.y = 0.0f;
            }
        }

        // 3: 적당히 빠르게 해주기 위함.
        _velocity = _velocity + arrive(_pickPos) + Wall_Avoidance();

        // 속도를 기반으로 새로운 위치 계산.
        transform.position = transform.position + _velocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_pickPos, 1.0f);
    }

    private Vector3 arrive(Vector3 target_pos)
    {
        float distance = Vector3.Distance(target_pos, transform.position);

        if (distance > 0.0f)
        {
            Vector3 to_target = target_pos - transform.position;

            if (to_target.sqrMagnitude > 0.005f)
            {
                transform.forward = to_target.normalized;
            }

            float _speed = distance / _deceleration;

            // 최대 속도로 제한.
            _speed = Mathf.Min(_speed, _maxSpeed);

            Vector3 desired_velocity = to_target / distance * _speed;

            return (desired_velocity - _velocity);
        }

        return Vector3.zero;
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