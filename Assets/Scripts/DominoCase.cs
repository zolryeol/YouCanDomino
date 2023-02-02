using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 도미노 케이스
/// 스테이지에 따라 케이스에 차있는 도미노의 종류와 갯수가 다름.
/// 
/// DominoMaker가 생성하면 여기 들어있던 도미노가 메이커로 이동된다.
/// </summary>

public struct DominosTypeAndCount
{
    public int defaultCount;
    public int jetPackCount;
    public int longCount;
    public int bombCount;
    public int inPortalCount;
    public int outPortalCount;
}

public class DominoCase : MonoBehaviour
{
    public DominosTypeAndCount sDominosTypeAndCount = new DominosTypeAndCount();

    Transform spawnPosParent;   // 스폰될위치를 정해둔 오브젝트들의 부모
    Transform[] spawnPosChild;    // 각 도미노들이 소환될 위치

    [SerializeField] List<GameObject> dominosInCasePrefab;
    [Header("도미노케이스 생성속도")]
    [SerializeField]
    float createSpeed = 0.08f;

    public Stack<GameObject>[] stackDominosInCase;

    JsonSerialize jsonSerialize = new JsonSerialize();

    int caseDominoMax;

    void Start()
    {
        createSpeed = 0.08f;

        InitDominoCase();

        Debug.Log("현재스테이지= " + GameManager.Instance.nowStage);
    }
    void InitDominoCase()
    {
        stackDominosInCase = new Stack<GameObject>[5];

        for (int i = 0; i < stackDominosInCase.Length; ++i)
        {
            stackDominosInCase[i] = new Stack<GameObject>();
        }

        spawnPosParent = transform.Find("SpawnPosition").transform;
        spawnPosChild = new Transform[spawnPosParent.childCount];

        for (int i = 0; i < spawnPosParent.childCount; ++i)
        {
            spawnPosChild[i] = spawnPosParent.GetChild(i).transform;
        }

        jsonSerialize.Initialize(this);

        GetDominosCountAccordingToStage(GameManager.Instance.nowStage); // 현재 스테이지에 지정된 도미노 카운트를 확인한다.

        LoadCase(); // 확인했다면 케이스에 생성한다.
    }

    public void LoadCase()
    {
        StartCoroutine(CreateDominosCaseCoroutine(caseDominoMax));
    }

    IEnumerator CreateDominosCaseCoroutine(int _maxR)
    {
        int repeatCount = 0;

        WaitForSeconds ws = new WaitForSeconds(createSpeed);

        while (repeatCount < _maxR)
        {
            if (repeatCount < sDominosTypeAndCount.inPortalCount)
                CreateDominoInCase(eDominoType.InPortal);

            if (repeatCount < sDominosTypeAndCount.bombCount)
                CreateDominoInCase(eDominoType.Bomb);

            if (repeatCount < sDominosTypeAndCount.longCount)
                CreateDominoInCase(eDominoType.LongLong);

            if (repeatCount < sDominosTypeAndCount.jetPackCount)
                CreateDominoInCase(eDominoType.JetPack);

            if (repeatCount < sDominosTypeAndCount.defaultCount)
                CreateDominoInCase(eDominoType.Default);

            repeatCount++;

            yield return ws;
        }
        yield return null;
    }

    void GetDominosCountAccordingToStage(int _nowStage)
    {
        List<int> temp = new List<int>();

        sDominosTypeAndCount.defaultCount = GameManager.Instance.dominoCountForStage[_nowStage][eDominoType.Default]; // 게임매니저의 현재 스테이지 도미노들의 갯수를 파악해서 넣어둔다.
        temp.Add(sDominosTypeAndCount.defaultCount);

        sDominosTypeAndCount.jetPackCount = GameManager.Instance.dominoCountForStage[_nowStage][eDominoType.JetPack];
        temp.Add(sDominosTypeAndCount.jetPackCount);

        sDominosTypeAndCount.longCount = GameManager.Instance.dominoCountForStage[_nowStage][eDominoType.LongLong];
        temp.Add(sDominosTypeAndCount.longCount);

        sDominosTypeAndCount.bombCount = GameManager.Instance.dominoCountForStage[_nowStage][eDominoType.Bomb];
        temp.Add(sDominosTypeAndCount.bombCount);

        sDominosTypeAndCount.inPortalCount = GameManager.Instance.dominoCountForStage[_nowStage][eDominoType.InPortal];
        temp.Add(sDominosTypeAndCount.inPortalCount);

        foreach (var _max in temp)
        {
            if (caseDominoMax < _max)
            {
                caseDominoMax = _max;
                Debug.Log("제일큰값 = " + caseDominoMax);
            }
        }

    }

    public void CreateDominoInCase(eDominoType _type)
    {
        switch (_type)
        {
            case eDominoType.Default:
                var defaultDomino = Instantiate(dominosInCasePrefab[(int)eDominoType.Default], spawnPosChild[0].position, Quaternion.identity, spawnPosChild[0]);
                stackDominosInCase[(int)eDominoType.Default].Push(defaultDomino);
                break;
            case eDominoType.JetPack:
                var jetDomino = Instantiate(dominosInCasePrefab[(int)eDominoType.JetPack], spawnPosChild[1].position, Quaternion.identity, spawnPosChild[1]);
                stackDominosInCase[(int)eDominoType.JetPack].Push(jetDomino);
                break;
            case eDominoType.LongLong:
                var longDomino = Instantiate(dominosInCasePrefab[(int)eDominoType.LongLong], spawnPosChild[2].position, Quaternion.identity, spawnPosChild[2]);
                stackDominosInCase[(int)eDominoType.LongLong].Push(longDomino);
                break;
            case eDominoType.Bomb:
                var bombDomino = Instantiate(dominosInCasePrefab[(int)eDominoType.Bomb], spawnPosChild[3].position, Quaternion.identity, spawnPosChild[3]);
                stackDominosInCase[(int)eDominoType.Bomb].Push(bombDomino);
                break;
            case eDominoType.InPortal:
                var portalDomino = Instantiate(dominosInCasePrefab[(int)eDominoType.InPortal], spawnPosChild[4].position, Quaternion.identity, spawnPosChild[4]);
                stackDominosInCase[(int)eDominoType.InPortal].Push(portalDomino);
                break;
        }
    }

    public void PopAndDestroyInCase(eDominoType _type)
    {
        switch (_type)
        {
            case eDominoType.Default:
                if (stackDominosInCase[(int)eDominoType.Default].Count != 0)
                {
                    var def = stackDominosInCase[(int)eDominoType.Default].Pop();
                    Destroy(def);
                }
                break;

            case eDominoType.JetPack:
                if (stackDominosInCase[(int)eDominoType.JetPack].Count != 0)
                {
                    var jet = stackDominosInCase[(int)eDominoType.JetPack].Pop();
                    Destroy(jet);
                }
                break;

            case eDominoType.LongLong:
                if (stackDominosInCase[(int)eDominoType.LongLong].Count != 0)
                {
                    var longg = stackDominosInCase[(int)eDominoType.LongLong].Pop();
                    Destroy(longg);
                }
                break;

            case eDominoType.Bomb:
                if (stackDominosInCase[(int)eDominoType.Bomb].Count != 0)
                {
                    var bombb = stackDominosInCase[(int)eDominoType.Bomb].Pop();
                    Destroy(bombb);
                }
                break;

            case eDominoType.InPortal:
                if (stackDominosInCase[(int)eDominoType.InPortal].Count != 0)
                {
                    var portal = stackDominosInCase[(int)eDominoType.InPortal].Pop();
                    Destroy(portal);
                }
                break;
        }
    }
}
