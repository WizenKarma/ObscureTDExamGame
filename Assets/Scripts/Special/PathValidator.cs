using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PathValidator : MonoBehaviour {

    private Seeker seeker;
    public Transform[] waypoints;
    private Path path;
    bool valid = false;
    bool searching = false;

	// Use this for initialization
	void Start () {
        seeker = this.GetComponent<Seeker>();
        GameObject waypointParent = GameObject.FindGameObjectWithTag("Waypoint");
        waypoints = waypointParent.GetComponentsInChildren<Transform>();
    }
    

    public bool isValid() {

        List<GraphNode> nodes = new List<GraphNode>();

        for(int i=1;i<waypoints.Length-1; i++)
        {
            nodes.Add(AstarPath.active.GetNearest(waypoints[i].position).node);
        }
        if (PathUtilities.IsPathPossible(nodes))
            return true;
        else
            return false;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
