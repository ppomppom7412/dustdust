using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

public class TweenImage : MonoBehaviour
{
    [Header("Tween")]
    public Tweener myTween;
    [Tooltip("�ݺ� ����")]
    public LoopType loopType;
    [Tooltip("���� �ð�")]
    public float duration;
    [Tooltip("�ִ� ���� ����")]
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
    /// Ʈ�� �����ϱ�
    /// </summary>
    virtual public void TweenPlay()
    {

    }

    /// <summary>
    /// Ʈ�� �ʱ� �������� ���ư���
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

