using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offset : MonoBehaviour
{
    public GameObject offsetPursueTarget = null;

    Vector3 offset;

    public Vector3 force;

    public float mass = 1.0f;

    Vector3 offsetPursueTargetPos;

    private float _maxSpeed = 3.0f;
    public float MaxSpeed
    {
        get { return _maxSpeed; }
    }
    public Vector3 velocity;

    [SerializeField]
    public static float m_dWanderRadius = 5.0f;
    public static float m_dWanderJitter = 30.0f;

    [SerializeField]
    private float _deceleration = 2.0f;

    private Vector3 _velocity = Vector3.zero;

    public float maxForce = 10.0f;

    RaycastHit hit;

    [SerializeField]
    float MaxDistance = 2.0f;

    private Vector3 SteeringForce = Vector3.zero;

    Vector3 m_vWanderTarget = new Vector3(m_dWanderRadius * Mathf.Cos(0), 0, m_dWanderRadius * Mathf.Sin(0));
    private void Start()

    {
        offset = transform.position - offsetPursueTarget.transform.position;

        offset = Quaternion.Inverse(offsetPursueTarget.transform.rotation) * offset;

        m_vWanderTarget = Random.insideUnitSphere * m_dWanderRadius;

    }

    private void Update()
    {
        force = Vector3.ClampMagnitude(force, maxForce);

        Vector3 acceleration = force / mass;

        velocity += acceleration * Time.deltaTime;

        velocity = Vector3.ClampMagnitude(velocity, MaxSpeed);

        if (velocity.magnitude > float.Epsilon)

        {

            transform.forward = velocity;

        }

        velocity = OffsetPursue(offsetPursueTarget, offset) + Wall_Avoidance();

        transform.position += velocity * Time.deltaTime * 3;



    }

    public Vector3 OffsetPursue(GameObject leader, Vector3 offset)

    {
        Vector3 target = leader.transform.TransformPoint(offset);

        Vector3 toTarget = transform.position - target;

        float dist = toTarget.magnitude;

        float lookAhead = dist / _maxSpeed;

        offsetPursueTargetPos = target + (lookAhead * leader.GetComponent<Agent_pursuit>().AgentVelocity);

        return arrive(offsetPursueTargetPos);

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
