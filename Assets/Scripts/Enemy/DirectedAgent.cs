using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class DirectedAgent : MonoBehaviour
{

    public GameObject target;
    public float updateFrequency = 0.1f;
    public float animationMovingThreshold = 0.1f;

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