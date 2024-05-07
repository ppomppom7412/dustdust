using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class TweenScale : TweenAnimation
{
    [Header("Scale")]
    public Vector3 startScale;
    public Vector3 endScale;

    override public void TweenPlay()
    {
        if (myTween != null)
            myTween.Kill();

            transform.localScale = startScale;

            if (startEvent != null)
                startEvent.Invoke();

            myTween = DOTween.To(() => transform.localScale, pos => transform.localScale = pos, endScale, duration)
                .SetUpdate(ignoreTimeScale)
                .SetLoops(loopCount, loopType)
                .SetEase(tweenCurve)
                .OnComplete(() => {
                    if (endEvent != null)
                        endEvent.Invoke();
                });
    }

    override public void TweenReset()
    {
        if (myTween != null)
            myTween.Kill();

       transform.localScale = startScale;
    }

}
