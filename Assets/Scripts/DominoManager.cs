using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  도미노 프리팹을 관리함
/// </summary>

public class DominoManager : MonoBehaviour
{
    private static DominoManager instance = null;

    [Header("도미노프리팹")]
    public List<Domino> lDominoPrefab = new List<Domino>();

    [Header("쓰러뜨릴 첫번째 도미노")]
    private GameObject FirstDomino;

    [Header("첫번째 도미노 쓰러뜨리는 힘")]
    [SerializeField]
    private int firstDominoPower = 50;
    private bool FirstDominoDownOnlyOnce = false;

    [Header("제트팩 관련")]
    [Range(0, 10000)]
    public float jetPackMovePower;

    //public iTween.EaseType tweenType; //에셋 사용했던것
    [SerializeField]
    public float delayTime = 0.01f;
//     [SerializeField]
//     public float needTime = 5f;

    [Header("LongDomino 길어지는 길이")]
    [SerializeField]
    public float longLength = 3;
    [SerializeField]
    public float forcePower = 2;
    [SerializeField]
    public float longspeed = 0.03f;

    [Header("BombDomino 관련")]
    [SerializeField]
    public float explosionForce = 150;
    public float radius = 0.5f;
    public float bombTimer = 2.0f;
    public GameObject bombDebris;
    public float debrisExplosionForce = 5;
    

    [Header("PortalDomino 관련")]
    [SerializeField]
    public float shrinkSize = 0.01f;
    public LinkedList<PortalDomino> llPortalsLink = new LinkedList<PortalDomino>(); // 포탈연결

    [Header("사운드크기")]
    [Range(0, 1)]
    public float defaultDominoColVolume;
    [Range(0, 1)]
    public float jetPackVolume;
    [Range(0, 1)]
    public float longVolume;
    [Range(0, 1)]
    public float boomVolume;
    [Range(0, 1)]
    public float boomWatingVolume;
    [Range(0, 1)]
    public float potalvolume;
    [Range(0, 1)]
    public float forceOutVolume;
    [Range(0, 1)]
    public float createDominoVolume;


    public static DominoManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DominoManager>();

                if (instance == null) instance = new DominoManager();
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DominoManagerInit()
    {
        FirstDominoDownOnlyOnce = false;
    }

    public void FirstDominoDown()
    {
        FirstDomino = GameObject.Find("Start Domino");

        if (FirstDominoDownOnlyOnce == false)
        {
            FirstDomino.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * firstDominoPower);
            FirstDominoDownOnlyOnce = true;
        }
    }
}
