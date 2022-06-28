using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//interface for objects that use navmeshAgent
public interface IAgent
{
    GameObject body { get; set; }
    NavMeshAgent agent { get; set; }
    Vector3 currentTarget { get; set; }

    Vector3[] patrolPath { get; set; }

    //bool AgentUpdate();
}
