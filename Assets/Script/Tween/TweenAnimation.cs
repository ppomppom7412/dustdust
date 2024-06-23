using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;
using NaughtyAttributes;

public class TweenAnimation : MonoBehaviour
{
    public Tweener myTween;

    //[HorizontalLine(color: EColor.Violet)]
    [Header("Setting")]
    [Tooltip("진행 시간")]
    public float duration= 1f;//진행 시간
    [Tooltip("반복 횟수")]
    public int loopCount = 1;//-1 무한
    [Tooltip("반복 종류")]
    public LoopType loopType = LoopType.Restart;
    //Restart : 기본적인 일자형 루프 / 루프 시작시 처음으로 되돌린다.
    //Yoyo : 되돌아오는 루프 / 진행1회 되돌아옴1회로 구분한다.
    //Incremental : 마지막을 기준으로 해당 루프 진행 / 움직이는 경우 계속 앞으로 간다던가의 경우
    [Tooltip("애님 진행 형태")]
    public AnimationCurve tweenCurve = AnimationCurve.Linear(0f,0f,1f,1f);
    [Tooltip("시간값에 따른 진행 여부")]
    public bool ignoreTimeScale = false;
    [Tooltip("OnEnable에 따른 실행 여부")]
    public bool isEnable = true;

    [Foldout("onEvent")]
    //시작시 실행되는 이벤트
    public UnityEvent startEvent;
    [Foldout("onEvent")]
    //종료시 실행되는 이벤트
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
