using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingHead : MonoBehaviour
{
    public GameObject toRotate;
    public Vector3 axis = Vector3.up;
    public float radius;
    public float rotationSpeed = 80.0f;
    public float headHeight = 1.7f;
    public float maxHeadTurn = 50;

    private Vector3 desiredPosition;
    private int direction = 1;
    public float radiusSpeed = 0.5f;

    void Start()
    {
        toRotate.transform.position = (toRotate.transform.position - transform.position).normalized * radius + transform.position;
    }

    void Update()
    {
        // should I change direction
        Vector3 targetDirection = toRotate.transform.position - transform.position;
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
        if (viewableAngle > 50)
        {
            var line = (transform.forward * radius);
            var minLine = toRotate.transform.position + (Quaternion.AngleAxis(viewableAngle*direction, transform.up) * line);
            direction = direction == 1 ? -1 : 1;
        }
        toRotate.transform.RotateAround(transform.position, axis, (rotationSpeed * direction) * Time.deltaTime);
        desiredPosition = (toRotate.transform.position - transform.position).normalized * radius + transform.position;
        desiredPosition.y = headHeight;
        toRotate.transform.position = Vector3.MoveTowards(toRotate.transform.position, desiredPosition, radiusSpeed);
    }
}
