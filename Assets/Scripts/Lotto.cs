using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lotto : MonoBehaviour
{
    // 지정된 범위 내의 숫자 중에서 랜덤하게 숫자를 뽑아서 나열하고 싶다.
    public int MaxNumber = 12;
    public int numCount = 12;

    public List<int> luckyNumbers = new List<int>();
    public List<string> teamName = new List<string>();

    void Start()
    {
        luckyNumbers.Capacity = numCount;

        DrawLottoNumbers();
    }

    public void DrawLottoNumbers()
    {
        // 1. 최대 숫자만큼 랜덤한 숫자를 뽑는다.
        for(int i = 0; i < numCount; i++)
        {
            int num = Random.Range(0, MaxNumber);
            luckyNumbers[i] = num;

            // 2. 혹시 중복된 숫자가 있는지 확인한다.
            for (int j = 0; j < i; j++)
            {
                if (num == luckyNumbers[j])
                {
                    // 3. 중복된 숫자가 있었다면 재추첨하기로 하고 중복 검사를 종료한다.
                    i--;
                    break;
                }
            }            
        }

        // 테스트용 출력
        //string result = "";

        for(int i = 0; i < luckyNumbers.Count; i++)
        {
            //result += luckyNumbers[i].ToString() + ", ";
            print(teamName[luckyNumbers[i]]);
        }

        //print(result);
    }

}
