using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonBlockEventManager : MonoBehaviour
{
    public Dictionary<string, GameObject> children = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            GameObject currentChild = child.gameObject;
            children[currentChild.name] = currentChild;
        }
    }

    public void TriggerEvent(EventData data)
    {
        GameObject eventManager;
        if (!children.TryGetValue(data.key, out eventManager))
        {
            Debug.Log($"Could not find event handler for {data.key}");
            return;
        }

        eventManager.GetComponent<ITriggerableEvent>().TriggerEvent(data);  // dispatch the event data to the respective event manager
    }
}
