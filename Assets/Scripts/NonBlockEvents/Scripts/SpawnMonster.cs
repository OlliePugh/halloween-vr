using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonster : MonoBehaviour, ITriggerableEvent
{
    public GameObject pumkinMonster;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerEvent(EventData eventData)
    {
        Vector3 location = new Vector3(eventData.location[0], 0, eventData.location[1]);
        Instantiate(pumkinMonster, location, Quaternion.identity);
    }
}
