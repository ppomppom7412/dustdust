using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class AnimController : MonoBehaviour
{
    public Animator anim;
    Tweener jumpTwn;

    /// <summary>
    /// 트리거형 애님 재생
    /// </summary>
    /// <param name="index"></param>
    public void PlayAnim(int index) 
    {
        if (anim == null) return;

        if (index.Equals(1))
            OnAttack();
        else if (index.Equals(2))
            OnDamage();
        else
            OnJump();
    }

    public void OnDamage()
    {
        anim.SetTrigger("Damage");
    }

    public void OnAttack() 
    {
        anim.SetTrigger("Attack");
    }

    public void OnMove(bool isMove) 
    {
        if (isMove)
            anim.SetFloat("Speed", 5f);
        else
            anim.SetFloat("Speed", 0f);
    }

    public void OnJump() 
    {
        if (jumpTwn != null) {
            jumpTwn.Kill();
            SetvSpeed(0);
        }

        float vSpeed = 0;
        jumpTwn = DOTween.To(() => vSpeed, value => vSpeed = value, 1f, 0.5f)
            .OnUpdate(()=> {
                SetvSpeed(vSpeed);
            })
            .OnComplete(()=> {
            jumpTwn = DOTween.To(() => vSpeed, value => vSpeed = value, 0, 0.5f).OnUpdate(()=> {
                SetvSpeed(vSpeed);
            });
        });
    }

    void SetvSpeed(float value)
    {
        if (value < 0.1f)
        {
            anim.SetBool("IsGrounded", true);
            value = 0;
        }
        else if (value > 0.1f)
        { 
        anim.SetBool("IsGrounded", false);
        }

        anim.SetFloat("vSpeed", value);
        transform.localPosition = Vector3.up * value;
    }

}
