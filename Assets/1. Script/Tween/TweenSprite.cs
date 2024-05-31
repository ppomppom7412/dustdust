using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

public class TweenSprite : TweenAnimation
{
    [Header("Sprite")]
    SpriteRenderer spRenderer;
    public Sprite[] sprites;
    float curtime;
    float frametime;
    int curframe;

    private void Awake()
    {
        if (spRenderer == null)
            spRenderer = GetComponent<SpriteRenderer>();

        if (sprites == null)
            sprites = new Sprite[1];

        if (sprites != null && spRenderer != null)
            sprites[0] = spRenderer.sprite;
    }

    override public void TweenPlay()
    {
        if (spRenderer == null) return;

        if (myTween != null)
            myTween.Kill();

        if (startEvent != null)
            startEvent.Invoke();

        //한 프레임당 표준 시간
        frametime = duration / sprites.Length;
        curframe = 0;

        myTween = DOTween.To(() => 0f, cur => curtime = cur, duration, duration)
                .SetUpdate(ignoreTimeScale)
                .SetLoops(loopCount, loopType)
                .SetEase(tweenCurve)
                .OnUpdate(() =>
                {
                    //커브를 통한 이미지 변환 속도 변경
                    //tweenCurve.Evaluate(progress)를 통해 커브에 입력된 값 호출
                    // < 단, progress 0~1사이 값만 가진다. >
                    //if (frametime * tweenCurve.Evaluate(progress) > curframe * frametime )

                    //개당 프레임 속도보다 진행되었을 때
                    if (curframe < sprites.Length-1 && frametime * curframe <= curtime)
                        spRenderer.sprite = sprites[++curframe];

                })
                .OnComplete(() =>
                {
                    if (endEvent != null)
                        endEvent.Invoke();
                });
    }

    override public void TweenReset()
    {
        if (spRenderer == null) return;

        if (myTween != null)
            myTween.Kill();

        spRenderer.sprite = sprites[0];
        curframe = 0;
    }
}