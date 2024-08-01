using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBase : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // summary 표시가 안된다면 vs를 껐다 키면 됨
    /// <summary>
    /// 데미지를 입힐 때 사용하는 함수
    /// </summary>
    /// <param name="atkPower">실제로 줄 데미지 값</param>
    /// <param name="hitDir">넉백을 시킬 방향 벡터</param>
    /// <param name="attacker">공격자의 트랜스폼 컴포넌트</param>
    public virtual void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {

    }
}
