using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

public class TweenAnimation : MonoBehaviour
{
    [Header("Tween")]
    public Tweener myTween;
    [Tooltip("반복 횟수")]
    public int loopCount = 1;//-1 무한
    [Tooltip("반복 종류")]
    public LoopType loopType;
    [Tooltip("진행 시간")]
    public float duration;
    [Tooltip("애님 진행 형태")]
    public AnimationCurve tweenCurve;

    [Header("Setting")]
    public bool ignoreTimeScale = false;
    public bool isEnable = false;

    [Header("Event")]
    public UnityEvent startEvent;
    public UnityEvent endEvent;

    void Start()
    {
        TweenPlay();
    }

    void OnEnable()
    {
        if (isEnable)
            TweenPlay();
    }

    private void OnDisable()
    {
        TweenStop();
    }

    /// <summary>
    /// 트윈 시작하기
    /// </summary>
    virtual public void TweenPlay()
    {

    }

    /// <summary>
    /// 트윈 초기 지점으로 돌아가기
    /// </summary>
    virtual public void TweenReset()
    {
        if (myTween != null)
            myTween.Kill();
    }

    public void TweenStop(bool complete = false)
    {
        if (myTween != null)
            myTween.Kill(complete);
    }
}
