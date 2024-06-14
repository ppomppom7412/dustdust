using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserNameUI : MonoBehaviour
{

    [Header("UIs")]
    public GameObject turnObject;   //���� �� ǥ��
    public Text nameText;
    public Image[] heathImages;
    public Image[] manaImages;

    [Header("Colors")]
    public Color noneColor;
    public Color manaColor;
    public Color heathColor;
    public Color shieldColor;

    /// <summary>
    /// �ʱ� ���·� ����
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
    }

    /// <summary>
    /// ���¿� ���� ǥ�� ��ȭ
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
    /// ��������ī���� �ʱ� ����
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
    /// ��������ī���� �� ǥ�� ����
    /// </summary>
    /// <param name="myturn"></param>
    public void SetTurn(bool myturn) 
    {
        turnObject.SetActive(myturn);
    }

}
