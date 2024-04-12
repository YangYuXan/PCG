using NPBehave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AI_1 : MonoBehaviour
{
    private Root behaviorTree;
    public bool seeEnermy=false;

    public float viewRadius = 5f;//视野距离
    public int viewAngleStep = 20;//射线密度
    [Range(0, 360)]
    public float viewAngle = 270f;//视野角度
    public GameObject guardBox;
    public GameObject Enermy;
    Vector3 guardPoint;
    Vector3 StarPoint;
    Vector3 enermyPoint;

    public NavMeshAgent agent;
    Blackboard blackboard;


    // Start is called before the first frame update
    void Start()
    {
        blackboard = new Blackboard(UnityContext.GetClock());
        blackboard["seeEnermy"] = false;

        behaviorTree = new Root(blackboard,
            new Selector(

                new BlackboardCondition("seeEnermy", Operator.IS_EQUAL, false, Stops.IMMEDIATE_RESTART,
                    new Sequence(
                            new Action(() => Xunluo()),
                            new Wait(2)

                        )
                ),

                new BlackboardCondition("seeEnermy", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        new Sequence(
                                new Action(() => Zhuiji()),
                                new Wait(0.1f)
                            )


                    )

                )
        );

        behaviorTree.Start();
    }

    // Update is called once per frame
    void Update()
    {

        DrawFieldOfView();

    }

    void Zhuiji()
    {
        agent.SetDestination(enermyPoint);
    }

    void Xunluo()
    {
        float x = Random.RandomRange(-2,2);
        float z = Random.RandomRange(-2, 2);

        guardPoint = new Vector3(guardBox.transform.position.x+x,0, guardBox.transform.position.z+z);
        StarPoint = guardPoint;
        agent.SetDestination(guardPoint);
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
                if (hitInfo.collider.tag == "Theft")
                {
                    seeEnermy = true;
                    enermyPoint = hitInfo.collider.transform.position;
                }
            }
        }

        if (!seeEnermy)
        {
            blackboard["seeEnermy"] = false;
        }
    }
}
