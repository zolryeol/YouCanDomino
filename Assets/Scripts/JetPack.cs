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
        SoundMG.Instance.PlaySFX(eSoundType.JetPackShoot, transform.position,DominoManager.Instance.jetPackVolume); // 제트팩 사운드

        rigidB.AddForce(transform.up * movePower, ForceMode.VelocityChange);

        Debug.Log("출발했다");
        ///에셋 사용했던것
        //         iTween.MoveTo(this.gameObject, iTween.Hash("path", path, "movetopath", true, "Time", DominoManager.Instance.needTime, "easetype", DominoManager.Instance.tweenType));
        //         Invoke("OnGravity", DominoManager.Instance.needTime);
    }

    void OnGravity()
    {
        rigidB.useGravity = true;
    }

    void RotateToJet()
    {

        // 쓰런진 후 각도를 X도 만큼 올려서 발사한다.
    }

    IEnumerator RotaToJet()
    {

        Debug.Log("코루틴시작");

        //         Debug.Log(transform.localRotation.eulerAngles.x);
        // 
        //         if (transform.localRotation.eulerAngles.x < 0) // 뒤로넘어가는 경우
        //         {
        //             Debug.Log("뒤로");
        //             while (transform.localRotation.eulerAngles.x <= -60f)
        //             {
        //                 Debug.Log(transform.localRotation.eulerAngles.x);
        // 
        //                 transform.Rotate(transform.right, -1f);
        //                 yield return new WaitForFixedUpdate();
        //             }
        //         }

        Debug.Log("앞으로");
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
        Debug.Log("인보크시작");

        //rigidB.useGravity = false;

        Invoke("StartJet", DominoManager.Instance.delayTime);
    }

    protected override void FixedUpdate()
    {
        if (isUsedSkill == false && GetTop().y <= 0.2) // 넘어졌다라고 치면 사운드 출력
        {
            SoundMG.Instance.PlaySFX(eSoundType.TouchDomino, this.transform.position,DominoManager.Instance.defaultDominoColVolume);

            //Invoke(nameof(DelayedJet), 1f);

            DelayedJet();

            Debug.Log("쓰러짐");

            isUsedSkill = true;
        }
    }
}
