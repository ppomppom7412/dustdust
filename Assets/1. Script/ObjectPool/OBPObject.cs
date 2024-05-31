using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OBPObject<T> : MonoBehaviour where T : MonoBehaviour
{
    public ObjectPool<T> pool;
    bool isActive = false;
    public bool IsActive
    {
        get { return isActive; }
        set {
            if (!value) 
                gameObject.SetActive(false);

            isActive = value;
        }
    }

    /// <summary>
    /// 오브젝트 생성
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    static public OBPObject<T> InstantiateObject(OBPObject<T> prefab, Transform parent) 
    {
        return Instantiate<OBPObject<T>>(prefab, Vector3.zero, Quaternion.identity, parent);
    }

    public void Active(Vector3 position) 
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

    //비활성화될 때 풀로 돌아가기
    private void OnDisable()
    {
        if (pool != null)
        {
            isActive = false;
            pool.DisableObject(this);
        }
    }

}
