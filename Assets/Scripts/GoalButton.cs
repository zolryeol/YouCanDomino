using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum eButtonType
{
    Goal, UnlockGoal, Number, NumSphere, UnlockSphere,
}

public class GoalButton : MonoBehaviour
{
    Transform pushedButton;

    public int needButtonCount; // 이카운트가 0이 되어야만 버튼을 누를 수 있게 활성화됌

    public int number;

    [Header("버튼눌리는속도")]
    public float pushedSpeed = 0.001f;

    [SerializeField]
    eButtonType buttonType;

    bool unlock = false;

    [SerializeField]
    bool numButtonOn = false; // 넘버버튼 전의 버튼이 눌러졌는가 확인용
    public int numButtonCount = 0;
    bool isClear = false;
    GoalButton goal;
    [SerializeField] GoalButton nextButton { get; set; }

    [Header("클리어볼륨크기")]
    [Range(0, 1)] public float clearVolume;
    [Header("풍선갯볼륨")]
    [Range(0, 1)] public float getVolume;
    private void Awake()
    {
        if (buttonType == eButtonType.NumSphere || buttonType == eButtonType.UnlockSphere) { }
        else pushedButton = gameObject.transform.GetChild(0);

        goal = GameObject.Find("SM_GoalButton01").GetComponent<GoalButton>();
    }

    private void Start()
    {
        GoalButton[] _findButtons = FindObjectsOfType<GoalButton>();

        if (this.buttonType == eButtonType.Goal)
        {
            foreach (var findButtone in _findButtons)
            {
                if (findButtone.buttonType == eButtonType.UnlockGoal) needButtonCount++;

                if (findButtone.buttonType == eButtonType.UnlockSphere) needButtonCount++;

                if (findButtone.buttonType == eButtonType.Number) numButtonCount++;

                if (findButtone.buttonType == eButtonType.NumSphere) numButtonCount++;

                if (numButtonCount == 0) numButtonOn = true;
            }
        }

        if (this.buttonType == eButtonType.Number)
        {
            nextButton = goal;

            foreach (var nextB in _findButtons)
            {
                if (nextB.buttonType != eButtonType.Number) continue;

                if (number + 1 == nextB.number)
                {
                    nextButton = nextB;
                }
            }
            if (nextButton)
                Debug.Log("이버튼은 = " + transform.name + " 의 number은 = " + number + "의 다음 버튼은 = " + nextButton.number);
        }

        if (this.buttonType == eButtonType.NumSphere)
        {
            nextButton = goal;

            foreach (var nextB in _findButtons)
            {
                if (nextB.buttonType != eButtonType.NumSphere) continue;

                if (number + 1 == nextB.number)
                {
                    nextButton = nextB;
                }
            }
            if (nextButton)
                Debug.Log("이버튼은 = " + transform.name + " 의 number은 = " + number + "의 다음 버튼은 = " + nextButton.number);
        }

    }

    void NextButtonOn()
    {
        nextButton.numButtonOn = true;
    }

    //     private void Update()
    //     {
    //         if (Input.GetKeyDown(KeyCode.P))
    //         {
    //             StartCoroutine(Clear());
    //         }
    //     }

    public void OnTriggerStay(Collider other)
    {
        //         if (GameManager.Instance.isStarted == false && other.transform.CompareTag("Domino"))
        //         {
        //             other.attachedRigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);
        //             return;
        //         }

        if (other.transform.CompareTag("Domino") && GameManager.Instance.isStarted == false)
        {
            SoundMG.Instance.PlaySFX(eSoundType.ForceOut, transform.position, DominoManager.Instance.forceOutVolume);
            other.attachedRigidbody.AddForce(new Vector3(Random.Range(0, 2), 1, Random.Range(0, 2)) * 3, ForceMode.Impulse);
            return;
        }

        if (other.CompareTag("Domino"))
        {
            switch (buttonType)
            {
                case eButtonType.Goal:
                    {
                        if (needButtonCount == 0 && numButtonOn && isClear == false)
                        {
                            isClear = true;

                            StartCoroutine(ButtonDown());

                            SoundMG.Instance.PlaySFX(eSoundType.ClearStage, transform.position, clearVolume);

                            StartCoroutine(Clear());
                        }
//                         else
//                         {
//                             SoundMG.Instance.PlaySFX(eSoundType.ForceOut, transform.position, DominoManager.Instance.forceOutVolume);
//                             other.GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
//                         }
                        break;
                    }

                case eButtonType.UnlockGoal:
                    {
                        if (!unlock)
                        {
                            StartCoroutine(ButtonDown());
                            UnlockGoalFunc();
                        }
                    }
                    break;

                case eButtonType.UnlockSphere:
                    {
                        if (!unlock)
                        {
                            SoundMG.Instance.PlaySFX(eSoundType.GetSphere, transform.position, getVolume);
                            UnlockGoalFunc();
                            Destroy(this.gameObject);
                        }
                    }
                    break;

                case eButtonType.Number:
                    {
                        if (this.number == 1) numButtonOn = true;

                        if (numButtonOn) // 전에 버튼이 작동된 상태여야만 내것도 작동할것이다
                        {
                            StartCoroutine(ButtonDown());
                            NextButtonOn();
                        }
                        else
                        {
                            SoundMG.Instance.PlaySFX(eSoundType.ForceOut, transform.position, DominoManager.Instance.forceOutVolume);
                            other.GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
                        }
                    }
                    break;

                case eButtonType.NumSphere:
                    {
                        if (this.number == 1) numButtonOn = true;

                        if (numButtonOn) // 전에 버튼이 작동된 상태여야만 내것도 작동할것이다
                        {
                            SoundMG.Instance.PlaySFX(eSoundType.GetSphere, transform.position, getVolume);
                            NextButtonOn();
                            Destroy(this.gameObject);
                        }
                        else
                        {
                            other.GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
                        }
                    }
                    break;
            }
        }
    }
    void UnlockGoalFunc()
    {
        unlock = true;
        goal.needButtonCount -= 1;
    }

    IEnumerator ButtonDown()
    {
        Debug.Log("코루틴 호출");

        WaitForSeconds wfs = new WaitForSeconds(0.05f);

        while (0 < pushedButton.transform.localPosition.y)
        {
            pushedButton.transform.Translate(-Vector3.up * pushedSpeed);

            yield return wfs;
        }

    }

    IEnumerator Clear()
    {

        yield return new WaitForSeconds(1.5f);

        UIMG.Instance.UnlockStage(GameManager.Instance.nowStage);

        UIMG.Instance.afterClear.SetActive(true);

        GameManager.Instance.MouseCursorHide();
        //SceneMG.Instance.NextScene(); // 다음씬으로
    }
}
