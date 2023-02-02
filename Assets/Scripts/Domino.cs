using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ���̳�鿡�� �پ����� ��ũ��Ʈ.
/// </summary>

public enum eDominoType
{
    Default, JetPack, LongLong, Bomb, InPortal, OutPortal,
}

public class Domino : MonoBehaviour
{
    [SerializeField]
    protected eDominoType dominoType;

    bool isFallDown = false; // �������°�?

    Vector3 topVector = Vector3.up;

    protected bool isUsedSkill = false;

    [Header("���������� ���̳��ΰ�")]
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
            case eDominoType.Default: break;    // addcomponent�� �̿��Ͽ� ����� �־�����

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
            case eDominoType.InPortal: // ����Ż�� �����Ѵ�.
                {
                    Debug.Log("����Ż");
                    var portalDomino = gameObject.AddComponent<PortalDomino>();
                    portalDomino.isIn = true; // ����Ż���� ��
                    DominoManager.Instance.llPortalsLink.AddLast(portalDomino);
                }
                break;
            case eDominoType.OutPortal: // �ƿ���Ż�� �����Ѵ�.
                {
                    Debug.Log("�ƿ���Ż");
                    var _outportal = gameObject.AddComponent<PortalDomino>();
                    _outportal.isIn = false; // �ƿ���Ż ��
                    DominoManager.Instance.llPortalsLink.Last.Value.outPortal = _outportal.gameObject;
                }
                break;
        }
    }

    protected virtual void FixedUpdate()
    {
        //         if (GetTop().y <= 0.2 && isUsedSkill == false) // �Ѿ����ٶ�� ġ�� ���� ���
        //         {
        //             SoundMG.Instance.PlaySFX(eSoundType.TouchDomino, this.transform.position);
        //             isUsedSkill = true;
        //         }
    }

    public Vector3 GetTop()
    {
        // ���� ���̳��� ����� ����⸦ ���Ѵ�.
        float height = this.gameObject.GetComponent<MeshCollider>().bounds.size.y; // ���̸� ���Ѵ�.

        Vector3 vectorHeight;
        vectorHeight.x = 0;
        vectorHeight.y = height;
        vectorHeight.z = 0;

        //         transform.TransformPoint(vectorHeight);
        //         Debug.Log("���� top = " + height);

        //Debug.Log("���� ����� = " + vectorHeight);

        return vectorHeight;
    }
}
