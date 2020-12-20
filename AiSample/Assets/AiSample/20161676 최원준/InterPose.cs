using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterPose : MonoBehaviour
{
  
    public Offset_pursuit off_1;
    public Offset_pursuit off_2;

    [SerializeField]
    private float _maxSpeed = 0.01f;

    [SerializeField]
     private Vector3 _velocity = Vector3.zero;


    RaycastHit hit;

    [SerializeField]
    float MaxDistance = 2.0f;

    private Vector3 SteeringForce = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        _velocity = _velocity + Interpose(off_1, off_2) + Wall_Avoidance();

        transform.position = transform.position + _velocity * Time.deltaTime;
    }
    
    public Vector3 Interpose(Offset_pursuit off_1, Offset_pursuit off_2)
    {
        Vector3 midPoint = (off_1.transform.position + off_2.transform.position) / 2;

        float timeToReachMidPoint = Vector3.Distance(transform.position, midPoint) / _maxSpeed;

        Vector3 futureTarget1Pos = off_1.transform.position + off_1.AgentVelocity * timeToReachMidPoint;
        Vector3 futureTarget2Pos = off_2.transform.position + off_2.AgentVelocity * timeToReachMidPoint;

        midPoint = (futureTarget1Pos + futureTarget2Pos) / 2;

        return seek(midPoint);
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
