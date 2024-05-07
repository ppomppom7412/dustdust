using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserNameUI : MonoBehaviour
{
    public Text nameText;
    public GameObject turnObject;
    public GameObject meImage; //나일 경우 아이콘 표기

    /// <summary>
    /// 유저네임카드의 초기 설정
    /// </summary>
    /// <param name="name"></param>
    /// <param name="is_me"></param>
    public void SetUserNameUI(string name, bool is_me) 
    {
        nameText.text = name;
        turnObject.SetActive(false);

        meImage.SetActive(is_me);
    }

    /// <summary>
    /// 유저네임카드의 턴 표기 설정
    /// </summary>
    /// <param name="myturn"></param>
    public void SetTurn(bool myturn) 
    {
        turnObject.SetActive(myturn);
    }
}
