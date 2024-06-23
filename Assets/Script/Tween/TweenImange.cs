using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TweenImange : TweenAnimation
{
    [SerializeField] Image uiImage;
    [SerializeField] Sprite[] sprites;

    float frameSec;
    float curTime;
    int curIndex;
    int curLoop = -2;

    private void Start()
    {
        uiImage = GetComponent<Image>();
    }

    void OnEnable() 
    {
        curLoop = -2;

        if (isEnable)
            TweenPlay();
    }

    override public void TweenPlay()
    {
        if (uiImage == null || sprites == null) return;

        if (curLoop < -1)
        {
            if (loopCount > 0)
                curLoop = loopCount - 1;
            else //0회 실행 X
                return;
        }

        frameSec = duration / sprites.Length;

        curTime = 0f;
        curIndex = 0;
        uiImage.sprite = sprites[curIndex];

        myTween = DOTween.To(() => curTime, time => curTime = time, duration, duration)
        .SetUpdate(false)
        .OnStart(() => {
            if (startEvent != null)
               startEvent.Invoke();
         })
        .OnUpdate(()=>{
            if (sprites.Length-1 > curIndex )
            {
                if ((curIndex + 1) * frameSec <= curTime)
                    uiImage.sprite = sprites[++curIndex];
            }
        })
        .OnComplete(() => {
            if (endEvent != null)
                endEvent.Invoke();

            if (curLoop < 0 || curLoop > 0)
            {
                --curLoop;
                TweenPlay();
            }
        });
    }

    override public void TweenReset() 
    {
        if (myTween != null)
            myTween.Kill();

        curTime = 0f;
        curIndex = 0;
        uiImage.sprite = sprites[curIndex];
    }

    public void SetSprite(int index)
    {
        if (index < 0)
            index = 0;
        if (index >= sprites.Length)
            index = sprites.Length - 1;

        uiImage.sprite = sprites[index];
    }
}
