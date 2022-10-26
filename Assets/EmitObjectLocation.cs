using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitObjectLocation : MonoBehaviour
{
    public int rate;
    public string name;

    private GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        StartCoroutine("SendLocationRouting");
    }

    // Update is called once per frame
    IEnumerator SendLocationRouting()
    {
        for (;;)  // forever
        {
            gameController.EmitObjectLocation(name, this.transform.position);
            yield return new WaitForSeconds(1/(float) rate);
        }
    }
}
