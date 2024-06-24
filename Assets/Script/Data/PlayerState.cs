using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState 
{
    const int maxHp = 9;
    const int maxMn = 4;
    public int curHp;//체력
    public int curSd;//임시 체력
    public int curMn;//마나

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
            //죽음을 게임 매니저에게 보내기
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
