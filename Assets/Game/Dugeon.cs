using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Dugeon : MonoBehaviour
{
    public enum terrainColor
    {
        black,
        white,
        orange,
        red,
        blue,
        yellow,
        green
    }

    public int height = 256;
    public int width = 256;
    public int smoothTimes=1;

    public NavMeshSurface navSurf;


    [Range(0, 100)]
    public int fillRate = 45;

    public GameObject BlackMark;
    public GameObject WhiteMark;
    public GameObject G1;
    public GameObject G2;
    public GameObject G3;
    public GameObject G4;

    public GameObject theft;
    public GameObject awardBox;
    public GameObject guard;

    public Dictionary<Vector3, int> mark = new Dictionary<Vector3, int>();
    public Dictionary<Vector3, float> weight = new Dictionary<Vector3, float>();
    public Dictionary<Vector3, terrainColor> t_color = new Dictionary<Vector3, terrainColor>();
    public Dictionary<Vector3, int> Cube_height = new Dictionary<Vector3, int>();

    public List<Vector3> whieCube = new List<Vector3>();
    public List<Vector3> Island = new List<Vector3>();

    private List<HashSet<Vector3>> regions;     // 存储连续区域的列表


    void Start()
    {

        regions = new List<HashSet<Vector3>>();

        Initial2DMap();


        for (int i = 0; i < smoothTimes; i++)
        {
            CalcRule();
            MotifyTerrain();
        }


        //Set List for white
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Vector3 cube = new Vector3(i, 0, j);
                if (t_color[new Vector3(i, 0, j)] == terrainColor.white)
                {
                    whieCube.Add(cube);
                }
            }
        }



        
        


        HashSet<Vector3> visited = new HashSet<Vector3>(); // 已访问过的坐标

        foreach (Vector3 start in whieCube)
        {
            if (!visited.Contains(start))
            {
                HashSet<Vector3> newRegion = PerformFloodFill(start, visited);
                regions.Add(newRegion);
            }
        }


        HashSet<Vector3> largestRegion = FindLargestRegion(regions);
        PrintNonLargestRegions(regions, largestRegion);
        Generate2D_Map();
        
        SpawnPawn();
        navSurf.BuildNavMesh();


        





        CalculateWall();


        mark.Clear();
        weight.Clear();
        t_color.Clear();
        Cube_height.Clear();

        print(mark.Count);
    }

    void Initial2DMap()
    {
        for (int i = 0; i < width; i++)
        {

            for (int j = 0; j < height; j++)
            {

                float randomValue = Random.Range(0f, 100f);

                Vector3 pos = new Vector3(i, 0, j);

                if (randomValue <= fillRate)
                {


                    mark.Add(pos, 8);
                    weight.Add(pos, randomValue);
                    t_color.Add(pos, terrainColor.white);
                }
                else
                {


                    mark.Add(pos, 8);
                    weight.Add(pos, randomValue);
                    t_color.Add(pos, terrainColor.black);
                }

                Cube_height.Add(pos, 0);
            }
        }
    }

    void CalcRule()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (weight.ContainsKey(new Vector3(i - 1, 0, j - 1))&&
                    t_color.ContainsKey(new Vector3(i - 1, 0, j - 1)))
                {
                    if(t_color[new Vector3(i - 1, 0, j - 1)]==terrainColor.white)
                    {
                        mark[new Vector3(i, 0, j)] = mark[new Vector3(i, 0, j)] - 1;
                    }

                }

                if (weight.ContainsKey(new Vector3(i, 0, j - 1))&&
                    t_color.ContainsKey(new Vector3(i, 0, j - 1)))
                {
                    if (t_color[new Vector3(i, 0, j - 1)] == terrainColor.white)
                    {
                        mark[new Vector3(i, 0, j)] = mark[new Vector3(i, 0, j)] - 1;
                    }

                }

                if (weight.ContainsKey(new Vector3(i + 1, 0, j - 1))&&
                    t_color.ContainsKey(new Vector3(i+1, 0, j - 1)))
                {
                    if(t_color[new Vector3(i + 1, 0, j - 1)] == terrainColor.white)
                    {
                        mark[new Vector3(i, 0, j)] = mark[new Vector3(i, 0, j)] - 1;
                    }

                }

                if (weight.ContainsKey(new Vector3(i - 1, 0, j))&&
                    t_color.ContainsKey(new Vector3(i - 1, 0, j)))
                {
                    if(t_color[new Vector3(i - 1, 0, j)] == terrainColor.white)
                    {
                        mark[new Vector3(i, 0, j)] = mark[new Vector3(i, 0, j)] - 1;
                    }

                }

                if (weight.ContainsKey(new Vector3(i + 1, 0, j)))
                {
                    if(t_color[new Vector3(i + 1, 0, j)] == terrainColor.white)
                    {
                        mark[new Vector3(i, 0, j)] = mark[new Vector3(i, 0, j)] - 1;
                    }

                }

                if (weight.ContainsKey(new Vector3(i - 1, 0, j + 1)))
                {
                    if(t_color[new Vector3(i - 1, 0, j+1)] == terrainColor.white)
                    {
                        mark[new Vector3(i, 0, j)] = mark[new Vector3(i, 0, j)] - 1;
                    }

                }

                if (weight.ContainsKey(new Vector3(i, 0, j + 1)))
                {
                    if (t_color[new Vector3(i , 0, j + 1)] == terrainColor.white)
                    {
                        mark[new Vector3(i, 0, j)] = mark[new Vector3(i, 0, j)] - 1;
                    }
                }

                if (weight.ContainsKey(new Vector3(i + 1, 0, j + 1)))
                {
                    if(t_color[new Vector3(i+1, 0, j + 1)] == terrainColor.white)
                    {
                        mark[new Vector3(i, 0, j)] = mark[new Vector3(i, 0, j)] - 1;
                    }
                }
            }
        }
    }

    void MotifyTerrain()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(mark[new Vector3(i, 0, j)] <= 3)
                {
                    t_color[new Vector3(i, 0, j)] = terrainColor.white;
                }

                if (mark[new Vector3(i, 0, j)] >= 5)
                {
                    t_color[new Vector3(i, 0, j)] = terrainColor.black;
                }
            }
        }

        //Reset Weight
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                mark[new Vector3(i, 0, j)] = 8;
            }
        }
    }

    void Generate2D_Map()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (t_color[new Vector3(i, 0, j)] == terrainColor.white)
                {
                    Instantiate(WhiteMark, new Vector3(i, 0, j), new Quaternion());
                }

                

            }
        }
    }

    void CalculateWall()
    {
        for(int i=0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (t_color[new Vector3(i, 0, j)] == terrainColor.black)
                {
                    if(t_color.ContainsKey(new Vector3(i, 0, j-1))&&
                       t_color[new Vector3(i, 0, j - 1)] == terrainColor.white)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.orange;
                    }

                    if (t_color.ContainsKey(new Vector3(i, 0, j + 1)) &&
                       t_color[new Vector3(i, 0, j + 1)] == terrainColor.white)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.orange;
                    }

                    if (t_color.ContainsKey(new Vector3(i-1, 0, j)) &&
                       t_color[new Vector3(i-1, 0, j)] == terrainColor.white)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.orange;
                    }

                    if (t_color.ContainsKey(new Vector3(i + 1, 0, j)) &&
                       t_color[new Vector3(i + 1, 0, j)] == terrainColor.white)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.orange;
                    }
                }

                if (t_color[new Vector3(i, 0, j)] == terrainColor.black)
                {
                    if (t_color.ContainsKey(new Vector3(i, 0, j - 1)) &&
                       t_color[new Vector3(i, 0, j - 1)] == terrainColor.orange)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.red;
                    }

                    if (t_color.ContainsKey(new Vector3(i, 0, j + 1)) &&
                       t_color[new Vector3(i, 0, j + 1)] == terrainColor.orange)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.red;
                    }

                    if (t_color.ContainsKey(new Vector3(i - 1, 0, j)) &&
                       t_color[new Vector3(i - 1, 0, j )] == terrainColor.orange)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.red;
                    }

                    if (t_color.ContainsKey(new Vector3(i + 1, 0, j)) &&
                       t_color[new Vector3(i + 1, 0, j)] == terrainColor.orange)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.red;
                    }
                }

                if (t_color[new Vector3(i, 0, j)] == terrainColor.black)
                {
                    if (t_color.ContainsKey(new Vector3(i, 0, j - 1)) &&
                       t_color[new Vector3(i, 0, j - 1)] == terrainColor.red)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.blue;
                    }

                    if (t_color.ContainsKey(new Vector3(i, 0, j + 1)) &&
                       t_color[new Vector3(i, 0, j + 1)] == terrainColor.red)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.blue;
                    }

                    if (t_color.ContainsKey(new Vector3(i - 1, 0, j)) &&
                       t_color[new Vector3(i - 1, 0, j )] == terrainColor.red)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.blue;
                    }

                    if (t_color.ContainsKey(new Vector3(i + 1, 0, j)) &&
                       t_color[new Vector3(i + 1, 0, j )] == terrainColor.red)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.blue;
                    }
                }

                if (t_color[new Vector3(i, 0, j)] == terrainColor.black)
                {
                    if (t_color.ContainsKey(new Vector3(i, 0, j - 1)) &&
                       t_color[new Vector3(i, 0, j - 1)] == terrainColor.blue)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.yellow;
                    }

                    if (t_color.ContainsKey(new Vector3(i, 0, j + 1)) &&
                       t_color[new Vector3(i, 0, j + 1)] == terrainColor.blue)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.yellow;
                    }

                    if (t_color.ContainsKey(new Vector3(i - 1, 0, j)) &&
                       t_color[new Vector3(i - 1, 0, j )] == terrainColor.blue)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.yellow;
                    }

                    if (t_color.ContainsKey(new Vector3(i + 1, 0, j)) &&
                       t_color[new Vector3(i + 1, 0, j)] == terrainColor.blue)
                    {
                        t_color[new Vector3(i, 0, j)] = terrainColor.yellow;
                    }
                }


            }           
        }

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(t_color[new Vector3(i, 0, j)] == terrainColor.orange)
                {
                    for(int x = 1; x < 3; x++)
                    {
                        Instantiate(G1, new Vector3(i, x, j), new Quaternion());
                    }
                    
                    //Instantiate(G1, new Vector3(i, 1, j), new Quaternion());

                }

                if (t_color[new Vector3(i, 0, j)] == terrainColor.red)
                {
                    for (int x = 1; x < 5; x++)
                    {
                        Instantiate(G2, new Vector3(i, x, j), new Quaternion());
                    }

                    
                    //Instantiate(G2, new Vector3(i, 3, j), new Quaternion());

                }

                if (t_color[new Vector3(i, 0, j)] == terrainColor.blue)
                {

                    for (int x = 1; x < 7; x++)
                    {
                        Instantiate(G3, new Vector3(i, x, j), new Quaternion());
                    }

                    //Instantiate(G3, new Vector3(i, 0, j), new Quaternion());
                    //Instantiate(G3, new Vector3(i, 5, j), new Quaternion());

                }

                if (t_color[new Vector3(i, 0, j)] == terrainColor.yellow)
                {
                    for (int x = 1; x < 8; x++)
                    {
                        Instantiate(G4, new Vector3(i, x, j), new Quaternion());
                    }

                    //Instantiate(G4, new Vector3(i, 0, j), new Quaternion());
                    //Instantiate(G4, new Vector3(i, 7, j), new Quaternion());

                }
            }
        }
    }

    void Perlin2DMap()
    {
        Dictionary<Vector3, float> weight = new Dictionary<Vector3, float>();
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                float randomValue = Random.Range(0f, 1f);
                Vector3 pos = new Vector3(i, 0, j);
                weight.Add(pos, randomValue);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 pos = new Vector3(i, 0, j);
                if (weight[pos] >= 0.9)
                {
                    Instantiate(WhiteMark, pos, new Quaternion());
                }
                else if(weight[pos] >= 0.7&& weight[pos] < 0.9)
                {
                    Instantiate(G1, pos, new Quaternion());
                    //Instantiate(G1, new Vector3(i, -1, j), new Quaternion());
                }
                else if (weight[pos] >= 0.5 && weight[pos] < 0.7)
                {
                    Instantiate(G2, pos, new Quaternion());
                    //Instantiate(G2, new Vector3(i, -2, j), new Quaternion());
                }
                else if (weight[pos] >= 0.3 && weight[pos] < 0.5)
                {
                    Instantiate(G3, pos, new Quaternion());
                    //Instantiate(G3, new Vector3(i, -3, j), new Quaternion());
                }
                else
                {
                    Instantiate(BlackMark, pos, new Quaternion());
                    //Instantiate(BlackMark, new Vector3(i, -4, j), new Quaternion());
                }
            }
        }

    }

    void InitalPerlin()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                float x_coord = (float)i / width;
                float y_coord = (float)j / height;
                float weight = Mathf.PerlinNoise(x_coord, y_coord);
                print(weight);
                Vector3 pos = new Vector3(i, weight, j);
                GameObject cube = Instantiate(WhiteMark, pos, new Quaternion());
            }
        }
    }

    void TestCube()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (t_color[new Vector3(i, 0, j)] == terrainColor.black)
                {


                       Instantiate(BlackMark, new Vector3(i, 0, j), new Quaternion());
                    

                }
            }
        }
    }


    HashSet<Vector3> PerformFloodFill(Vector3 start, HashSet<Vector3> visited)
    {
        HashSet<Vector3> region = new HashSet<Vector3>();
        Queue<Vector3> queue = new Queue<Vector3>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            Vector3 current = queue.Dequeue();
            if (visited.Contains(current))
                continue;

            visited.Add(current);
            region.Add(current);

            // Check all possible 6 directions around the point (consider 3D cardinal directions)
            Vector3[] possibleDirections = new Vector3[]
            {
                new Vector3(1, 0, 0), new Vector3(-1, 0, 0),  // X directions
                new Vector3(0, 1, 0), new Vector3(0, -1, 0), // Y directions
                new Vector3(0, 0, 1), new Vector3(0, 0, -1)  // Z directions
            };

            foreach (Vector3 dir in possibleDirections)
            {
                Vector3 neighbor = current + dir;
                if (whieCube.Contains(neighbor) && !visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        return region;
    }

    HashSet<Vector3> FindLargestRegion(List<HashSet<Vector3>> regions)
    {
        HashSet<Vector3> largest = null;
        int maxSize = 0;

        foreach (HashSet<Vector3> region in regions)
        {
            if (region.Count > maxSize)
            {
                largest = region;
                maxSize = region.Count;
            }
        }

        return largest;
    }

    void PrintNonLargestRegions(List<HashSet<Vector3>> regions, HashSet<Vector3> largestRegion)
    {
        foreach (HashSet<Vector3> region in regions)
        {
            if (region != largestRegion) 
            {
                foreach (Vector3 point in region)
                {
                    t_color[point] = terrainColor.black;
                    whieCube.Remove(point);
                }
            }
        }
    }

    void SpawnPawn()
    {
        int randomIndex = UnityEngine.Random.Range(0, whieCube.Count);
        int randomIndex2 = UnityEngine.Random.Range(0, whieCube.Count);
        GameObject award_Box = Instantiate(awardBox, new Vector3(whieCube[randomIndex].x, 1f, whieCube[randomIndex].z), new Quaternion());
        GameObject obj=Instantiate(theft,new Vector3(whieCube[randomIndex2].x,0.5f, whieCube[randomIndex2].z), new Quaternion());
        GameObject spawnGuard=Instantiate(guard, new Vector3(whieCube[randomIndex].x, 0.5f, whieCube[randomIndex].z), new Quaternion());
        obj.GetComponent<Theft>().canMoveArea = whieCube;
        spawnGuard.GetComponent<AI_1>().guardBox = award_Box;
    }
}
