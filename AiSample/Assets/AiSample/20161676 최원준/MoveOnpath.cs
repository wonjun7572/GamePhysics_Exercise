using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnpath : MonoBehaviour
{
    public Path_OBJ pathTofollow;

    public int CurrentWayPointID = 0;
    public float _maxSpeed = 5.0f;
    private float reachDistance = 1.0f;
    public float rotationSpeed = 5.0f;
    public string pathName;

    private Vector3 _velocity = Vector3.zero;

    Vector3 last_position;
    Vector3 current_position;

    // Start is called before the first frame update
    void Start()
    {
        last_position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(pathTofollow.path_objs [CurrentWayPointID].position, transform.position);

        if (distance <= reachDistance)
        {
            CurrentWayPointID++;
        }

        if(CurrentWayPointID >= pathTofollow.path_objs.Count)
        {
            CurrentWayPointID = 0;
        }
        
        _velocity = seek(pathTofollow.path_objs[CurrentWayPointID].position) * Time.deltaTime * 2;

        transform.position = transform.position + _velocity;

    }
    
    private Vector3 seek(Vector3 target_pos)
    {
        // 방향 변경을 위함.
        Vector3 distance = pathTofollow.path_objs[CurrentWayPointID].position - transform.position;
      
        if (distance.sqrMagnitude > 0.005f)
        {
            transform.forward = distance.normalized;
        }

        Vector3 desired_velocity = distance.normalized * _maxSpeed;

        return (desired_velocity - _velocity);
    }

  

}
