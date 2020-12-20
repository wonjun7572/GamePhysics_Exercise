using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour
{
    public GameObject HideObject;

    public GameObject pursuer;
    // Update is called once per frame

    private float _maxSpeed = 3.0f;

    [SerializeField]
    private float _deceleration = 2.0f;

    private Vector3 _velocity = Vector3.zero;

    [SerializeField]
    public static float m_dWanderRadius = 5.0f;

    [SerializeField]
    public static float m_dWanderJitter = 30.0f;

    public float hide_obj_width = 2.0f;
    public float hide_from_obj = 3.0f;

    Vector3 m_vWanderTarget = new Vector3(m_dWanderRadius * Mathf.Cos(0), 0, m_dWanderRadius * Mathf.Sin(0));

    void Update()
    {

        float distance = Vector3.Distance(transform.position, HideObject.transform.position);

        if (distance > 1.0f)
        {
            _velocity = _velocity + HideOn(HideObject, pursuer);
        }
        else
        {
            _velocity = _velocity + Wander();
        }
        transform.position = transform.position + _velocity * Time.deltaTime * 3;
    }

    Vector3 GetHidingPosition(GameObject obj, GameObject pursuer)
    {
        Vector3 awayFromPursuer = Vector3.Normalize(obj.transform.position - pursuer.transform.position);
        return ((hide_obj_width + hide_from_obj) * obj.transform.localScale.x / 2) * awayFromPursuer + obj.transform.position;

    }
    
    private Vector3 HideOn(GameObject hide_obj, GameObject pursuer)
    {
        Vector3 hidePos = GetHidingPosition(hide_obj, pursuer);
        return arrive(hidePos);
    }

    private Vector3 Wander()
    {
        float JitterThisTimeSlice = m_dWanderJitter * Time.deltaTime;

        m_vWanderTarget += new Vector3(Random.Range(-5.0f, 5.0f) * JitterThisTimeSlice,
                                   0, Random.Range(-5.0f, 5.0f) * JitterThisTimeSlice);

        m_vWanderTarget.Normalize();
        m_vWanderTarget = m_vWanderTarget * m_dWanderRadius;

        Vector3 target = m_vWanderTarget + (transform.forward * 3.0f);

        Vector3 Target = target + transform.position;

        transform.forward = Target - transform.position;

        return Target - transform.position;
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
}
