using NPBehave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Theft : MonoBehaviour
{
    public float viewRadius = 5f;//��Ұ����
    public int viewAngleStep = 20;//�����ܶ�
    [Range(0, 360)]
    public float viewAngle = 270f;//��Ұ�Ƕ�

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
        // ��������෽�������
        Vector3 forward_left = Quaternion.Euler(0, -(viewAngle / 2f), 0) * transform.forward * viewRadius;

        for (int i = 0; i <= viewAngleStep; i++)
        {
            Vector3 v = Quaternion.Euler(0, (viewAngle / viewAngleStep) * i, 0) * forward_left;// ���ݵ�ǰ�Ƕȼ��㷽������
            Vector3 pos = transform.position + v;// ���������յ�

            // ��Scene�л�������(������۲죬Game��ͼ�в��ɼ�)
            Debug.DrawLine(transform.position, pos, Color.red);

            // ���߼��
            Ray ray = new Ray(transform.position, v);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, viewRadius))
            {
                // ������߻�����ײ���������ǩΪ"Enemy"
                if (hitInfo.collider.tag == "Box")
                {
                    // �����ʾ��Ϣ
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
