using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMG : MonoBehaviour
{
    // Start is called before the first frame update
    private static SceneMG instance = null;

    Scene nowSceneInfo;

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

    public static SceneMG Instance
    {
        get { return instance; }
    }

    public void GoStageSelectScene()
    {
        Debug.Log("셀렉트씬으로");
        SceneManager.LoadScene("02_SelectScene");
        UIMG.Instance.OffTitle();
        UIMG.Instance.OnStageSelect();
        UIMG.Instance.OffCameras();
        GameManager.Instance.PopUpMenu();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        GameManager.Instance.isStarted = false;
        //SoundMG.Instance.SetBgmPlayer();
    }

    public void ReStartNowScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        DominoManager.Instance.DominoManagerInit();
        GameManager.Instance.isStarted = false;
        JsonSerialize.loadingNow = false;
        SoundMG.Instance.SetBgmPlayer();
    }
    public string GetNowSceneName() => SceneManager.GetActiveScene().name;


    public void GoTitle()
    {
        Debug.Log("타이틀씬으로");
        SceneManager.LoadScene("01_TitleScene");
        UIMG.Instance.OnTitle();
        UIMG.Instance.OffStageSelect();
        GameManager.Instance.PopUpMenu();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        GameManager.Instance.isStarted = false;
    }

    public void NextScene()
    {
        UIMG.Instance.UnActiveUndoButton();

        JsonSerialize.ResetSave();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // 다음씬을 로드한다;

        GameManager.Instance.nowStage = SceneManager.GetActiveScene().buildIndex - 1; // 현재 씬의 인덱스-2가 현재스테이지 인덱스이다

        GameManager.Instance.isStarted = false;

        SoundMG.Instance.SetBgmPlayer();
    }
    public Scene GetNowScene()
    {
        nowSceneInfo = SceneManager.GetActiveScene();

        GameManager.Instance.isStarted = false;

        return nowSceneInfo;
    }

    //     public void PrevScene()
    //     {
    //         GetNowScene();
    // 
    //         SceneManager.LoadScene(nowSceneInfo.buildIndex - 1);
    //         GameManager.Instance.isStarted = false;
    //         SoundMG.Instance.SetBgmPlayer();
    //     }

    public void GoSpecificStage(int stage)
    {
        UIMG.Instance.UnActiveUndoButton();

        JsonSerialize.ResetSave();

        UIMG.Instance.OffStageSelect();

        SceneManager.LoadScene("Stage_0" + stage);

        GameManager.Instance.nowStage = stage;

        Debug.Log("현재 스테이지는 = " + GameManager.Instance.nowStage);

        Cursor.visible = false;

        Cursor.lockState = CursorLockMode.Locked;

        GameManager.Instance.isStarted = false;
    }

    public void GoSandBoxStage()
    {
        SceneManager.LoadScene("SandBox");
        UIMG.Instance.OffTitle();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
