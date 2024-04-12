using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanDetection : MonoBehaviour
{
    public float viewRadius = 5f;//��Ұ����
    public int viewAngleStep = 20;//�����ܶ�
    [Range(0, 360)]
    public float viewAngle = 270f;//��Ұ�Ƕ�

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DrawFieldOfView();
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
                if (hitInfo.collider.tag == "Enemy")
                {
                    // �����ʾ��Ϣ
                    Debug.Log("��Ұ���е���");
                    // ����Լ���Ҫ���߼�
                }
            }
        }
    }

}
