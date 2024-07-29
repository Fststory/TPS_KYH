using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState  // 원본과 변수는 접근한정자가 동일해야 된다. enum EnemyState/ myState
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
        // 나의 현재 상태에 따라 각각의 함수를 실행한다.
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
        // 선택된 지점으로 이동한다.
        Vector3 dir = patrolNext - transform.position;
        if (dir.magnitude > 0.1f)
        {
            cc.Move(dir.normalized * patrolSpeed * Time.deltaTime);
        }
        // 목적지에 도달하고, 2초~3초 사이만큼 대기한 다음 다른 지점을 추첨한다.
        else
        {           
            // patrolRadius를 반경으로 하는 원의 임의의 지점을 선택한다.
            #region 1. 벡터 기본 연산 방식
            //float h, v;
            //h = Random.Range(-patrolRadius, patrolRadius);
            //v = Random.Range(-patrolRadius, patrolRadius);
            //Vector3 newPos = new Vector3(h, 0, v).normalized * patrolRadius;
            //patrolNext = patrolCenter + newPos;
            #endregion

            #region 2. Random 클래스에 있는 inside 함수를 이용해서 연산하는 방식
            Vector2 newPos = Random.insideUnitCircle* patrolRadius;
            patrolNext = patrolCenter + new Vector3(newPos.x, 0, newPos.y);
            #endregion

            #region 3. 삼각 함수를 이용한 계산식
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
        // 타겟을 향해 추격 이동한다.
        Vector3 dir = target.transform.position - transform.position;
        cc.Move(dir.normalized * traceSpeed * Time.deltaTime);

        // 공격 범위 이내로 들어가면 상태를 Attack 상태로 전환한다.
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

    // 원 그리기
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
