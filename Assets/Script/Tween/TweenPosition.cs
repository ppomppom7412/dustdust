using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class TweenPosition : TweenAnimation
{
    [Header("Position")]
    [Tooltip("로컬 좌표인지, 월드 좌표인지")]
    public bool isLocal = false;
    public Vector3 startPosition;
    public Vector3 endPosition;

    override public void TweenPlay()
    {
        if (myTween != null)
            myTween.Kill();

        if (isLocal) 
        {
            transform.localPosition = startPosition;

            if (startEvent != null)
                startEvent.Invoke();

            myTween = DOTween.To(() => transform.localPosition, pos => transform.localPosition = pos, endPosition, duration)
                .SetUpdate(ignoreTimeScale)
                .SetLoops(loopCount, loopType)
                .SetEase(tweenCurve)
                .OnComplete(()=> {
                    if (endEvent != null)
                        endEvent.Invoke();
                });
        } 
        else 
        {
            transform.position = startPosition;

            if (startEvent != null)
                startEvent.Invoke();

            myTween = DOTween.To(() => transform.position, pos => transform.position = pos, endPosition, duration)
                .SetUpdate(ignoreTimeScale)
                .SetLoops(loopCount, loopType)
                .SetEase(tweenCurve)
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

        if (isLocal)
            transform.localPosition = startPosition;
        else
            transform.position = startPosition;
    }

}
