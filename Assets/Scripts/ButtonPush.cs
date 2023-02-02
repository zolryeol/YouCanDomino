using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPush : MonoBehaviour
{

    GameObject buttonUpper = null;
    [Header("눌렀을때 열릴 문")]
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
        Debug.Log(other.transform.name + "콜라이더 닿인놈");

        if (other.CompareTag("Domino") && isOpen == false)
        {
            isOpen = true;
            SoundMG.Instance.PlaySFX(eSoundType.TouchButton, transform.position, pushButtonVolume);

            StartCoroutine(ButtonDown());

            Debug.Log("트리거 걸림");
        }
    }

    IEnumerator ButtonDown()
    {
        while (0 < buttonUpper.transform.localPosition.y)
        {
            buttonUpper.transform.Translate(-Vector3.up * 0.001f);
            //Debug.Log("코루틴 호출");

            yield return new WaitForEndOfFrame();
        }
        beOpenedGate.OpenGateNow();
    }
}
