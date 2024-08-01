using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : ActorBase
{
    public PlayerStateBase myStatus;

    public float rotSpeed = 200.0f;
    public float yVelocity = 2;
    public float jumpPower = 4;
    public int maxJumpCount = 1;
    public Image img_hitUI;

    // 회전 값을 미리 계산하기 위한 회전축(x, y) 변수
    float rotX;
    float rotY;
    float yPos;
    int currentJumpCount = 0;
    float currenTime = 0.5f;
    bool timerStart = false;

    CharacterController cc;

    // 중력을 적용하고 싶다.
    // 바닥에 충돌이 있을 때까지는 아래로 계속 내려가게 하고 싶다.
    // 방향: 아래, 크기: 중력
    Vector3 gravityPower;

    void Start()
    {
        // 최초의 회전 상태대로 시작을 하고 싶다.(전역 변수 초기화)
        rotX = transform.eulerAngles.x;
        rotY = transform.eulerAngles.y;

        // 캐릭터 컨트롤러 컴포넌트를 변수에 담아 놓는다.
        cc = GetComponent<CharacterController>();

        // 중력 값을 초기화한다.
        gravityPower = Physics.gravity;
    }

    void Update()
    {
        Move();
        Rotate();

        // 히트 UI 타이머
        //if (timerStart)
        //{
        //    currenTime -= Time.deltaTime;
        //    if (currenTime < 0)
        //    {
        //        timerStart = false;
        //        img_hitUI.gameObject.SetActive(false);
        //    }
        //}
    }

    // "Horizontal" "Vertical" 입력을 이용해서 수평면으로 이동하게 하고 싶다.
    // 1. 사용자의 입력을 받는다.
    // 2. 방향, 속력을 계산한다.
    // 3. 매 프레임마다 계산된 속도로 자신의 위치를 변경한다.
    void Move()
    {
        // 1. 수평 이동 계산
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 나의 정면 방향에 따라서 이동하도록 변경한다.
        // 1-1. 로컬 방향 벡터를 이용해서 계산하는 방법
        //Vector3 dir = transform.forward * v + transform.right * h;
        //dir.Normalize();

        // 1-2. 나의 회전 값에 따라서 월드 방향 벡터를 로컬 방향의 벡터로 변환하는 함수를 이용하는 방법
        Vector3 dir = new Vector3(h, 0, v); // 월드 방향 벡터
        dir = transform.TransformDirection(dir);
        dir.Normalize();

        // 2. 수직 이동 계산

        // 중력 적용
        yPos += gravityPower.y * yVelocity * Time.deltaTime;

        // 바닥에 닿아 있을 때에는 yPos의 값을 0으로 초기화한다.
        if (cc.collisionFlags == CollisionFlags.CollidedBelow)
        {
            yPos = 0;
            // (바닥에 닿으면) 점프 가능 횟수와 점프 횟수가 초기화 된다.
            currentJumpCount = 0;
        }

        if (currentJumpCount < maxJumpCount)   // 현재 점프 횟수가 점프 가능 횟수보다 작다면 그 횟수만큼 점프 가능하다.
        {
            // 키보드의 스페이스바를 누르면 위쪽으로 점프를 하게 하고 싶다.
            if (Input.GetButtonDown("Jump"))
            {
                yPos = jumpPower;
                // 점프할 때마다 점프횟수가 증가한다.
                currentJumpCount++;
            }
        }

        dir.y = yPos;

        // p = p0 + vt;
        //transform.position += dir * moveSpeed * Time.deltaTime;
        cc.Move(dir * myStatus.speed * Time.deltaTime); //중력 미적용 버전
        //cc.SimpleMove(dir * moveSpeed); //중력 적용 버전
        // 둘의 차이는 simplemove는 중력이 적용되면서 이미 시간 계산까지 하고 있기에 매개변수에 속력만 들어가고 move는 속력과 시간의 곱이 들어간다.
    }

    // 사용자의 마우스 드래그 방향에 따라서 나의 상하좌우 회전이 되게 하고 싶다.
    // 1. 사용자의 마우스 드래그 입력을 받는다.
    // 2. 회전 속력, 회전 방향이 필요하다.
    // 3. 매 프레임마다 계산된 속도로 자신의 회전값을 변경한다.
    void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 각 축 별로 회전 값을 미리 계산한다.(R= R0 +vt)
        rotX += mouseY * rotSpeed * Time.deltaTime;
        rotY += mouseX * rotSpeed * Time.deltaTime;

        // 상하 회전은 -60도 ~ +60도까지로 제한한다.
        if (rotX > 60.0f)
        {
            rotX = 60.0f;
        }
        else if (rotX < -60.0f)
        {
            rotX = -60.0f;
        }

        // 계산된 회전 값을 나의 트랜스폼 회전 값으로 적용한다.
        transform.eulerAngles = new Vector3(0, rotY, 0);
        Camera.main.transform.GetComponent<FollowCamera>().rotX = rotX;
    }

    // 데미지 받았을 때에 실행할 함수
    public override void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {
        base.TakeDamage(atkPower, hitDir, attacker);

        myStatus.currentHP = Mathf.Clamp(myStatus.currentHP - atkPower, 0, myStatus.maxHP);
        //print("내 체력: " + myStatus.currentHP);

        // img_hitUI 오브젝트를 활성화했다가, 0.5초 뒤에 다시 비활성화한다.
        //img_hitUI.gameObject.SetActive(true);
        //currenTime = 0.5f;
        //timerStart = true;
        //Invoke("DeActivateHitUI", 0.5f);
        StartCoroutine(DeActivateHitUI(0.5f));
    }

    // 코루틴 함수
    IEnumerator DeActivateHitUI(float delayTIme)
    {
        //float addValue = 0.05f;
        for (int i = 0; i < 100; i++)
        {
            Color colorVector = img_hitUI.color;
            float addValue = 0.05f;
            if (i>49)
            {
                addValue *= -1;
            }
            colorVector.a += addValue;
            img_hitUI.color = colorVector;
            //yield return new WaitForSeconds(delayTIme);
            yield return null;
        }
    }
        //void DeActivateHitUI()
        //{
        //    img_hitUI.gameObject.SetActive(false);
        //}

        //private void OnControllerColliderHit(ControllerColliderHit hit)
        //{

        //}

}