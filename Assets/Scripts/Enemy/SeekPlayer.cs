using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SeekPlayer : MonoBehaviour
{

    public LayerMask detectionLayer;
    public GameObject head;

    [Header("Debug Settings")]
    public int fovSegments;

    [Header("AI Settings")]
    public int detectionRadius;
    public int viewingAngle;

    // Start is called before the first frame update
    void Start()
    {
        head = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject;  // has to be a better way to do this
    }

    void Update()
    {
        HandleDetection();

        if (Application.isEditor)
        {
            Vector3 modifiedForward = head.transform.forward;
            modifiedForward.y = 0.0f;
            modifiedForward.Normalize();
            var line = (modifiedForward * detectionRadius); // TODO fix this as its still not quite right
            float incrimentSize = (viewingAngle * 2) / fovSegments;
            var minLine = head.transform.position + (Quaternion.AngleAxis(-viewingAngle, transform.up) * line);
            var maxLine = head.transform.position + (Quaternion.AngleAxis(viewingAngle, transform.up) * line);
            for (int i = 0; i < fovSegments; i++)
            {
                float currentIncriment = (incrimentSize*i) - viewingAngle;
                var currentLine = head.transform.position + (Quaternion.AngleAxis(currentIncriment, transform.up) * line);
                var nextLine = head.transform.position + (Quaternion.AngleAxis(currentIncriment+incrimentSize, transform.up) * line);
                Debug.DrawLine(currentLine, nextLine, Color.red);
            }
            // final connecting line (bit of a cheat)
            float finalPoint = (incrimentSize * (fovSegments-1)) - viewingAngle;
            var finalLine = head.transform.position + (Quaternion.AngleAxis(finalPoint + incrimentSize, transform.up) * line);
            Debug.DrawLine(finalLine, maxLine, Color.red); 

            Debug.DrawLine(head.transform.position, minLine, Color.red);
            Debug.DrawLine(head.transform.position, maxLine, Color.red);
        }
    }

    // Update is called once per frame
    void HandleDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider currentHit = colliders[i];

            if (currentHit.gameObject.tag == "Player")
            {
                Vector3 targetDirection = currentHit.transform.position - head.transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, head.transform.forward);

                if (viewableAngle > -viewingAngle && viewableAngle < viewingAngle)  // does the monster see it 
                {
                   // Debug.Log("DETECTED PLAYER");  // TODO ensure that there is not a wall between the player and this object (raycast)
                }
            }
        }
    }
}