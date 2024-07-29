using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState  // ������ ������ ���������ڰ� �����ؾ� �ȴ�. enum EnemyState/ myState
    {
        Idle = 0,
        Patrol = 1,
        Trace = 2,
        Attack = 4,
        AttackDelay = 8,
        Return = 16,
        Damaged = 32,
        Dead = 64
    }

    public EnemyState myState = EnemyState.Idle;
    public float patrolRadius = 4.0f;
    public float patrolSpeed = 5.0f;
    public float traceSpeed = 9.0f;
    public float attackRange = 2.0f;

    float currentTime = 0;
    float idleTime = 3.0f;
    Vector3 patrolCenter;
    CharacterController cc;
    Vector3 patrolNext;

    [SerializeField]
    Transform target;

    void Start()
    {
        patrolCenter = transform.position;
        patrolNext = patrolCenter;
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // ���� ���� ���¿� ���� ������ �Լ��� �����Ѵ�.
        switch (myState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Trace:
                TraceTarget();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.AttackDelay:
                AttackDelay();
                break;
            case EnemyState.Return:
                ReturnHome();
                break;
            case EnemyState.Damaged:
                Ondamaged();
                break;
            case EnemyState.Dead:
                Dead();
                break;

        }
    }

    private void Idle()
    {
        currentTime += Time.deltaTime;
        if (currentTime > idleTime)
        {
            currentTime = 0;
            myState = EnemyState.Patrol;
            print("My State: Idle -> Patrol");
        }

    }

    private void Patrol()
    {
        // ���õ� �������� �̵��Ѵ�.
        Vector3 dir = patrolNext - transform.position;
        if (dir.magnitude > 0.1f)
        {
            cc.Move(dir.normalized * patrolSpeed * Time.deltaTime);
        }
        // �������� �����ϰ�, 2��~3�� ���̸�ŭ ����� ���� �ٸ� ������ ��÷�Ѵ�.
        else
        {           
            // patrolRadius�� �ݰ����� �ϴ� ���� ������ ������ �����Ѵ�.
            #region 1. ���� �⺻ ���� ���
            //float h, v;
            //h = Random.Range(-patrolRadius, patrolRadius);
            //v = Random.Range(-patrolRadius, patrolRadius);
            //Vector3 newPos = new Vector3(h, 0, v).normalized * patrolRadius;
            //patrolNext = patrolCenter + newPos;
            #endregion

            #region 2. Random Ŭ������ �ִ� inside �Լ��� �̿��ؼ� �����ϴ� ���
            Vector2 newPos = Random.insideUnitCircle* patrolRadius;
            patrolNext = patrolCenter + new Vector3(newPos.x, 0, newPos.y);
            #endregion

            #region 3. �ﰢ �Լ��� �̿��� ����
            //float degree = Random.Range(-180.0f, 180.0f);
            //Vector3 newPos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), 0, Mathf.Sin(Mathf.Deg2Rad * degree));
            //float distance = Random.Range(0, patrolRadius);
            //patrolNext = patrolCenter + newPos * distance;
            #endregion

            myState = EnemyState.Idle;
            idleTime = Random.Range(2.0f, 3.0f);
               
        }
    }

    private void TraceTarget()
    {
        // Ÿ���� ���� �߰� �̵��Ѵ�.
        Vector3 dir = target.transform.position - transform.position;
        cc.Move(dir.normalized * traceSpeed * Time.deltaTime);

        // ���� ���� �̳��� ���� ���¸� Attack ���·� ��ȯ�Ѵ�.
        if (dir.magnitude < attackRange)
        {
            myState = EnemyState.Attack;
        }
    }

    private void Attack()
    {

    }

    private void AttackDelay()
    {

    }

    private void ReturnHome()
    {

    }

    private void Ondamaged()
    {

    }

    private void Dead()
    {

    }

    // �� �׸���
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = new Color32(23, 243, 255, 255);

    //    List<Vector3> points = new List<Vector3>();
    //    for(int i = 0; i < 360; i += 5)
    //    {
    //        Vector3 point = new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), 0, Mathf.Sin(i * Mathf.Deg2Rad)) * 5;
    //        points.Add(transform.position + point);
    //    }

    //    for(int i = 0; i < points.Count - 1; i++)
    //    {
    //        Gizmos.DrawLine(points[i], points[i + 1]);
    //    }
    //}
}
