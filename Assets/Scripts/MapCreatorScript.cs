using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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

    private GameObject[,] placedItems;
    
    void Start()
    {
        if (!System.String.IsNullOrEmpty(json)) {
            CreateMap(json);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ClearMap() {
        foreach (Transform child in this.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void CreateMap(string jsonMap) {
        ClearMap();
        map = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<Block>>>(jsonMap);
        placedItems = new GameObject[map.Count, map[0].Count];

        for (int i = 0; i < map.Count; i++)
        {
            for (int j = 0; j < map[i].Count; j++)
            {
                Block currentBlock = map[i][j];
                Coordinate placeCoords = new Coordinate(i, j);
                if (currentBlock != null)  // if there is a block 
                {
                    if (placedItems[i, j] == null)  // do we need to create a new block at the parent coords?
                    {
                        try
                        {  // if it fails just skip this coord
                           // does it have a parent
                            if (currentBlock.parent != null)
                            {
                                placeCoords = currentBlock.parent;  // set the coords to the parents coord
                            }
                            GameObject newObject = Instantiate(Resources.Load(currentBlock.type.key) as GameObject, new Vector3(placeCoords.row, 0, placeCoords.col), Quaternion.identity, this.transform);
                            double rotation = -currentBlock.rotation * (180 / System.Math.PI);
                            newObject.transform.Rotate(0, (float)rotation, 0);
                            Dimensions rotatedDimensions = GetRotatedDimensions(currentBlock.rotation, currentBlock.type.dimensions);
                            for (int x = placeCoords.col; x != placeCoords.col + rotatedDimensions.width; x += rotatedDimensions.width > 0 ? 1 : -1)
                            {
                                for (int y = placeCoords.row; y != placeCoords.row + rotatedDimensions.height; y += rotatedDimensions.height > 0 ? 1 : -1)
                                {
                                    placedItems[x, y] = newObject;  // set the object as being placed already
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.Log(e);  // throwing system out of bound exceptions but I have no idea why are its 
                        }
                    }

                    // check for any shelf items
                    if (currentBlock.shelfItems != null)
                    {
                        foreach (string key in currentBlock.shelfItems)
                        {
                            try
                            {
                                GameObject shelfItem = Instantiate(Resources.Load(key) as GameObject, new Vector3(placeCoords.row, 2.5f, placeCoords.col), Quaternion.identity, this.transform);  // place at the top of the world so it can drop to the 
                            }
                            catch (System.Exception e)
                            {
                                Debug.Log(e);  // throwing system out of bound exceptions but I have no idea why are its 
                            }
                        }
                    }
                }
            }
        }

        // build the mesh for the map
        gameObject.GetComponentInParent<NavigationBaker>().BuildNavMesh();

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

    public void TriggerEvent(int[] coords) {
        try {
            GameObject clickedGameObject = placedItems[coords[0], coords[1]];
            ITriggerableBlock triggerableBlock = clickedGameObject.GetComponent<ITriggerableBlock>();
            triggerableBlock.Trigger();
        } 
        catch (System.Exception e) {
            Debug.Log("Could not trigger event");
            Debug.Log(e);  // throwing system out of bound exceptions but I have no idea why are its 
        }
    }
}
