using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������� ���Ǻ�
/// ���̴� ���� ������� ����Ŀ� �Ѿ�����.
/// </summary>


public class LongLongDomino : MonoBehaviour
{
    Rigidbody rb;
    Vector3 targetVeclocity;
    bool usedSkill = false;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void BeLong()
    {
        SoundMG.Instance.PlaySFX(eSoundType.LongBeLong, transform.position,DominoManager.Instance.longVolume);
        StartCoroutine(LongLong());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Domino") && this.usedSkill == false)
        {
            var crb = collision.rigidbody;
            targetVeclocity = crb.velocity;
            rb.isKinematic = true;

            this.usedSkill = true;

            BeLong();
        }
    }
    IEnumerator LongLong()
    {
        /// ����� ����� one side scale change �θ������Ʈ�� ����� ���� �� ����� Ű���. �ǹ������� �߿�
        while (gameObject.transform.localScale.y < DominoManager.Instance.longLength)
        {
            gameObject.transform.localScale += new Vector3(0, DominoManager.Instance.longspeed, 0);
            yield return new WaitForEndOfFrame();
        }
        rb.isKinematic = false;

        rb.AddForce(targetVeclocity * DominoManager.Instance.forcePower, ForceMode.Force);
    }

}
