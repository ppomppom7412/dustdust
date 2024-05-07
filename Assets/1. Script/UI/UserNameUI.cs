using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserNameUI : MonoBehaviour
{
    public Text nameText;
    public GameObject turnObject;
    public GameObject meImage; //���� ��� ������ ǥ��

    /// <summary>
    /// ��������ī���� �ʱ� ����
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
    /// ��������ī���� �� ǥ�� ����
    /// </summary>
    /// <param name="myturn"></param>
    public void SetTurn(bool myturn) 
    {
        turnObject.SetActive(myturn);
    }
}
