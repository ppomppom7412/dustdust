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

        //�� �����Ӵ� ǥ�� �ð�
        frametime = duration / sprites.Length;
        curframe = 0;

        myTween = DOTween.To(() => 0f, cur => curtime = cur, duration, duration)
                .SetUpdate(ignoreTimeScale)
                .SetLoops(loopCount, loopType)
                .SetEase(tweenCurve)
                .OnUpdate(() =>
                {
                    //Ŀ�긦 ���� �̹��� ��ȯ �ӵ� ����
                    //tweenCurve.Evaluate(progress)�� ���� Ŀ�꿡 �Էµ� �� ȣ��
                    // < ��, progress 0~1���� ���� ������. >
                    //if (frametime * tweenCurve.Evaluate(progress) > curframe * frametime )

                    //���� ������ �ӵ����� ����Ǿ��� ��
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