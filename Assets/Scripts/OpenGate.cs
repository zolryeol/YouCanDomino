using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ư�� ������ �� ���� �������� ����ٰ� ���� ������ �ڵ带 ���� ���̴�.
/// </summary>
public class OpenGate : MonoBehaviour
{
    ///�ڽĹ� 2���� ������ ����������

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
