using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class DirectedAgent : MonoBehaviour
{

    public GameObject target;

    private NavMeshAgent agent;
    private LookAtConstraint headLookAt;

    // Use this for initialization
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        headLookAt = GetComponentInChildren<LookAtConstraint>();
        if(target)
        {
            SetTarget(target);  // set the starting target
        }
    }

    void Update()
    {
        // this might need to be set to a certain hz instead of every frame
        if (target)  // set the destination as the player
        {
            this.SetDestination(target.transform.position);
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = newTarget.transform;
        constraintSource.weight = 1;

        headLookAt.SetSources(new List<ConstraintSource> { constraintSource });
    }

    public void SetDestination(Vector3 targetPoint)
    {
        agent.destination = targetPoint;
        agent.isStopped = false;
    }
}