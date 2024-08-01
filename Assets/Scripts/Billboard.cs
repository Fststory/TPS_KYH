using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{


    void Start()
    {
        
    }

    void Update()
    {
        // 나의 정면 방향을 메인 카메라가 나를 바라보는 방향과 일치시킨다.
        transform.forward = (Camera.main.transform.position - transform.position).normalized;
    }
}
