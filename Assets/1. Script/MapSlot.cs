using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSlot : MonoBehaviour
{
    public GameObject slotBtn;

    public Vector2 mapPoint; //�ʻ��� x,y��ǥ
    public int slotindex;//���Ե��� ������

    /// <summary>
    /// ��ư Ȱ��ȭ ����
    /// </summary>
    /// <param name="isOn"></param>
    public void OnOffButton(bool isOn) 
    {
        slotBtn?.SetActive(isOn);
    }

    /// <summary>
    /// Ŭ�� ������ �� �� ��Ʈ�ѷ����� �����Ѵ�.
    /// </summary>
    public void ClickThisSlot() 
    {
        MapCotroller.Instance.CallingMapSlot(this);
    }

}
