using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectKeyEvent : MonoBehaviour
{
    // Start is called before the first frame update
    public void CollectKey()
    {
        GameObject.Find("GameController").GetComponent<GameController>().CollectKey();
    }
}
