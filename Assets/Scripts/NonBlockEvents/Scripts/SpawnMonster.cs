using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonster : MonoBehaviour, ITriggerableEvent
{
    public GameObject pumkinMonster;

    public GameObject currentSpawnedObject;

    public void TriggerEvent(EventData eventData)
    {
        Vector3 location = new Vector3(eventData.location[1], 0, eventData.location[0]);
        currentSpawnedObject = Instantiate(pumkinMonster, location, Quaternion.identity);
    }

    public void EndEvent()
    {
        if (currentSpawnedObject)
        {
            Destroy(currentSpawnedObject);
            currentSpawnedObject = null;
        }
    }
}
