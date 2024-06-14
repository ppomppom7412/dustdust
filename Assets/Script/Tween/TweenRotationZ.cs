using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class TweenRotationZ : TweenAnimation
{
    [Header("RotationZ")]
    public float startZ;
    public float endZ;
    float progress;

    override public void TweenPlay()
    {
        if (myTween != null)
            myTween.Kill();

        if (startEvent != null)
            startEvent.Invoke();

        myTween = DOTween.To(() => 0f, pos => progress = pos, 1f, duration)
            .SetUpdate(ignoreTimeScale)
            .SetLoops(loopCount, loopType)
            .SetEase(tweenCurve)
            .OnUpdate(() => {
                transform.rotation = Quaternion.Euler(0, 0, tweenCurve.Evaluate(progress) * endZ);
            })
            .OnComplete(() => {
                if (endEvent != null)
                    endEvent.Invoke();
            });
    }

    override public void TweenReset()
    {
        if (myTween != null)
            myTween.Kill();

        transform.rotation = Quaternion.Euler(0, 0, startZ);
    }

}