using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사운드매니저다
/// 싱글톤
/// </summary>

public enum eSoundType
{
    TouchDomino, CreateDomino, DominoDown, JetPackShoot, LongBeLong,
    BombCount, BombBoom, UIButton, TouchButton, CanonShoot,
    Portal, GetSphere, ClearStage, ForceOut, LoadingComplete,
}

public class SoundMG : MonoBehaviour
{
    private static SoundMG instance = null;
    public static SoundMG Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundMG>();

                if (instance == null) instance = new SoundMG();
            }
            return instance;
        }
    }

    [Header("사운드 발생지")]
    [SerializeField] AudioSource bgmPlayer;
    [SerializeField] AudioSource sfxPlayer;

    [Header("사운드파일 넣는 곳")]
    [SerializeField]
    List<AudioClip> lBgmAudioClip;
    [SerializeField]
    List<AudioClip> lSfxAudioClip;

    public float sizeVolumeBGM = 1f;
    public float sizeVolumeSFX = 1f;

    static bool bgmSwitch = true;
    static bool sfxSwitch = true;



    //Dictionary<string, AudioClip> audioClipsDic = new Dictionary<string, AudioClip>(); //효과음 딕셔너리
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    private void Start()
    {
        if (SceneMG.Instance.GetNowSceneName() == "01_TitleScene") { }
        else if (SceneMG.Instance.GetNowSceneName() == "02_SelectScene") { }
        else { PlayBGM(); }
    }

    public void SetBgmPlayer()
    {
        bgmPlayer = GameObject.Find("DominoMaker").GetComponentInChildren<AudioSource>();
    }
    public void PlayBGM()
    {
        if (bgmSwitch == false) return;

        bgmPlayer.clip = lBgmAudioClip[Random.Range(0, lBgmAudioClip.Count)];

        bgmPlayer.loop = true;

        bgmPlayer.Play();
    }

    public void PlayBGMforSandBox() => StartCoroutine(PlayBGMmultiple());
    IEnumerator PlayBGMmultiple()
    {
        WaitForSeconds wfs = new WaitForSeconds(5.0f);

        bgmPlayer.clip = lBgmAudioClip[Random.Range(0, lBgmAudioClip.Count)];

        bgmPlayer.Play();

        while (true)
        {
            yield return wfs;

            if (!bgmPlayer.isPlaying)
            {
                bgmPlayer.clip = lBgmAudioClip[Random.Range(0, lBgmAudioClip.Count)];
                bgmPlayer.Play();
            }
        }
    }

    public void PlaySFX(eSoundType _eSoundType, Vector3 _sourcePos)
    {
        if (sfxSwitch == false) return;

        AudioSource.PlayClipAtPoint(lSfxAudioClip[(int)_eSoundType], _sourcePos);
    }

    public void PlaySFX(eSoundType _eSoundType, Vector3 _sourcePos, float _soundSize)
    {
        AudioSource.PlayClipAtPoint(lSfxAudioClip[(int)_eSoundType], _sourcePos, _soundSize);
    }

    public void MUTE()
    {
        if (sfxSwitch == true)
            AudioListener.volume = 1;
        else
            AudioListener.volume = 0;
    }

    public void SFXSwitch()
    {
        sfxSwitch = !sfxSwitch;

        MUTE();
    }
    // Update is called once per frame
    //     public void PlaySFXSound(string name, float volume = 1f)
    //     {
    //         if (audioClipsDic.ContainsKey(name) == false)
    //         {
    //             Debug.Log(name + " is not Contained audioClipsDic");
    //             return;
    //         }
    //         sfxPlayer.PlayOneShot(audioClipsDic[name], volume * sizeVolumeSFX);
    //     }
}
