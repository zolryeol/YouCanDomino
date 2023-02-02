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
        Debug.Log("����Ʈ������");
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
        Debug.Log("Ÿ��Ʋ������");
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

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // �������� �ε��Ѵ�;

        GameManager.Instance.nowStage = SceneManager.GetActiveScene().buildIndex - 1; // ���� ���� �ε���-2�� ���罺������ �ε����̴�

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

        Debug.Log("���� ���������� = " + GameManager.Instance.nowStage);

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
