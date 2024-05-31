using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSlot : MonoBehaviour
{
    public GameObject slotBtn;

    public Vector2 mapPoint; //맵상의 x,y좌표
    public int slotindex;//슬롯들의 순서상

    /// <summary>
    /// 버튼 활성화 여부
    /// </summary>
    /// <param name="isOn"></param>
    public void OnOffButton(bool isOn) 
    {
        slotBtn?.SetActive(isOn);
    }

    /// <summary>
    /// 클릭 당했을 때 맵 컨트롤러에게 전달한다.
    /// </summary>
    public void ClickThisSlot() 
    {
        MapCotroller.Instance.CallingMapSlot(this);
    }

}
