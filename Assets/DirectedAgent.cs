using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class DirectedAgent : MonoBehaviour
{

    private NavMeshAgent agent;

    // Use this for initialization
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.destination = new Vector3(8.5f, 0f, 7.5f);
    }

    public void MoveToLocation(Vector3 targetPoint)
    {
        agent.destination = targetPoint;
        agent.isStopped = false;
    }
}