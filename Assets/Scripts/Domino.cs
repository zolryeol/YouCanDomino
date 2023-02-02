using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 도미노들에게 붙어있을 스크립트.
/// </summary>

public enum eDominoType
{
    Default, JetPack, LongLong, Bomb, InPortal, OutPortal,
}

public class Domino : MonoBehaviour
{
    [SerializeField]
    protected eDominoType dominoType;

    bool isFallDown = false; // 쓰러졌는가?

    Vector3 topVector = Vector3.up;

    protected bool isUsedSkill = false;

    [Header("스테이지용 도미노인가")]
    [SerializeField]
    public bool isStageDomin = false;

    private void Start()
    {
        SetDominoType(dominoType);
    }

    public eDominoType GetDominoType()
    {
        return dominoType;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Domino"))
        {
            float volumeSize = collision.relativeVelocity.magnitude / 10.0f;
            SoundMG.Instance.PlaySFX(eSoundType.TouchDomino, transform.position, DominoManager.Instance.defaultDominoColVolume);
        }
    }

    public void SetDominoType(eDominoType _dominoType)
    {
        dominoType = _dominoType;

        switch (_dominoType)
        {
            case eDominoType.Default: break;    // addcomponent를 이용하여 기능을 넣어주자

            case eDominoType.JetPack:
                var jetPack = gameObject.AddComponent<JetPack>();
                jetPack.SetMovePower();
                break;
            case eDominoType.LongLong:
                var longlongDomino = gameObject.AddComponent<LongLongDomino>();
                break;
            case eDominoType.Bomb:
                var bombDomino = gameObject.AddComponent<BombDomino>();
                break;
            case eDominoType.InPortal: // 인포탈을 생성한다.
                {
                    Debug.Log("인포탈");
                    var portalDomino = gameObject.AddComponent<PortalDomino>();
                    portalDomino.isIn = true; // 인포탈으로 셋
                    DominoManager.Instance.llPortalsLink.AddLast(portalDomino);
                }
                break;
            case eDominoType.OutPortal: // 아웃포탈을 생성한다.
                {
                    Debug.Log("아웃포탈");
                    var _outportal = gameObject.AddComponent<PortalDomino>();
                    _outportal.isIn = false; // 아웃포탈 셋
                    DominoManager.Instance.llPortalsLink.Last.Value.outPortal = _outportal.gameObject;
                }
                break;
        }
    }

    protected virtual void FixedUpdate()
    {
        //         if (GetTop().y <= 0.2 && isUsedSkill == false) // 넘어졌다라고 치면 사운드 출력
        //         {
        //             SoundMG.Instance.PlaySFX(eSoundType.TouchDomino, this.transform.position);
        //             isUsedSkill = true;
        //         }
    }

    public Vector3 GetTop()
    {
        // 현재 도미노의 월드상 꼭대기를 구한다.
        float height = this.gameObject.GetComponent<MeshCollider>().bounds.size.y; // 높이를 구한다.

        Vector3 vectorHeight;
        vectorHeight.x = 0;
        vectorHeight.y = height;
        vectorHeight.z = 0;

        //         transform.TransformPoint(vectorHeight);
        //         Debug.Log("현재 top = " + height);

        //Debug.Log("현재 꼭대기 = " + vectorHeight);

        return vectorHeight;
    }
}
