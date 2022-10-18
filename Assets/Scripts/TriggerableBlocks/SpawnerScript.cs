using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("XR Origin");
        player.transform.position = gameObject.transform.position;  // move the player to this position
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
