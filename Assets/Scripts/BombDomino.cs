using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 도미노가 닿으면 폭발한다.
/// </summary>
/// 
public class BombDomino : Domino
{
    SphereCollider childSphere;
    Transform childSphereScale;
    Rigidbody rigidBody;
    Transform createdDomino;

    public bool IsExplosion { get; private set; }
    public GameObject fireEffect;

    private void Awake()
    {
        childSphere = transform.GetChild(0).GetComponent<SphereCollider>();
        childSphereScale = transform.GetChild(1).GetComponent<Transform>();
        dominoType = eDominoType.Bomb;
        fireEffect = transform.GetChild(3).gameObject;
    }
    private void Start()
    {
        childSphere.radius = DominoManager.Instance.radius;
        childSphereScale.localScale = new Vector3(childSphere.radius * 2f, childSphere.radius * 2f, childSphere.radius * 2f);
        rigidBody = transform.GetComponent<Rigidbody>();
        createdDomino = GameObject.Find("CreatedDominos").transform;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!collision.transform.CompareTag("DominoMakerGhost"))
        {
            rigidBody.isKinematic = true;   // 어딘가에 놓이면 굳힌다.
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Domino") && isUsedSkill == false)
        {
            childSphere.enabled = true;

            SoundMG.Instance.PlaySFX(eSoundType.BombCount, transform.position, DominoManager.Instance.boomWatingVolume);

            fireEffect.SetActive(true);

            StartCoroutine(Explosion());

            isUsedSkill = true;
        }
    }

    void ExplosionNotCoroutine()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, DominoManager.Instance.radius);

        foreach (Collider near in colliders)
        {
            Rigidbody nrb = near.GetComponent<Rigidbody>();

            if (nrb != null && nrb.CompareTag("Domino"))
            {
                nrb.AddExplosionForce(DominoManager.Instance.explosionForce, transform.position, DominoManager.Instance.radius);

                if (nrb.gameObject.layer == 7)
                {
                    var _nearBomb = nrb.GetComponent<BombDomino>();
                    if (_nearBomb.IsExplosion)
                    {
                        continue;
                    }

                    IsExplosion = true;
                    _nearBomb.ExplosionNotCoroutine();
                }
                //Debug.Log("범위안에 녀석" + nrb.transform.name);
            }
        }

        Fragmentation();

    }

    IEnumerator Explosion()
    {
        float timer = 0;

        WaitForSeconds ws = new WaitForSeconds(0.1f);

        Collider[] colliders = Physics.OverlapSphere(transform.position, DominoManager.Instance.radius);

        while (timer < DominoManager.Instance.bombTimer)
        {
            //Debug.Log("타이머" + timer);
            timer += 0.1f;
            yield return ws;
        }

        SoundMG.Instance.PlaySFX(eSoundType.BombBoom, transform.position, DominoManager.Instance.boomVolume); // 폭발사운드

        Fragmentation();

        IsExplosion = true;

        foreach (Collider near in colliders)
        {
            Rigidbody nrb = near.GetComponent<Rigidbody>();

            if (nrb != null && nrb.CompareTag("Domino"))
            {
                //                if (nrb.transform.name == this.gameObject.transform.name) continue;

                nrb.AddExplosionForce(DominoManager.Instance.explosionForce, transform.position, DominoManager.Instance.radius);

                Debug.Log("범위안에 녀석" + nrb.transform.name);

                if (nrb.gameObject.layer == 7)
                {
                    Debug.Log("bomb가 있나?" + this.gameObject.layer);
                    var _nearBomb = nrb.GetComponent<BombDomino>();

                    _nearBomb.ExplosionNotCoroutine();
                }
            }
        }
    }

    /// 넘어지면 쓰러지는 버전
    //     protected override void FixedUpdate()
    //     {
    //         if (GetTop().y <= 0.2 && isUsedSkill == false) // 넘어졌다라고 치면 사운드 출력
    //         {
    //             SoundMG.Instance.PlaySFX(eSoundType.TouchDomino, this.transform.position);
    // 
    //             StartCoroutine(Explosion());
    // 
    //             isUsedSkill = true;
    //         }
    //     }
    void Fragmentation() // [분열] 폭발시 파편화
    {
        GameObject debris = Instantiate(DominoManager.Instance.bombDebris, transform.position, transform.rotation, createdDomino);
        Destroy(gameObject);
        Rigidbody[] debriRigidBody = debris.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in debriRigidBody)
        {
            rb.AddExplosionForce(DominoManager.Instance.explosionForce, transform.position, DominoManager.Instance.radius);
            //rb.AddForce(Vector3.up * DominoManager.Instance.debrisExplosionForce, ForceMode.Impulse);
        }
    }
}
