using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class EnemyFSM : ActorBase
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
    public bool drawGizmos = true;

    [Header("기본 속성")]
    public EnemyIninPreferences initPreferences;
    public EnemyStateBase myStatus;
    public Slider hpSlider;


    float currentTime = 0;
    float idleTime = 3.0f;
    Vector3 patrolCenter;
    CharacterController cc;
    Vector3 patrolNext;
    Vector3 hitDirection;

    [SerializeField]
    Transform target;

    void Start()
    {
        patrolCenter = transform.position;
        patrolNext = patrolCenter;
        cc = GetComponent<CharacterController>();
        //myStatus.Initialize(100, 9);
        //myStatus.patrolSpeed = 5;
        hpSlider.value = myStatus.currentHP / myStatus.maxHP;
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
        CheckSight(initPreferences.sightRange, initPreferences.sightDistance);

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
        CheckSight(initPreferences.sightRange, initPreferences.sightDistance);

        // 선택된 지점으로 이동한다.
        Vector3 dir = patrolNext - transform.position;
        if (dir.magnitude > 0.1f)
        {
            cc.Move(dir.normalized * myStatus.patrolSpeed * Time.deltaTime);
        }
        // 목적지에 도달하고, 2초~3초 사이만큼 대기한 다음 다른 지점을 추첨한다.
        else
        {
            // patrolRadius를 반경으로 하는 원의 임의의 지점을 선택한다.
            #region 1. 벡터 기본 연산 방식
            //float h, v;
            //h = Random.Range(-1, 1);
            //v = Random.Range(-1, 1);
            //float distance = Random.Range(0, initPreferences.patrolRadius);
            //Vector3 newPos = new Vector3(h, 0, v).normalized * distance;
            //patrolNext = patrolCenter + newPos;
            #endregion

            #region 2. Random 클래스에 있는 inside 함수를 이용해서 연산하는 방식
            Vector2 newPos = Random.insideUnitCircle * initPreferences.patrolRadius;
            patrolNext = patrolCenter + new Vector3(newPos.x, 0, newPos.y);
            #endregion

            #region 3. 삼각 함수를 이용한 계산식
            //float degree = Random.Range(-180.0f, 180.0f);
            //Vector3 newPos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), 0, Mathf.Sin(Mathf.Deg2Rad * degree));
            //float distance = Random.Range(0, initPreferences.patrolRadius);
            //patrolNext = patrolCenter + newPos * distance;
            #endregion

            myState = EnemyState.Idle;
            print("My State: Patrol -> Idle");
            idleTime = Random.Range(2.0f, 3.0f);
               
        }
    }

    void CheckSight(float degree, float maxDistance)
    {
        // 시야 범위 안에 들어온 대상이 있다면 그 대상을 타겟으로 설정하고 싶다.
        // 시야 범위(시야각: 좌우 30도, 전방, 최대 시야 거리: 15미터)
        // 대상 선택을 위한 태그(Player) 설정
        target = null;

        // 1. 월드 안에 배치된 오브젝트 중에 Tag 가 "Player"인 오브젝트를 모두 찾는다.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // 2. 찾은 오브젝트들 중에서 거리가 maxDistance 이내인 오브젝트만 찾는다.
        for(int i = 0; i < players.Length; i++)
        {
            float distance = Vector3.Distance(players[i].transform.position, transform.position);

            if (distance <= maxDistance)
            {
                // 3. 찾은 오브젝트를 바라보는 벡터와 나의 전방 벡터를 내적한다.
                Vector3 lookVector = players[i].transform.position - transform.position;
                lookVector.Normalize();

                float cosTheta = Vector3.Dot(transform.forward, lookVector);
                float theta = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;

                // 4-1. 만일, 내적의 결과 값이 0보다 크면(나보다 앞쪽에 있다)...
                // 4-2. 만일, 사잇각의 값이 30보다 작으면(전방 좌우 30도 이내)...
                if (cosTheta > 0 && theta < degree)
                {
                    target = players[i].transform;

                    // 상태를 trace 상태로 전환한다.
                    myState = EnemyState.Trace;
                    print("My State: Idle/Patrol -> Trace");
                }
            }
        }
    }


    private void TraceTarget()
    {
        // 만일, 최대 추격 거리를 벗어났다면...
        if(Vector3.Distance(transform.position, patrolCenter) > initPreferences.maxTraceDistance)
        {
            // 상태를 Return 상태로 전환한다.
            myState = EnemyState.Return;
            print("My State: Trace -> Return");
            return;
        }

        // 타겟을 향해 추격 이동한다.
        Vector3 dir = target.transform.position - transform.position;
        cc.Move(dir.normalized * myStatus.speed * Time.deltaTime);

        // 공격 범위 이내로 들어가면 상태를 Attack 상태로 전환한다.
        if (dir.magnitude < initPreferences.attackRange)
        {
            myState = EnemyState.Attack;
            currentTime = 0;
            print("My State: Trace -> Attack");
        }
    }

    private void Attack()
    {
        // 공격을 한다
        target.GetComponent<PlayerMove>().TakeDamage(20, Vector3.zero, transform);

        // 공격 애니메이션이 끝나면 공격 대기 상태로 전환한다.        
        myState = EnemyState.AttackDelay;
        print("My State: Attack -> AttackDelay");
    }

    private void AttackDelay()
    {
        // 만일, 타겟과 거리가 공격 가능한 범위를 벗어났다면...
        if (Vector3.Distance(transform.position, target.position) > initPreferences.attackRange)
        {
            // 다시 추격 상태로 전환한다.
            myState = EnemyState.Trace;
            print("My State: AttackDelay -> Trace");
            currentTime = 0;
            return;
        }

        // 일정 시간 대기한다.
        currentTime += Time.deltaTime;
        // 일정 시간이 지났다면 상태를 다시 공격 상태로 전환한다.
        if (currentTime > 1.5f)
        {
            currentTime = 0;
            myState = EnemyState.Attack;
            print("My State: AttackDelay -> Attack");
        }
    }

    private void ReturnHome()
    {
        // patrolCenter 위치로 다시 돌아간다.
        Vector3 dir = patrolCenter - transform.position;

        // 목적지에 근접했다면...
        if (dir.magnitude < 0.1f)
        {
            transform.position = patrolCenter;

            // 상태를 Idle 상태로 전환한다.
            myState = EnemyState.Idle;
            print("My State: Return -> Idle");
        }
        // 그렇지 않았다면...
        else
        {
            cc.Move(dir.normalized * myStatus.speed * Time.deltaTime);
        }

    }

    private void Ondamaged()
    {
        // 일정 시간 뒤로 물러났다가(knock-back) 상태를 Trace 상태로 전환한다.
        transform.position = Vector3.Lerp(transform.position, hitDirection, 0.25f);

        if (Vector3.Distance(transform.position, hitDirection) < 0.1f)
        {
            myState = EnemyState.Trace;
            print("My State: Damaged -> Trace");
        }
    }

    // 상대방이 나에게 데미지를 부여하는 함수
    public override void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {
        // 부모에 구현된 TakeDamage 함수를 먼저 실행한다.
        base.TakeDamage(atkPower, hitDir, attacker);

        if (myState == EnemyState.Dead || myState == EnemyState.Return || myState == EnemyState.Damaged)
        {
            return;
        }

        // 1. 현재 체력에 상대의 공격력만큼을 감소시킨다.(min 0 ~ max 100)
        myStatus.currentHP = Mathf.Clamp(myStatus.currentHP - atkPower, 0, myStatus.maxHP);

        // 1-2. 체력 슬라이더 UI에 현재 체력을 표시한다.
        hpSlider.value = myStatus.currentHP / myStatus.maxHP;

        // 2. 만일, 그 결과 현재 체력이 0 이하라면...
        if (myStatus.currentHP <= 0)
        {
            // 2-1. 나의 상태를 죽음 상태로 전환한다.
            myState = EnemyState.Dead;
            print("My State: Any -> Dead");

            // 2-2. 콜라이더 컴포넌트 비활성화 처리한다.
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            
        }
        // 3. 그렇지 않다면...
        else
        {
            // 3-1. 나의 상태를 데미지 상태로 전환한다.
            myState = EnemyState.Damaged;
            print("My State: Any -> Damaged");
            currentTime = 0;

            // 3-2. 타격 방향으로 일정 거리만큼을 넉백 위치로 지정한다.
            hitDirection = transform.position + hitDir * 1.5f;
            // 3-3. 공격자를 타겟으로 설정한다.
            target = attacker;
        }
    }


    private void Dead()
    {
        // 클래스 변수를 일시에 초기화하기
        //initPreferences = new EnemyInitPreferences(


        // 3초 뒤에 제거된다.
        currentTime += Time.deltaTime;
        if (currentTime > 3.0f)
        {
            Destroy(gameObject);
        }
    }

    // 원 그리기
    private void OnDrawGizmos()
    {
        if (!drawGizmos)
        {
            return;
        }

        Gizmos.color = new Color32(23, 243, 255, 255);

        #region 원 그리기
        //List<Vector3> points = new List<Vector3>();
        //for(int i = 0; i < 360; i += 5)
        //{
        //    Vector3 point = new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), 0, Mathf.Sin(i * Mathf.Deg2Rad)) * 5;
        //    points.Add(transform.position + point);
        //}

        //for(int i = 0; i < points.Count - 1; i++)
        //{
        //    Gizmos.DrawLine(points[i], points[i + 1]);
        //}
        #endregion

        // 시야각 그리기
        float rightDegree = 90 - initPreferences.sightRange;
        float leftDegree = 90 + initPreferences.sightRange;

        Vector3 rightPos = new Vector3(Mathf.Cos(rightDegree * Mathf.Deg2Rad),
                                       0,
                                       Mathf.Sin(rightDegree * Mathf.Deg2Rad)) * initPreferences.sightDistance
                                + transform.position;
        Vector3 leftPos = new Vector3(Mathf.Cos(leftDegree * Mathf.Deg2Rad),
                                      0,
                                      Mathf.Sin(leftDegree * Mathf.Deg2Rad)) * initPreferences.sightDistance
                                + transform.position;

        Gizmos.DrawLine(transform.position, rightPos);
        Gizmos.DrawLine(transform.position, leftPos);

        // 최대 추격 거리 그리기
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(patrolCenter, initPreferences.maxTraceDistance);        
    }
}

// 직렬화 클래스
[System.Serializable]
public class EnemyIninPreferences
{
    public float patrolRadius = 4.0f;
    public float attackRange = 2.0f;
    [Range(0.0f, 90.0f)]
    public float sightRange = 30.0f;
    public float sightDistance = 15.0f;
    public float maxTraceDistance = 25.0f;

    // 생성자 함수 (= 값 초기화 함수, 메모리에 인스턴스를 생성하는 과정에서 값을 넣어줌)
    public EnemyIninPreferences(float patrolRadius, float attackRange, float sightRange, float sightDistance, float maxTraceDistance)
    {
        this.patrolRadius = patrolRadius;
        this.attackRange = attackRange;
        this.sightRange = Mathf.Clamp(sightRange, 0, 90.0f);
        this.sightDistance = sightDistance;
        this.maxTraceDistance = maxTraceDistance;
    }
}