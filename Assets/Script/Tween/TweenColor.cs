using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TweenColor : TweenAnimation
{
    [Header("Color")]
    public Gradient gradient;

    SpriteRenderer sprite;
    Image image;

    float progress;

    private void Start()
    {
        image = GetComponent<Image>();
        sprite = GetComponent<SpriteRenderer>();
    }

    override public void TweenPlay()
    {
        progress = 0f;

        if (image != null)
        {
            myTween = DOTween.To(() => progress, cur => progress = cur, 1f, duration)
            .SetUpdate(ignoreTimeScale)
            .SetLoops(loopCount, loopType)
            //.SetEase(tweenCurve)
            .OnStart(() => {
                if (startEvent != null)
                    startEvent.Invoke();
            })
            .OnUpdate(() =>
            {
                //커브가 적용된 현재 값을 적용한 컬러
                image.color = gradient.Evaluate((tweenCurve.Evaluate(progress)));
            })
            .OnComplete(() => {
                if (endEvent != null)
                    endEvent.Invoke();
            });
        }
        else if (sprite != null)
        {
            myTween = DOTween.To(() => progress, cur => progress = cur, 1f, duration)
            .SetUpdate(ignoreTimeScale)
            .SetLoops(loopCount, loopType)
            //.SetEase(tweenCurve)
            .OnStart(() => {
                if (startEvent != null)
                    startEvent.Invoke();
            })
            .OnUpdate(() =>
            {
                //커브가 적용된 현재 값을 적용한 컬러
                sprite.color = gradient.Evaluate((tweenCurve.Evaluate(progress)));
            })
            .OnComplete(() => {
                if (endEvent != null)
                    endEvent.Invoke();
            });
        }
    }

    override public void TweenReset()
    {
        if (myTween != null)
            myTween.Kill();

        if (image != null)
            image.color = gradient.Evaluate(0f);
        else if (sprite != null)
            sprite.color = gradient.Evaluate(0f);
    }

}
