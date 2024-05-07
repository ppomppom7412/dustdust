using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState 
{
    const int maxHp = 9;
    const int maxMn = 4;
    int curHp;//ü��
    int curSd;//��¥ ü��
    int curMn;//����

    public void Init() 
    {
        curHp = maxHp;
        curMn = maxMn;
        curSd = 0; 
    }

    public void GetDamage() 
    {
        if (curSd > 0)
            curSd -= 1;
        else
            curHp -= 1;

        if (curHp <= 0)
        {
            curHp = 0;
            //������ ���� �Ŵ������� ������
        }
    }

    public void GetShield()
    {
        if (curSd < maxHp)
            curSd += 1;
    }

    public void GetMana()
    {
        if (curMn < maxMn)
            curMn += 1;
    }

    public void GetHeath()
    {
        if (curHp < maxHp)
            curHp += 1;
    }

    public bool UseMana(int count) 
    {
        if (curMn >= count)
        {
            curMn -= count;
            return true;
        }
        else 
        {
            return false;
        }
    }


}
