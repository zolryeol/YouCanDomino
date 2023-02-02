using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// �ΰ� �� ���̵�ƿ�
/// </summary>

public class FadeInOut : MonoBehaviour
{
    public Image fadePanel;

    bool isFadeDone = false;

    private void Awake()
    {
        fadePanel = GameObject.Find("Panel").GetComponent<Image>();
    }

    private void Start()
    {
        Invoke("FadeIn", 0.5f);
    }

    public void GoTitle()
    {
        SceneManager.LoadScene("01_TitleScene");
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        float fadeCount = 0; // ���İ�

        WaitForSeconds wfs = new WaitForSeconds(0.01f);

        while (fadeCount <= 1.0f) // 255 ���ƴ϶� 0~1���̰����� ����.
        {
            fadeCount += 0.01f;
            yield return wfs; // 0.01 �ʸ��� �����ų���̴�.

            fadePanel.color = new Color(0, 0, 0, fadeCount);
        }

        GoTitle();
    }


    IEnumerator FadeInCoroutine()
    {
        float fadeCount = 1; // ���İ�

        WaitForSeconds wfs = new WaitForSeconds(0.01f);

        while (fadeCount >= 0.0f) // 255 ���ƴ϶� 0~1���̰����� ����.
        {
            fadeCount -= 0.01f;
            yield return wfs; // 0.01 �ʸ��� �����ų���̴�.

            fadePanel.color = new Color(0, 0, 0, fadeCount);
        }

        FadeOut();
    }
}
