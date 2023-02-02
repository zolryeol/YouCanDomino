using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPush : MonoBehaviour
{

    GameObject buttonUpper = null;
    [Header("�������� ���� ��")]
    [SerializeField]
    OpenGate beOpenedGate = null;
    [Range(0, 1)]
    public float pushButtonVolume;
    bool isOpen = false;

    private void Awake()
    {
        buttonUpper = gameObject.transform.GetChild(0).gameObject;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.name + "�ݶ��̴� ���γ�");

        if (other.CompareTag("Domino") && isOpen == false)
        {
            isOpen = true;
            SoundMG.Instance.PlaySFX(eSoundType.TouchButton, transform.position, pushButtonVolume);

            StartCoroutine(ButtonDown());

            Debug.Log("Ʈ���� �ɸ�");
        }
    }

    IEnumerator ButtonDown()
    {
        while (0 < buttonUpper.transform.localPosition.y)
        {
            buttonUpper.transform.Translate(-Vector3.up * 0.001f);
            //Debug.Log("�ڷ�ƾ ȣ��");

            yield return new WaitForEndOfFrame();
        }
        beOpenedGate.OpenGateNow();
    }
}
