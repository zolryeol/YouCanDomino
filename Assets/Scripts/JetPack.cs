using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack : Domino
{

    [SerializeField]
    float movePower = 7;

    Rigidbody rigidB;

    public ParticleSystem smokeParticle;

    public ParticleSystem fireParticle;

    public ParticleSystem fire;

    [SerializeField]
    public Transform[] path = new Transform[3];

    private void Awake()
    {
        isUsedSkill = false;
        rigidB = GetComponent<Rigidbody>();
        //smokeParticle.Stop();
        //fireParticle.Stop();
        // fire.Stop();
    }
    private void Start()
    {
        path[0] = transform.GetChild(1).GetChild(0);
        path[1] = transform.GetChild(1).GetChild(1);
        path[2] = transform.GetChild(1).GetChild(2);

        SetMovePower();
        //OnDrawGizmos();
    }

    public void SetMovePower()
    {
        movePower = DominoManager.Instance.jetPackMovePower;
    }
    void StartJet()
    {
        //         smokeParticle.Play();
        //         fireParticle.Play();
        //fire.Play();
        SoundMG.Instance.PlaySFX(eSoundType.JetPackShoot, transform.position,DominoManager.Instance.jetPackVolume); // ��Ʈ�� ����

        rigidB.AddForce(transform.up * movePower, ForceMode.VelocityChange);

        Debug.Log("����ߴ�");
        ///���� ����ߴ���
        //         iTween.MoveTo(this.gameObject, iTween.Hash("path", path, "movetopath", true, "Time", DominoManager.Instance.needTime, "easetype", DominoManager.Instance.tweenType));
        //         Invoke("OnGravity", DominoManager.Instance.needTime);
    }

    void OnGravity()
    {
        rigidB.useGravity = true;
    }

    void RotateToJet()
    {

        // ������ �� ������ X�� ��ŭ �÷��� �߻��Ѵ�.
    }

    IEnumerator RotaToJet()
    {

        Debug.Log("�ڷ�ƾ����");

        //         Debug.Log(transform.localRotation.eulerAngles.x);
        // 
        //         if (transform.localRotation.eulerAngles.x < 0) // �ڷγѾ�� ���
        //         {
        //             Debug.Log("�ڷ�");
        //             while (transform.localRotation.eulerAngles.x <= -60f)
        //             {
        //                 Debug.Log(transform.localRotation.eulerAngles.x);
        // 
        //                 transform.Rotate(transform.right, -1f);
        //                 yield return new WaitForFixedUpdate();
        //             }
        //         }

        Debug.Log("������");
        // 
        //         while (transform.localRotation.eulerAngles.x > 60)
        //         {
        //             Debug.Log(transform.localRotation.eulerAngles.x);
        // 
        //             transform.Rotate(transform.right, 15, Space.Self);
        // 
        yield return new WaitForFixedUpdate();
    }

    public void DelayedJet()
    {
        Debug.Log("�κ�ũ����");

        //rigidB.useGravity = false;

        Invoke("StartJet", DominoManager.Instance.delayTime);
    }

    protected override void FixedUpdate()
    {
        if (isUsedSkill == false && GetTop().y <= 0.2) // �Ѿ����ٶ�� ġ�� ���� ���
        {
            SoundMG.Instance.PlaySFX(eSoundType.TouchDomino, this.transform.position,DominoManager.Instance.defaultDominoColVolume);

            //Invoke(nameof(DelayedJet), 1f);

            DelayedJet();

            Debug.Log("������");

            isUsedSkill = true;
        }
    }
}
