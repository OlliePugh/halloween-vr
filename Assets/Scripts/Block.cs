using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Block
{
    public Type type;
    #nullable enable
    public Coordinate? parent;  // can be null
    public string[] shelfItems;
    #nullable disable
    public float rotation;
}

[System.Serializable]
public class Coordinate {
    public int col;
    public int row;

    public Coordinate(int col, int row) {
        this.col = col;
        this.row = row;
    }
}

[System.Serializable]
public class Type {
    public Dimensions dimensions;
    public string key;
}

[System.Serializable]
public class Dimensions {
    public int height;
    public int width;
}