using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownUI : MonoBehaviour
{
    public Text timeText;
    public Image fillImage;
    IEnumerator countCorutine;

    public void SetFillValue(float value)
    {
        fillImage.fillAmount = value;
    }

    public void SetTimeText(float time)
    {
        timeText.text = ((int)time).ToString("D2");
    }

    public void StartTimer(int time)
    {
        if (countCorutine != null)
            StopCoroutine(countCorutine);

        gameObject.SetActive(true);
        countCorutine = CountDownTime(time);
        StartCoroutine(countCorutine);
    }

    public void StopTimer() 
    {
        if (countCorutine != null)
            StopCoroutine(countCorutine);

        gameObject.SetActive(false);
    }

    IEnumerator CountDownTime(int time) 
    {
        WaitForSeconds wait01f = new WaitForSeconds(0.1f);

        for (int i = 0; i <= time * 10; ++i)
        {
            //1�ʸ��� ����
            if (i % 10 == 0)
                SetTimeText(time - (i / 10f));

            SetFillValue((i + 0.01f) / (time * 10));

            yield return wait01f;
        }

        //�ð��� �������� �ѱ�� ����
        GameManager.Instance.Action(-1);
    }
}
