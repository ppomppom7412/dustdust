using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Slider))]
public class ColorSlider : MonoBehaviour
{
    [SerializeField] Gradient gradient;
    [SerializeField] Slider slider;
    [SerializeField] Image targetImage;

    float progress;

    public void Start()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        //�� ��ȭ�� ���� ��ȭ�ڵ� �߰�
        if (slider != null)
            slider.onValueChanged.AddListener((value)=> 
            {
                if (slider.maxValue != 0)
                    progress = slider.value / slider.maxValue;
                else
                    progress = (slider.value + 1f) / (slider.maxValue + 1f);

                if (targetImage != null)
                    targetImage.color = gradient.Evaluate(progress);
            });
    }

    [NaughtyAttributes.Button]
    public void SetUp() 
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        if (targetImage == null)
            targetImage = slider.fillRect.GetComponent<Image>();
    }
}
