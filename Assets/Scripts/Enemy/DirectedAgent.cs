using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class DirectedAgent : MonoBehaviour
{

    public GameObject target;
    public float updateFrequency = 0.1f;
    public float animationMovingThreshold = 0.1f;

    [Header("Patrol Settings")]
    public float patrolRange;
    public GameObject patrolLookAt;

    private Vector3 lastSeenLocation = Vector3.negativeInfinity;
    private float nextActionTime = 0.0f;
    private NavMeshAgent agent;
    private LookAtConstraint headLookAt;
    private Vector3 lastPosition;
    private Animator animator;

    // Use this for initialization
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        headLookAt = GetComponentInChildren<LookAtConstraint>();
        if(target)
        {
            SetTarget(target);  // set the starting target
        }
    }

    void Update()
    {
        // this might need to be set to a certain hz instead of every frame
        if (Time.time > nextActionTime)
        {
            nextActionTime += updateFrequency;
            if (target)  // set the destination as the player
            {
                this.SetDestination(target.transform.position);
            } else if (lastSeenLocation != Vector3.negativeInfinity)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    lastSeenLocation = Vector3.negativeInfinity; // the enemy has reached the last seen 
                } 
            }

            Vector3 currentPosition = transform.position;
            if (Vector3.Distance(currentPosition, lastPosition) > animationMovingThreshold)  // is moving
            {
                animator.SetBool("move", true);
            } else
            {
                animator.SetBool("move", false);
            }
            lastPosition = currentPosition;
        }

        if (agent.remainingDistance <= agent.stoppingDistance) //done with path
        {
            Vector3 point;
            if (RandomPoint(this.transform.position, patrolRange, out point)) { //pass in our centre point and radius of area
                agent.SetDestination(point);
            }
        }

        Debug.DrawLine(transform.position, agent.destination, Color.blue);
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)  // cheers JonDevTutorial for this <3
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    public void SetTarget(GameObject newTarget)
    {
        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.weight = 1;
        if(newTarget)
        {
            constraintSource.sourceTransform = newTarget.transform;
            headLookAt.SetSources(new List<ConstraintSource> { constraintSource });
        }
        else
        {
            this.SetDestination(lastSeenLocation);
            constraintSource.sourceTransform = patrolLookAt.transform;
            headLookAt.SetSources(new List<ConstraintSource> { constraintSource });
            if (target)  // if there was a target already
            {
                lastSeenLocation = target.transform.position;  // set it as the last seen position
            }
        }

        target = newTarget;
    }

    public void SetDestination(Vector3 targetPoint)
    {
        agent.destination = targetPoint;
        agent.isStopped = false;
    }
}