using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallWithPaintingTrigger : MonoBehaviour, ITriggerableBlock
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Trigger() {
        Debug.Log("I have been triggered");
    }
}
