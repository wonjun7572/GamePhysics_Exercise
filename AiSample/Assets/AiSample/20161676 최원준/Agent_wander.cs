using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Agent_wander : MonoBehaviour
{
    [SerializeField]

    private Vector3 _velocity = Vector3.zero;
    private Vector3 _pickPos = Vector3.zero;

    [SerializeField]
    public static float m_dWanderRadius = 5.0f;
    
    [SerializeField]
    public static float m_dWanderJitter = 30.0f;


    RaycastHit hit;

    [SerializeField]
    float MaxDistance = 2.0f;

    private Vector3 SteeringForce = Vector3.zero;

    Vector3 m_vWanderTarget = new Vector3(m_dWanderRadius * Mathf.Cos(0), 0, m_dWanderRadius * Mathf.Sin(0));

    void Update()
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

        _velocity = Wall_Avoidance() + Wander() * Time.deltaTime;
        transform.position = transform.position + _velocity;

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

   private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_pickPos, 1.0f);
    }

}