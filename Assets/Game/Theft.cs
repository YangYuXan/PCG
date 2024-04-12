using NPBehave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Theft : MonoBehaviour
{
    public float viewRadius = 5f;//视野距离
    public int viewAngleStep = 20;//射线密度
    [Range(0, 360)]
    public float viewAngle = 270f;//视野角度

    public bool see = false;

    private Blackboard blackboard;
    private Root behaviorTree;

    //public Transform targetPoint;
    public Vector3 startPoint;
    public float moveSpeed = 2.0f;

    public List<Vector3> canMoveArea = new List<Vector3>();
    public NavMeshAgent agent;
    int randomIndex;

    // Start is called before the first frame update
    void Start()
    {

        randomIndex = UnityEngine.Random.Range(0, canMoveArea.Count);
        startPoint = transform.position;
        print(startPoint);


        blackboard = new Blackboard(UnityContext.GetClock());
        blackboard["see"] = see;

        behaviorTree = CreateBehaviorTree();
        behaviorTree.Start();
    }

    // Update is called once per frame
    void Update()
    {
        DrawFieldOfView();
        blackboard["see"] = see;
    }

    void DrawFieldOfView()
    {
        // 计算最左侧方向的向量
        Vector3 forward_left = Quaternion.Euler(0, -(viewAngle / 2f), 0) * transform.forward * viewRadius;

        for (int i = 0; i <= viewAngleStep; i++)
        {
            Vector3 v = Quaternion.Euler(0, (viewAngle / viewAngleStep) * i, 0) * forward_left;// 根据当前角度计算方向向量
            Vector3 pos = transform.position + v;// 计算射线终点

            // 在Scene中绘制线条(仅方便观察，Game视图中不可见)
            Debug.DrawLine(transform.position, pos, Color.red);

            // 射线检测
            Ray ray = new Ray(transform.position, v);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, viewRadius))
            {
                // 如果射线击中碰撞体且物体标签为"Enemy"
                if (hitInfo.collider.tag == "Box")
                {
                    // 输出提示信息
                    see = true;
                    Destroy(hitInfo.collider.gameObject);
                    Debug.Log("Box");
                }
            }
        }
    }



    Root CreateBehaviorTree()
    {
        return new Root(blackboard,
                new Selector(
                        new BlackboardCondition("see", Operator.IS_EQUAL, false, Stops.IMMEDIATE_RESTART,
                                new Sequence(
                                        new Action(()=>agent.SetDestination(canMoveArea[randomIndex])),
                                        new Wait(3),
                                        new Action(()=> WanderRandomly())
                                    )

                            ),
                        new BlackboardCondition("see", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                            new Sequence(
                                    new Action(() => agent.SetDestination(startPoint)),
                                    new Action(()=>print("go back")),
                                    new Wait(10),
                                    new Action(()=>see=false)

                                )
                        
                        )
                    )
                );   
    }

    void SetCanMoveArea(List<Vector3> moveArea)
    {
        canMoveArea = moveArea;
    }

    void WanderRandomly()
    {
        randomIndex = UnityEngine.Random.Range(0, canMoveArea.Count);

    }



}
