using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class NavigationBaker : MonoBehaviour
{
    public void BuildNavMesh()
    {
        this.GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
