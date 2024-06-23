using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class UserNameUI : MonoBehaviour
{
    [Header("UIs")]
    public GameObject turnObject;   //나의 턴 표기
    public Text nameText;
    public Image[] heathImages;
    public Image[] manaImages;

    [Header("Sprite")]
    public ImageLibraryAnimator animator;
    public CharacterSpriteBulider builder;
    public Sprite defaultSprite;

    [Header("Colors")]
    public Color noneColor;
    public Color manaColor;
    public Color heathColor;
    public Color shieldColor;

    Tween damageTwn;
    Tween hurtTwn;

    /// <summary>
    /// 초기 상태로 세팅
    /// </summary>
    public void InitState() 
    {
        for (int m = 0; m < manaImages.Length; ++m)
        {
            manaImages[m].color = noneColor;
        }

        for (int m = 0; m < heathImages.Length; ++m)
        {
            heathImages[m].color = heathColor;
        }

        animator.image.sprite = defaultSprite;
    }

    /// <summary>
    /// 상태에 따른 표기 변화
    /// </summary>
    /// <param name="state"></param>
    public void SyncState(PlayerState state)
    {
        for (int m = 0; m < manaImages.Length; ++m) 
        {
            if (state.curMn > m) 
                manaImages[m].color = manaColor;
            else
                manaImages[m].color = noneColor;
        }

        for (int h = 0; h < heathImages.Length; ++h)
        {
            if (state.curHp > h)
                heathImages[h].color = heathColor;
            else
                heathImages[h].color = noneColor;
        }

        for (int s = state.curHp; s > 0; --s)
        {
            if (state.curSd > (state.curHp - s))
                heathImages[s].color = shieldColor;
        }
    }

    /// <summary>
    /// 유저네임카드의 초기 설정
    /// </summary>
    /// <param name="name"></param>
    /// <param name="is_me"></param>
    public void SetUserNameUI(string name, bool is_me) 
    {
        nameText.text = name;
        turnObject.SetActive(false);

        if (is_me)
            nameText.color = new Color(1f, 0.877f,0f,1f);
        else
            nameText.color = Color.white;
    }

    /// <summary>
    /// 유저네임카드의 턴 표기 설정
    /// </summary>
    /// <param name="myturn"></param>
    public void SetTurn(bool myturn) 
    {
        turnObject.SetActive(myturn);
    }

    /// <summary>
    /// 하트(체력UI) 흔들기
    /// </summary>
    /// <param name="index"></param>
    public void ShakeHeart(int index) 
    {
        //기존에 진행하던 것이 있으면 종료위치로 이동하기
        if (damageTwn != null)
            damageTwn.Kill(true);

        if (index < 0)
            index = 0;
        else if (index >= heathImages.Length)
            index = heathImages.Length-1;

        Vector3 uppos = heathImages[index].transform.position + (Vector3.up * 0.1f);

        damageTwn = DOTween.To(
            () => heathImages[index].transform.position,
            movepos => heathImages[index].transform.position = movepos
            , uppos, 0.1f)
            .SetLoops(2, LoopType.Yoyo);
    }

    /// <summary>
    /// 데이터 기반으로 프로필 캐릭터 변경
    /// </summary>
    /// <param name="data"></param>
    public void SetProfileCharacter(string data) 
    {
        builder.RebuildToString(data);
    }

    /// <summary>
    /// 프로필 캐릭터 애니메이션 출력
    /// </summary>
    /// <param name="motion"></param>
    public void PlayAnim(ImageLibraryAnimator.MotionList motion) 
    {
        if (motion.Equals(ImageLibraryAnimator.MotionList.Death))
        {
            animator.PlayAnim(motion.ToString(), false, 10f, 0.2f, false);
        }
        else if (motion.Equals(ImageLibraryAnimator.MotionList.Hurt)) 
        {
            if (hurtTwn != null)
                hurtTwn.Kill(true);

            animator.image.color = Color.white;
            hurtTwn = DOTween.To(() => animator.image.color, redcolor => animator.image.color = redcolor, Color.red, 0.2f).SetLoops(2, LoopType.Yoyo);
        }
        else if (motion.Equals(ImageLibraryAnimator.MotionList.Run) || motion.Equals(ImageLibraryAnimator.MotionList.Roll) )
        {
            animator.PlayAnim(motion.ToString(), true, 3f, 0.2f, true);
        }
        else
        {
            animator.PlayAnim(motion.ToString());
        }
    }
}
