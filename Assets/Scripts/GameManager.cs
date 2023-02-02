using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary; // 시리얼라이즈가능한 딕셔너리 에셋
using System.IO;
using System;

[System.Serializable]
public class DominoCountDictionary : SerializableDictionaryBase<eDominoType, int> { } // 시리얼라이즈된 딕셔너리를 사용하기위해 따로 서브클래스를 만들어줘야할 필요가 있었음;

[System.Serializable]   // 세이브로드를 위한 클래스
public class DominoDataForSave
{
    public eDominoType dominoType;
    public int index;
    public string position;
    public string rotation;

    public DominoDataForSave(eDominoType _dominoType, int _index, string _position, string _rotation)
    {
        dominoType = _dominoType;
        index = _index;
        position = _position;
        rotation = _rotation;
    }
}

public class SandboxDataForSave
{
    public eSandBoxObjectType objectType;
    public int objID;
    public string position;
    public string rotation;
    public string scale;

    public SandboxDataForSave(int _objType, string _pos, string _rota, string _scale)
    {
        objectType = (eSandBoxObjectType)_objType;
        position = _pos;
        rotation = _rota;
        scale = _scale;
    }
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    [Header("현재 스테이지")]
    [SerializeField] public int nowStage;

    [SerializeField] public List<DominoCountDictionary> dominoCountForStage = new List<DominoCountDictionary>();

    public delegate void GameManagerInit();
    public GameManagerInit dInit;



    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                if (instance == null) instance = new GameManager();
            }
            return instance;
        }
    }

    public bool isStarted = false;  // 엔터를 눌렀는가 확인용

    [SerializeField]
    Collider[] goalCollider = new Collider[5];

    private bool isCursorVisible = false;

    string path;
    // Update is called once per frame
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
    private void Start()
    {
        Cursor.visible = true;

        if (SceneMG.Instance.GetNowSceneName() == "01_TitleScene") { } // 타이틀 마우스 사용가능하게
        else if (SceneMG.Instance.GetNowSceneName() == "02_SelectScene") { } // 스테이지 선택창 마우스 사용가능하게
        else
        {
            Cursor.visible = false;                     // 마우스 가리기
            Cursor.lockState = CursorLockMode.Locked;
        }

        // 저장관련
        path = Path.Combine(Application.dataPath, "dataBase.Json");
    }

    public void InitGoalCollider()
    {
        var goalChildren = GameObject.Find("SM_Goal_Area01");

        if (goalChildren == null) return;

        // 골인지점에 콜라이더 설정해주기위해
        for (int i = 0; i < goalChildren.transform.childCount; ++i)
        {
            goalCollider[i] = goalChildren.transform.GetChild(i).GetComponent<BoxCollider>();
        }
    }

    public void RemoveGoalCollider()
    {
        var goalArea = GameObject.Find("SM_Goal_Area01");
        goalArea.SetActive(false);
    }

    void Update()
    {
        //Debug.Log("카운트" + dominoCountForStage[0][eDominoType.JetPack]);
        //         if (Input.GetKeyUp(KeyCode.F4))
        //         {
        //             MouseCursorHide();
        //         }

        // 첫번째 도미노를 쓰러뜨린다.
        if (Input.GetKeyUp(KeyCode.Return) && isStarted == false && JsonSerialize.loadingNow == false) // 엔터를 누르고 시작한게 아닐때
        {
            Debug.Log("엔터눌림?");

            var cds = GameObject.Find("CreatedDominos").transform;
            // 세이브
            JsonSerialize.SaveDataToJson(cds); // 도미노들 저장

            if (SceneMG.Instance.GetNowScene().name != "SandBox") // 샌드박스용 예외처리
            {
                RemoveGoalCollider();
                DominoManager.Instance.FirstDominoDown();
            }
            else
            {
                JsonSerialize.SaveOBJForSandBox(); // 샌드박스일때는 샌드박스용 전용 저장
                Debug.Log("샌드박스저장");
                var copyfilter = GameObject.Find("CopyFilter");
                if (copyfilter) copyfilter.GetComponent<MeshRenderer>().enabled = false;
            }

            DominoManager.Instance.DominoManagerInit(); // 시작도미노 활성화용

            isStarted = true;

            UIMG.Instance.ActiveUndoButton();
            UIMG.Instance.OffCameras();

            GameObject.Find("DominoMaker").GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("MakerShadow").GetComponent<MeshRenderer>().enabled = false;
        }

        if ((Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Equals)) && isStarted) TimeFast();

        if ((Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus)) && isStarted) TimeSlow();

        if (Input.GetKeyDown(KeyCode.Escape)) PopUpMenu(); // 메뉴 버튼을 띄우자

        if (Input.GetKeyDown(KeyCode.F1)) UIMG.Instance.HowToPlay();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (UIMG.Instance.LCamera.activeSelf == false) UIMG.Instance.OnLCamera();
            else UIMG.Instance.OffLCamera();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (UIMG.Instance.RCamera.activeSelf == false) UIMG.Instance.OnRCamera();
            else UIMG.Instance.OffRCamera();
        }
    }
    public void Retry()
    {
        JustRestart();

        JsonSerialize.loadingNow = true; // 로드중일땐 생성이 안됌;

        // 로드
        if (SceneMG.Instance.GetNowScene().name == "SandBox")
        {
            StartCoroutine(JsonSerialize.LoadOBJForSandBox());
            StartCoroutine(JsonSerialize.LoadDataFromJsonForSandBox(() => SoundMG.Instance.PlaySFX(eSoundType.LoadingComplete, FindObjectOfType<AudioListener>().transform.position, 0.4f)));
        }
        else StartCoroutine(JsonSerialize.LoadDataFromJson(() => SoundMG.Instance.PlaySFX(eSoundType.LoadingComplete, FindObjectOfType<AudioListener>().transform.position, 0.4f)));
    }
    void TimeFast()
    {
        Time.timeScale += 0.1f;

        UIMG.Instance.EnableSpeedUI();

        float speedUIText = float.Parse(UIMG.Instance.SpeedUI.text);
        speedUIText += 0.1f;
        UIMG.Instance.SpeedUI.text = speedUIText.ToString();

        if (1 <= Time.timeScale)
        {
            Time.timeScale = 1;
            speedUIText = 1f;
            UIMG.Instance.SpeedUI.text = speedUIText.ToString();
        }

        Invoke("UnableSpeedUI", 0.3f);

        Debug.Log("현재 타임스케일 = " + Time.timeScale);
    }
    void TimeSlow()
    {
        Time.timeScale -= 0.1f;

        UIMG.Instance.EnableSpeedUI();

        float speedUIText = float.Parse(UIMG.Instance.SpeedUI.text);
        speedUIText -= 0.1f;
        UIMG.Instance.SpeedUI.text = speedUIText.ToString();

        if (Time.timeScale <= 0.1f)
        {
            Time.timeScale = 0.1f;
            speedUIText = 0.1f;
            UIMG.Instance.SpeedUI.text = speedUIText.ToString();
        }

        Invoke("UnableSpeedUI", 0.3f);

        Debug.Log("현재 타임스케일 = " + Time.timeScale);
    }
    void UnableSpeedUI()
    {
        UIMG.Instance.SpeedUI.gameObject.SetActive(false);
    }
    public void PopUpMenu() // esc 눌러서 팝업메뉴 띄우기
    {
        if (SceneMG.Instance.GetNowSceneName() == "01_TitleScene")
        {
            UIMG.Instance.pauseUI.SetActive(false);
            return;
        }
        else if (SceneMG.Instance.GetNowSceneName() == "02_SelectScene")
        {
            UIMG.Instance.pauseUI.SetActive(false);
            return;
        }
        else
        {
            if (UIMG.Instance.pauseUI.activeInHierarchy) // 팝업창이 이미 온되어있을때 오프시킨다.
            {
                UIMG.Instance.pauseUI.SetActive(false);
                MouseCursorHide();
                Time.timeScale = 1;
            }
            else                                        //  팝업창이 오프되어있을때 온시킨다.
            {
                UIMG.Instance.pauseUI.SetActive(true);
                MouseCursorHide();
                Time.timeScale = 0;
            }
        }
    }
    public void RestartFunc()
    {
        SceneMG.Instance.ReStartNowScene();
        PopUpMenu();
    }
    public void JustRestart()
    {
        SceneMG.Instance.ReStartNowScene();
    }
    public void MouseCursorHide()
    {
        isCursorVisible = !isCursorVisible;

        switch (isCursorVisible)
        {
            case true:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                break;
            case false:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }
}