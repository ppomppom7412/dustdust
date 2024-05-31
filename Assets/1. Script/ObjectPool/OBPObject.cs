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
    /// ������Ʈ ����
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

    //��Ȱ��ȭ�� �� Ǯ�� ���ư���
    private void OnDisable()
    {
        if (pool != null)
        {
            isActive = false;
            pool.DisableObject(this);
        }
    }

}
