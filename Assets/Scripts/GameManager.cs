using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary; // �ø����������� ��ųʸ� ����
using System.IO;
using System;

[System.Serializable]
public class DominoCountDictionary : SerializableDictionaryBase<eDominoType, int> { } // �ø��������� ��ųʸ��� ����ϱ����� ���� ����Ŭ������ ���������� �ʿ䰡 �־���;

[System.Serializable]   // ���̺�ε带 ���� Ŭ����
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

    [Header("���� ��������")]
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

    public bool isStarted = false;  // ���͸� �����°� Ȯ�ο�

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

        if (SceneMG.Instance.GetNowSceneName() == "01_TitleScene") { } // Ÿ��Ʋ ���콺 ��밡���ϰ�
        else if (SceneMG.Instance.GetNowSceneName() == "02_SelectScene") { } // �������� ����â ���콺 ��밡���ϰ�
        else
        {
            Cursor.visible = false;                     // ���콺 ������
            Cursor.lockState = CursorLockMode.Locked;
        }

        // �������
        path = Path.Combine(Application.dataPath, "dataBase.Json");
    }

    public void InitGoalCollider()
    {
        var goalChildren = GameObject.Find("SM_Goal_Area01");

        if (goalChildren == null) return;

        // ���������� �ݶ��̴� �������ֱ�����
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
        //Debug.Log("ī��Ʈ" + dominoCountForStage[0][eDominoType.JetPack]);
        //         if (Input.GetKeyUp(KeyCode.F4))
        //         {
        //             MouseCursorHide();
        //         }

        // ù��° ���̳븦 �����߸���.
        if (Input.GetKeyUp(KeyCode.Return) && isStarted == false && JsonSerialize.loadingNow == false) // ���͸� ������ �����Ѱ� �ƴҶ�
        {
            Debug.Log("���ʹ���?");

            var cds = GameObject.Find("CreatedDominos").transform;
            // ���̺�
            JsonSerialize.SaveDataToJson(cds); // ���̳�� ����

            if (SceneMG.Instance.GetNowScene().name != "SandBox") // ����ڽ��� ����ó��
            {
                RemoveGoalCollider();
                DominoManager.Instance.FirstDominoDown();
            }
            else
            {
                JsonSerialize.SaveOBJForSandBox(); // ����ڽ��϶��� ����ڽ��� ���� ����
                Debug.Log("����ڽ�����");
                var copyfilter = GameObject.Find("CopyFilter");
                if (copyfilter) copyfilter.GetComponent<MeshRenderer>().enabled = false;
            }

            DominoManager.Instance.DominoManagerInit(); // ���۵��̳� Ȱ��ȭ��

            isStarted = true;

            UIMG.Instance.ActiveUndoButton();
            UIMG.Instance.OffCameras();

            GameObject.Find("DominoMaker").GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("MakerShadow").GetComponent<MeshRenderer>().enabled = false;
        }

        if ((Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Equals)) && isStarted) TimeFast();

        if ((Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus)) && isStarted) TimeSlow();

        if (Input.GetKeyDown(KeyCode.Escape)) PopUpMenu(); // �޴� ��ư�� �����

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

        JsonSerialize.loadingNow = true; // �ε����϶� ������ �ȉ�;

        // �ε�
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

        Debug.Log("���� Ÿ�ӽ����� = " + Time.timeScale);
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

        Debug.Log("���� Ÿ�ӽ����� = " + Time.timeScale);
    }
    void UnableSpeedUI()
    {
        UIMG.Instance.SpeedUI.gameObject.SetActive(false);
    }
    public void PopUpMenu() // esc ������ �˾��޴� ����
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
            if (UIMG.Instance.pauseUI.activeInHierarchy) // �˾�â�� �̹� �µǾ������� ������Ų��.
            {
                UIMG.Instance.pauseUI.SetActive(false);
                MouseCursorHide();
                Time.timeScale = 1;
            }
            else                                        //  �˾�â�� �����Ǿ������� �½�Ų��.
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