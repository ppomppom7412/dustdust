using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSlot : MonoBehaviour
{
    public Button slotBtn;
    public Text slottext;
    public TweenImange crackTwImg;
    public SpriteRenderer[] passages;//상좌우하

    public Vector2 mapPoint; //맵상의 x,y좌표
    public int slotindex;//슬롯들의 순서상
    public int damge = 0;

    public void Start()
    {
        crackTwImg.gameObject.SetActive(false);

        GameManager.Instance.ChangeTurn.AddListener(DownDamage);
    }

    public void SetText(string content)
    {
        if (slottext != null)
            slottext.text = content;
    }

    /// <summary>
    /// 버튼 활성화 여부
    /// </summary>
    /// <param name="isOn"></param>
    public void OnOffButton(bool isOn) 
    {
        slotBtn?.gameObject.SetActive(isOn);
    }

    /// <summary>
    /// 클릭 당했을 때 맵 컨트롤러에게 전달한다.
    /// </summary>
    public void ClickThisSlot()
    {
        slotBtn.interactable = false;
        slotBtn.interactable = true;

        MapCotroller.Instance.CallingMapSlot(this);
    }

    /// <summary>
    /// 데미지 입다
    /// </summary>
    [Button]
    public void OnDamage()
    {
        crackTwImg.gameObject.SetActive(false);

        damge = 2;
        crackTwImg.transform.Rotate(Vector3.forward * Random.Range(-360f, 360f));
        crackTwImg.gameObject.SetActive(true);

        //슬롯이 공격 받았을 때 같은 위치에 있는 플레이어 데미지
        for (int i = 0; i < PlayerController.players.Length; ++i)
        {
            if (!slotindex.Equals(PlayerController.players[i].curpos))
                continue;

            PlayerController.players[i].Damaged();
        }
    }

    /// <summary>
    /// 데미지 카운트 다운
    /// </summary>
    [Button]
    public void DownDamage() 
    {
        if (damge <= 0) return;

        // 0 1 2 
        crackTwImg.SetSprite(--damge);

        if (damge < 0)
        {
            damge = 0;
            crackTwImg.gameObject.SetActive(false);

            //같은 위치에 있는 플레이어 가리기
            for (int i = 0; i < PlayerController.players.Length; ++i) 
            {
                if (slotindex.Equals(PlayerController.players[i].curpos))
                    PlayerController.players[i].IsOpen = false;
            }
        }
    }

    /// <summary>
    /// 데미지 리셋
    /// </summary>
    public void ResetDamage() 
    {
        damge = 0;
        crackTwImg.gameObject.SetActive(false);
    }

    /// <summary>
    /// 통로 열기
    /// </summary>
    /// <param name="direction"></param>
    public void OpenPassage(Vector2 direction) 
    {
        if (direction == null) return;

        if (direction.Equals(Vector2.up))
            passages[0].enabled = false;
        else if (direction.Equals(Vector2.left))
            passages[1].enabled = false;
        else if (direction.Equals(Vector2.right))
            passages[2].enabled = false;
        else if (direction.Equals(Vector2.down))
            passages[3].enabled = false;
    }

    /// <summary>
    /// 모든 통로 닫기
    /// </summary>
    public void AllClosePassage() 
    {
        for (int i = 0; i < passages.Length; ++i)
            passages[i].enabled = true;
    }

}
