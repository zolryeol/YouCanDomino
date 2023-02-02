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

    public int needButtonCount; // ��ī��Ʈ�� 0�� �Ǿ�߸� ��ư�� ���� �� �ְ� Ȱ��ȭ��

    public int number;

    [Header("��ư�����¼ӵ�")]
    public float pushedSpeed = 0.001f;

    [SerializeField]
    eButtonType buttonType;

    bool unlock = false;

    [SerializeField]
    bool numButtonOn = false; // �ѹ���ư ���� ��ư�� �������°� Ȯ�ο�
    public int numButtonCount = 0;
    bool isClear = false;
    GoalButton goal;
    [SerializeField] GoalButton nextButton { get; set; }

    [Header("Ŭ�����ũ��")]
    [Range(0, 1)] public float clearVolume;
    [Header("ǳ��������")]
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
                Debug.Log("�̹�ư�� = " + transform.name + " �� number�� = " + number + "�� ���� ��ư�� = " + nextButton.number);
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
                Debug.Log("�̹�ư�� = " + transform.name + " �� number�� = " + number + "�� ���� ��ư�� = " + nextButton.number);
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

                        if (numButtonOn) // ���� ��ư�� �۵��� ���¿��߸� ���͵� �۵��Ұ��̴�
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

                        if (numButtonOn) // ���� ��ư�� �۵��� ���¿��߸� ���͵� �۵��Ұ��̴�
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
        Debug.Log("�ڷ�ƾ ȣ��");

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
        //SceneMG.Instance.NextScene(); // ����������
    }
}
