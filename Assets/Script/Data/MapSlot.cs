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
    public TweenUIImange crackTwImg;

    public Vector2 mapPoint; //맵상의 x,y좌표
    public int slotindex;//슬롯들의 순서상
    public int damge;

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

    [Button]
    public void OnDamage() 
    {
        crackTwImg.gameObject.SetActive(false);

        damge = 2;
        crackTwImg.transform.Rotate(Vector3.forward * Random.Range(-360f, 360f));
        crackTwImg.gameObject.SetActive(true);
    }

    [Button]
    public void DownDamage() 
    {
        if (damge < 0) return;

        crackTwImg.SetSprite(--damge);

        if (damge < 0)
            crackTwImg.gameObject.SetActive(false);
    }
}
