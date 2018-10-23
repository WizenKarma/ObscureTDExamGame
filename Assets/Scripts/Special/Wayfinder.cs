using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;

public class Wayfinder : MonoBehaviour {

    public Transform targetPosition;

    private Seeker seeker;
    private CharacterController controller;

    public Path path;

    public float speed = 2;

    public float nextWaypointDistance = 3;

    private int currentWaypoint = 0;

    public bool reachedEndOfPath;

    private bool searchingForPath;

    public float repathRate = 1;
    public float elapsedSinceRepath = 0f;

    public List<Transform> waypoints = new List<Transform>();

    int waypointIndex = 0;
    public void Start()
    {
        seeker = GetComponent<Seeker>();
        // If you are writing a 2D game you can remove this line
        // and use the alternative way to move sugggested further below.
        controller = GetComponent<CharacterController>();
        GameObject waypointParent = GameObject.FindGameObjectWithTag("Waypoint");
        waypoints = waypointParent.GetComponentsInChildren<Transform>().ToList();
        waypoints.Remove(waypointParent.transform);
        targetPosition = waypoints[0];
        // Start a new path to the targetPosition, call the the OnPathComplete function
        // when the path has been calculated (which may take a few frames depending on the complexity)
        seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
        searchingForPath = true;
        waypointIndex = 0;
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
            searchingForPath = false;
        }
    }

    public void Update()
    {
        if (path == null)
        {
            // We have no path to follow yet, so don't do anything
            return;
        }
        pathHandler(); // Checks if there is a path being searched for, if not then look for one after a set amount of time
        move();
    }

    private void pathHandler()
    {
        if (!searchingForPath)
        { // are we currently searching for a path?
            if (elapsedSinceRepath > repathRate)
            { // has enough time passed?
                seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
                searchingForPath = true;
                elapsedSinceRepath = 0f;
            }
            elapsedSinceRepath += Time.deltaTime; // only counts when not searching for a path
        }
    }

    private void move()
    {
        // Check in a loop if we are close enough to the current waypoint to switch to the next one.
        // We do this in a loop because many waypoints might be close to each other and we may reach
        // several of them in the same frame.
        reachedEndOfPath = false;
        // The distance to the next waypoint in the path
        float distanceToWaypoint;
        while (true)
        {
            // If you want maximum performance you can check the squared distance instead to get rid of a
            // square root calculation. But that is outside the scope of this tutorial.
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                // Check if there is another waypoint or if we have reached the end of the path
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    // Set a status variable to indicate that the agent has reached the end of the path.
                    // You can use this to trigger some special code if your game requires that.
                    reachedEndOfPath = true;
                    if (!searchingForPath)
                    {
                        waypointIndex++;
                        if (waypointIndex > waypoints.Count-1)
                        {
                            waypointIndex = 0;
                            this.transform.position = waypoints[waypointIndex].position;
                        }
                        targetPosition = waypoints[waypointIndex];
                        seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
                        searchingForPath = true;
                    }
                    break;
                }
            }
            else
            {
                break;
            }
        }

        // Slow down smoothly upon approaching the end of the path
        // This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

        // Direction to the next waypoint
        // Normalize it so that it has a length of 1 world unit
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        // Multiply the direction by our desired speed to get a velocity
        Vector3 velocity = dir * speed * speedFactor;

        // Move the agent using the CharacterController component
        // Note that SimpleMove takes a velocity in meters/second, so we should not multiply by Time.deltaTime
        controller.SimpleMove(velocity);

        // If you are writing a 2D game you may want to remove the CharacterController and instead use e.g transform.Translate
        // transform.position += velocity * Time.deltaTime;
    }
}
