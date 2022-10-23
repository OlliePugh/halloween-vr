using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonBlockEventManager : MonoBehaviour
{
    public Dictionary<string, ITriggerableEvent> children = new Dictionary<string, ITriggerableEvent>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            GameObject currentChild = child.gameObject;
            ITriggerableEvent _event = currentChild.GetComponent<ITriggerableEvent>();
            children[currentChild.name] = _event;
        }
    }

    public void TriggerEvent(EventData data)
    {
        ITriggerableEvent eventManager;
        if (!children.TryGetValue(data.key, out eventManager))
        {
            Debug.Log($"Could not find event handler for {data.key}");
            return;
        }


        eventManager.TriggerEvent(data);
        StartCoroutine(WaitToKill(data.duration, eventManager));
      // dispatch the event data to the respective event manager
    }

    IEnumerator WaitToKill(int duration, ITriggerableEvent _event)
    {
        yield return new WaitForSeconds(duration);
        _event.EndEvent();
    }

    public void CleanAll()  // end all events
    {
        foreach (KeyValuePair<string, ITriggerableEvent> currentEvent in children)
        {
            currentEvent.Value.EndEvent();  // end the event
        }
    }

}
