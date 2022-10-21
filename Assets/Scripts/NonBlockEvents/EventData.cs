using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventData
{
    public string key;
    #nullable enable
    public int[] location;  // can be null
    public int duration;
    #nullable disable
}