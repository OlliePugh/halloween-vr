using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTorch : MonoBehaviour, ITriggerableEvent
{
    public GameObject flashlight;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        flashlight = GameObject.Find("Flashlight");
        animator = flashlight.GetComponentInChildren<Animator>();
    }

    public void TriggerEvent(EventData data)
    {
        animator.SetBool("FlashlightDisabled", true);
    }

    public void EndEvent()
    {
        animator.SetBool("FlashlightDisabled", false);
    }
}
