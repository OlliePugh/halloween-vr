using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BpmWatch : MonoBehaviour
{
    public TextMeshPro textMesh;
    
    GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        textMesh.SetText(gameController.bpm.ToString());
    }
}
