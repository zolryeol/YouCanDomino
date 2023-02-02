using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 길어져라 여의봉
/// 닿이는 순간 길어지고 잠시후에 넘어진다.
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
        /// 편법을 사용한 one side scale change 부모오브젝트에 빈것을 넣은 후 빈것을 키운다. 피벗설정이 중요
        while (gameObject.transform.localScale.y < DominoManager.Instance.longLength)
        {
            gameObject.transform.localScale += new Vector3(0, DominoManager.Instance.longspeed, 0);
            yield return new WaitForEndOfFrame();
        }
        rb.isKinematic = false;

        rb.AddForce(targetVeclocity * DominoManager.Instance.forcePower, ForceMode.Force);
    }

}
