using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GuardScript : MonoBehaviour
{
    public Transform pathHolder;
    public float guardSpeed = 4;
    public float waitTime = 0.3f;
    public float turnSpeed = 90f;

    public void Start()
    {
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < pathHolder.childCount; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints, guardSpeed));
    }

    void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.2f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);
    }



    IEnumerator FollowPath(Vector3[] waypoints, float speed)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
               
                yield return StartCoroutine(LookTowards(targetWaypoint));
                
            }

            yield return null;
        }
    }

    IEnumerator LookTowards(Vector3 Target)
    {
        Vector3 lookDirn = (Target - transform.position).normalized;
        float targetAngle = Mathf.Atan2(lookDirn.z, lookDirn.x) * Mathf.Rad2Deg;

        while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }

    }
}
