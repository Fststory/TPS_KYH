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

    // ȸ�� ���� �̸� ����ϱ� ���� ȸ����(x, y) ����
    float rotX;
    float rotY;
    float yPos;
    int currentJumpCount = 0;
    float currenTime = 0.5f;
    bool timerStart = false;

    CharacterController cc;

    // �߷��� �����ϰ� �ʹ�.
    // �ٴڿ� �浹�� ���� �������� �Ʒ��� ��� �������� �ϰ� �ʹ�.
    // ����: �Ʒ�, ũ��: �߷�
    Vector3 gravityPower;

    void Start()
    {
        // ������ ȸ�� ���´�� ������ �ϰ� �ʹ�.(���� ���� �ʱ�ȭ)
        rotX = transform.eulerAngles.x;
        rotY = transform.eulerAngles.y;

        // ĳ���� ��Ʈ�ѷ� ������Ʈ�� ������ ��� ���´�.
        cc = GetComponent<CharacterController>();

        // �߷� ���� �ʱ�ȭ�Ѵ�.
        gravityPower = Physics.gravity;
    }

    void Update()
    {
        Move();
        Rotate();

        // ��Ʈ UI Ÿ�̸�
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

    // "Horizontal" "Vertical" �Է��� �̿��ؼ� ��������� �̵��ϰ� �ϰ� �ʹ�.
    // 1. ������� �Է��� �޴´�.
    // 2. ����, �ӷ��� ����Ѵ�.
    // 3. �� �����Ӹ��� ���� �ӵ��� �ڽ��� ��ġ�� �����Ѵ�.
    void Move()
    {
        // 1. ���� �̵� ���
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // ���� ���� ���⿡ ���� �̵��ϵ��� �����Ѵ�.
        // 1-1. ���� ���� ���͸� �̿��ؼ� ����ϴ� ���
        //Vector3 dir = transform.forward * v + transform.right * h;
        //dir.Normalize();

        // 1-2. ���� ȸ�� ���� ���� ���� ���� ���͸� ���� ������ ���ͷ� ��ȯ�ϴ� �Լ��� �̿��ϴ� ���
        Vector3 dir = new Vector3(h, 0, v); // ���� ���� ����
        dir = transform.TransformDirection(dir);
        dir.Normalize();

        // 2. ���� �̵� ���

        // �߷� ����
        yPos += gravityPower.y * yVelocity * Time.deltaTime;

        // �ٴڿ� ��� ���� ������ yPos�� ���� 0���� �ʱ�ȭ�Ѵ�.
        if (cc.collisionFlags == CollisionFlags.CollidedBelow)
        {
            yPos = 0;
            // (�ٴڿ� ������) ���� ���� Ƚ���� ���� Ƚ���� �ʱ�ȭ �ȴ�.
            currentJumpCount = 0;
        }

        if (currentJumpCount < maxJumpCount)   // ���� ���� Ƚ���� ���� ���� Ƚ������ �۴ٸ� �� Ƚ����ŭ ���� �����ϴ�.
        {
            // Ű������ �����̽��ٸ� ������ �������� ������ �ϰ� �ϰ� �ʹ�.
            if (Input.GetButtonDown("Jump"))
            {
                yPos = jumpPower;
                // ������ ������ ����Ƚ���� �����Ѵ�.
                currentJumpCount++;
            }
        }

        dir.y = yPos;

        // p = p0 + vt;
        //transform.position += dir * moveSpeed * Time.deltaTime;
        cc.Move(dir * myStatus.speed * Time.deltaTime); //�߷� ������ ����
        //cc.SimpleMove(dir * moveSpeed); //�߷� ���� ����
        // ���� ���̴� simplemove�� �߷��� ����Ǹ鼭 �̹� �ð� ������ �ϰ� �ֱ⿡ �Ű������� �ӷ¸� ���� move�� �ӷ°� �ð��� ���� ����.
    }

    // ������� ���콺 �巡�� ���⿡ ���� ���� �����¿� ȸ���� �ǰ� �ϰ� �ʹ�.
    // 1. ������� ���콺 �巡�� �Է��� �޴´�.
    // 2. ȸ�� �ӷ�, ȸ�� ������ �ʿ��ϴ�.
    // 3. �� �����Ӹ��� ���� �ӵ��� �ڽ��� ȸ������ �����Ѵ�.
    void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // �� �� ���� ȸ�� ���� �̸� ����Ѵ�.(R= R0 +vt)
        rotX += mouseY * rotSpeed * Time.deltaTime;
        rotY += mouseX * rotSpeed * Time.deltaTime;

        // ���� ȸ���� -60�� ~ +60�������� �����Ѵ�.
        if (rotX > 60.0f)
        {
            rotX = 60.0f;
        }
        else if (rotX < -60.0f)
        {
            rotX = -60.0f;
        }

        // ���� ȸ�� ���� ���� Ʈ������ ȸ�� ������ �����Ѵ�.
        transform.eulerAngles = new Vector3(0, rotY, 0);
        Camera.main.transform.GetComponent<FollowCamera>().rotX = rotX;
    }

    // ������ �޾��� ���� ������ �Լ�
    public override void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {
        base.TakeDamage(atkPower, hitDir, attacker);

        myStatus.currentHP = Mathf.Clamp(myStatus.currentHP - atkPower, 0, myStatus.maxHP);
        //print("�� ü��: " + myStatus.currentHP);

        // img_hitUI ������Ʈ�� Ȱ��ȭ�ߴٰ�, 0.5�� �ڿ� �ٽ� ��Ȱ��ȭ�Ѵ�.
        //img_hitUI.gameObject.SetActive(true);
        //currenTime = 0.5f;
        //timerStart = true;
        //Invoke("DeActivateHitUI", 0.5f);
        StartCoroutine(DeActivateHitUI(0.5f));
    }

    // �ڷ�ƾ �Լ�
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