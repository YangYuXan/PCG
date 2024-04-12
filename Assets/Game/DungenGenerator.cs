using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungenGenerator : MonoBehaviour
{
    int[,] map;

    [Range(0, 100)]
    public int randomFillPercent;
    public int width, height;
    public string seed;
    
    public bool useRandom;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
        RandomFillMap();
    }


    void GenerateMap()
    {
        map = new int[width, height];
    }

    void RandomFillMap()
    {
        if (useRandom)
        {
            seed = Time.time.ToString();
        }
        System.Random prng = new System.Random(seed.GetHashCode());

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                map[x, y] = prng.Next(0, 100)<randomFillPercent?1:0;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //Gizmos.color = (map[x, y] == 1) ? Color.color.black : Color.color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                    print("tttt");
                }
            }
        }
    }
}
