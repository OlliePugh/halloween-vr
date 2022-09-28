using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public struct ObjectMapper {
    public string name;
    public GameObject gameObject;
}


public class MapCreatorScript : MonoBehaviour
{

    public string json;
    public List<List<Block>> map;
    public ObjectMapper[] objectMapping;

    private Dictionary<string, GameObject> prefabMapper = new Dictionary<string, GameObject>();
    private GameObject[,] placedItems;
    
    void Start()
    {

        foreach (var gameObjectMapping in objectMapping)
        {
            prefabMapper.Add(gameObjectMapping.name, gameObjectMapping.gameObject);
        }

        map = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<Block>>>(json);
        placedItems = new GameObject[map.Count, map[0].Count];

        for (int i = 0; i < map.Count; i++)
        {
            for (int j = 0; j < map[i].Count; j++)
            {
                Block currentBlock = map[i][j];
                if (currentBlock != null && placedItems[i,j] == null) {
                    try {  // if it fails just skip this coord
                        GameObject newObject = Instantiate(prefabMapper[currentBlock.type.key], new Vector3(j,0,i), Quaternion.identity);
                        double rotation = currentBlock.rotation * (180/System.Math.PI);
                        newObject.transform.Rotate(0,(float)rotation,0);
                        Dimensions rotatedDimensions = GetRotatedDimensions(currentBlock.rotation, currentBlock.type.dimensions);
                        for (int x = i; x < i + Mathf.Abs(rotatedDimensions.width); x++)
                        {
                            for (int y = j; y < j+Mathf.Abs(rotatedDimensions.height); y++) {
                                placedItems[x,y] = newObject;  // set the object as being placed already
                            }
                        }
                    }
                    catch (System.Exception e) {
                        Debug.Log(e);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    Dimensions GetRotatedDimensions(float rotation, Dimensions dimensions) {
        int[,] rotationMatrix = {{(int)Mathf.Round((float)System.Math.Cos(rotation)), (int)Mathf.Round((float)System.Math.Sin(rotation))},
                  {(int)-Mathf.Round((float)System.Math.Sin(rotation)), (int)Mathf.Round((float)System.Math.Cos(rotation))}};

        // Debug.Log(rotationMatrix[0,0]);

        Dimensions rotatedDimensions = new Dimensions();
        rotatedDimensions.width = rotationMatrix[0,0] * dimensions.width + rotationMatrix[0,1] * dimensions.height;
        rotatedDimensions.height = rotationMatrix[1,0] * dimensions.width + rotationMatrix[1,1] * dimensions.height;

        return rotatedDimensions;
    }
}
