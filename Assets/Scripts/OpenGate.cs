using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 버튼이 눌렸을 때 문이 열리도록 여기다가 문이 열리는 코드를 넣을 것이다.
/// </summary>
public class OpenGate : MonoBehaviour
{
    ///자식문 2개를 변수로 가지고있자

    Transform doorLeft;
    Transform doorRight;

    float maxAngle = -90;
    float nowAngle = 0;

    private void Awake()
    {
        doorLeft = transform.GetChild(0).transform;
        doorRight = transform.GetChild(1).transform;
    }

    public void OpenGateNow()
    {
        StartCoroutine(StartOpenSlowly());
    }

    IEnumerator StartOpenSlowly()
    {
        doorLeft.gameObject.GetComponent<Collider>().enabled = false;
        doorRight.gameObject.GetComponent<Collider>().enabled = false;

        while (maxAngle < nowAngle)
        {
            nowAngle -= 1f;

            doorLeft.localRotation = Quaternion.Euler(0, nowAngle, 0);
            doorRight.localRotation = Quaternion.Euler(0, nowAngle * -1, 0);

            yield return new WaitForFixedUpdate();
        }
    }

}
