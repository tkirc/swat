using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
public class FollowWaypoints : NetworkBehaviour
{

    //FollowPathBehavior
    public Transform[] waypoints;
    public bool followByRandom = false;
    int m_CurrentWaypointIndex = 0;

    public Vector3 NextWayPoint()
    {
        if (followByRandom)
        {
            m_CurrentWaypointIndex = Random.Range(0, waypoints.Length - 1);
        }
        else
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;

        }
        return waypoints[m_CurrentWaypointIndex].position;
    }

    public Vector3 returnToLastWayPoint()
    {
        return waypoints[m_CurrentWaypointIndex].position;
    }



}
