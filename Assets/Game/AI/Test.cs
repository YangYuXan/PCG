using System.Collections;
using System.Collections.Generic;
using Unity.AI;
using Unity.AI.Navigation;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject nav;
    public NavMeshSurface sfmesh;
    public int x;
    public int y;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                Instantiate(nav, new Vector3(i, 0, j), new Quaternion());
            }
        }

        sfmesh.BuildNavMesh();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
