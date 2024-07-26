using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    // 배열(Array)
    Transform[] camPositions = new Transform[2];

    // 리스트(List)
    public List<Transform> camList = new List<Transform>();

    FollowCamera followCam;
    float currentRate = 0;

    void Start()
    {
        // FollowCamera 컴포넌트를 캐싱한다.
        followCam = Camera.main.gameObject.GetComponent<FollowCamera>();

        // 자식 게임오브젝트 중에서 두번째(Near)와 세번째(Far) 오브젝트를 찾아서 배열에 넣는다.
        //camPositions[0] = transform.GetChild(1);
        //camPositions[1] = transform.GetChild(2);

        camList.Add(transform.GetChild(1));
        camList.Add(transform.GetChild(2));

        // 초기 카메라는 Near 카메라(1인칭)로 한다.
        ChangeCamTarget(0, false);
    }

    void Update()
    {
        // 만일, 숫자 키 1번을 누르면 NearPos를 타겟으로 지정하고,
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeCamTarget(0, false);
        }
        // 2번을 누르면 FarPos를 타겟으로 지정한다.
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeCamTarget(1, true);
        }

        currentRate -= Input.GetAxis("Mouse ScrollWheel") * 0.5f;
        // currentRate += Time.deltaTime * 0.5f;
        currentRate = Mathf.Clamp(currentRate, 0.0f, 1.0f);

        Camera.main.transform.position = Vector3.Lerp(camList[0].position, camList[1].position, currentRate);
    }

    void ChangeCamTarget(int targetNum, bool isDynamic)
    {
        // 메인 카메라의 FollowCamera 클래스에 있는 target에 0번 요소를 넣는다.
        // 1. 메인 카메라의 FollowCamera 컴포넌트에 접근한다.

        if (followCam != null)
        {
            //followCam.target = camPositions[targetNum];
            followCam.target = camList[targetNum];
            followCam.dynamicCam = isDynamic;
        }
    }
}
