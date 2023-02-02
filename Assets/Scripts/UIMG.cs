using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 유아이를 담당하는 매니저
/// </summary>
public class UIMG : MonoBehaviour
{
    private static UIMG instance = null;

    public GameObject titleUI;

    public GameObject stageSelect;

    public GameObject pauseUI;

    public GameObject optionUI;

    public GameObject howToPlay;

    public Text SpeedUI;

    public GameObject LCamera;
    public GameObject RCamera;

    public GameObject afterClear;

    Event eventData;
    public static UIMG Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIMG>();

                if (instance == null) instance = new UIMG();
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
        SetCameras();
        OffCameras();
    }

    private void Start()
    {
        UnlockStageStart();
    }

    public void HowToPlay()
    {
        if(howToPlay.activeSelf)
        {
            howToPlay.SetActive(false);
        }
        else
        {
            howToPlay.SetActive(true);
        }
    }

    public void EnableSpeedUI() => SpeedUI.gameObject.SetActive(true);

    public void SetCameras()
    {
        LCamera = GameObject.Find("LeftSideCamera").gameObject;
        RCamera = GameObject.Find("RightSideCamera").gameObject;
    }

    public void OffCameras()
    {
        LCamera.SetActive(false);
        RCamera.SetActive(false);
    }


    public void OnLCamera() => LCamera.SetActive(true);
    public void OffLCamera() => LCamera.SetActive(false);
    public void OnRCamera() => RCamera.SetActive(true);
    public void OffRCamera() => RCamera.SetActive(false);

    public void OffTitle()
    {
        titleUI.SetActive(false);
    }
    public void OffStageSelect()
    {
        stageSelect.SetActive(false);
    }
    public void OnTitle()
    {
        titleUI.SetActive(true);
    }
    public void OnStageSelect()
    {
        stageSelect.SetActive(true);
    }

    public void ActiveUndoButton()
    {
        pauseUI.transform.GetChild(4).GetComponent<Button>().interactable = true;
    }

    public void UnActiveUndoButton()
    {
        pauseUI.transform.GetChild(4).GetComponent<Button>().interactable = false;
    }

    public void UnlockStage(int _nowStageIndex)
    {
        Button[] layOutButtons = stageSelect.GetComponentsInChildren<Button>();

        layOutButtons[_nowStageIndex].interactable = true;

        PlayerPrefs.SetInt("UnlockStage", _nowStageIndex);

        PlayerPrefs.Save();
    }
    public void UnlockStageStart()
    {
        int _highestStage = PlayerPrefs.GetInt("UnlockStage");

        for (int i = 1; i <= _highestStage; ++i)
        {
            UnlockStage(i);
        }
    }
    public void ExitApp()
    {
        Application.Quit();
    }

    public void OnOptionButton()
    {
        if (optionUI.activeSelf)
            optionUI.SetActive(false);
        else optionUI.SetActive(true);
    }

    public void AfterClearOff()
    {
        afterClear.SetActive(false);
    }
}
