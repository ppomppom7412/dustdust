using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

public class EffectController : MonoSingleton<EffectController>
{
    public List<ObjectPool<EffectObject>> fxPool;

    [NaughtyAttributes.Button]
    public void TestEfx()
    {
        ShowEfx(0, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0));
    }

    public void ShowEfx(int id, Vector3 pos) 
    {
        if ((fxPool ?? new List<ObjectPool<EffectObject>>()).Count <= id)
            return;

        OBPObject<EffectObject> getefx = fxPool[id].GetPoolObject();
        getefx.Active(pos);
    }

}
