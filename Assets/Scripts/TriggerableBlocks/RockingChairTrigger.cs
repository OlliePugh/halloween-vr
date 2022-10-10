using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockingChairTrigger : MonoBehaviour, ITriggerableBlock
{

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Trigger() {
        Debug.Log("triggered");
        StartCoroutine (RockChair());
    }

    
    IEnumerator RockChair ()
    {
        animator.SetBool("IsRocking", true);
 
        yield return new WaitForSeconds (5f);  // halts for 5 seconds
 
        animator.SetBool("IsRocking", false);
     }
}

