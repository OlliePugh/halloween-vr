using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SeekPlayer : MonoBehaviour
{

    public LayerMask detectionLayer;
    public GameObject head;

    public DirectedAgent directedAgent;
    public GameController gameController;

    public float killDistance = 0.5f;

    [Header("Debug Settings")]
    public int fovSegments;

    [Header("AI Settings")]
    public int defaultDetectionRadius;
    public int defaultViewingAngle;

    [Header("BPM Impact")]
    [Range(0, 3)]
    public float upperBpmMultiplier = 2f;
    [Range(0, 3)]
    public float lowerBpmMultiplier = 0.5f;

    private int detectionRadius;
    private int viewingAngle;
    // Start is called before the first frame update
    void Start()
    {
        head = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject;  // has to be a better way to do this
        directedAgent = GetComponent<DirectedAgent>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        float bpmSkew = (float) (gameController.bpm - GameController.MIN_BPM) / (float) (GameController.MAX_BPM - GameController.MIN_BPM);
        float bpmMultiplier = (bpmSkew * (upperBpmMultiplier - lowerBpmMultiplier)) + lowerBpmMultiplier; // maps between 
        detectionRadius = (int) (defaultDetectionRadius * bpmMultiplier);  // 100bpm = *1 | 50bpm = *0.5 | 200 bpm = *2
        viewingAngle = (int) (defaultViewingAngle * bpmMultiplier);
        HandleDetection();

        if (Application.isEditor)
        {
            Vector3 modifiedForward = head.transform.forward;
            modifiedForward.y = 0.0f;
            modifiedForward.Normalize();
            var line = (modifiedForward * detectionRadius);
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
                    RaycastHit hit;
                    if (Physics.Raycast(head.transform.position, targetDirection, out hit, Vector3.Distance(head.transform.position, currentHit.transform.position), ~( detectionLayer.value | (1<<2) ) ))  // everything other than the player mask
                    {
                        // hit a wall
                        //Debug.DrawLine(head.transform.position, hit.point, Color.red);
                        directedAgent.SetTarget(null);
                    }
                    else  // nothing inbetween the player
                    {
                        Debug.DrawLine(head.transform.position, currentHit.transform.position, Color.green);
                        directedAgent.SetTarget(currentHit.gameObject);

                        // is the distance very low?
                        if(Vector3.Distance(this.transform.position, currentHit.transform.position) < killDistance)
                        {
                            gameController.KillPlayer();
                        }
                    }
                }
            }
        }
    }
}