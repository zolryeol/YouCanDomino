using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButton : MonoBehaviour
{
    [SerializeField] int stageNum;
    [SerializeField] bool isCleared;

    private void Awake()
    {
        stageNum = int.Parse(this.transform.name);
    }

    public void GoStage()
    {
        UIMG.Instance.OffStageSelect();

        SceneMG.Instance.GoSpecificStage(stageNum);
    }
}
