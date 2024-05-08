using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownUI : MonoBehaviour
{
    public Text timeText;
    public Image fillImage;

    public void SetFillValue(float value)
    {
        fillImage.fillAmount = value;
    }

    public void SetTimeText(float time)
    {
        timeText.text = ((int)time).ToString("D2");
    }
}
