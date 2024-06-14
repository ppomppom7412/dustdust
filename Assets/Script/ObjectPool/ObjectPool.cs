using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YellowGreen.ObjectPool
{
    //����ǰ ���� �� ������
    [System.Serializable]
    public class ObjectPool<T> where T : MonoBehaviour
    {
        bool isInit = false;
        public int limitCount = -1;

        public Transform parent;
        public OBPObject<T> prefab;
        public List<OBPObject<T>> pool;
        public List<int> active; //Ȱ��ȭ����Ʈ(�켱������ ����)

        /// <summary>
        /// �ʱ�ȭ
        /// </summary>
        public void Initialize()
        {
            if (pool == null)
                pool = new List<OBPObject<T>>();

            if (active == null)
                active = new List<int>();

            if (prefab != null && parent != null)
                isInit = true;
        }

        /// <summary>
        /// ������Ʈ Ǯ���� ��Ȱ��ȭ�� ������Ʈ �θ���
        /// </summary>
        /// <returns></returns>
        public OBPObject<T> GetPoolObject()
        {
            if (!isInit)
                Initialize();

            //���� ���� ����) ��� Ȱ��ȭ ���� ��
            if ((limitCount < 1 && pool.Count <= active.Count)
                //���� �ִ� ����) ���Ѱ������� ���� ��� Ȱ��ȭ ���� ��
                || (limitCount > 0 && limitCount >= pool.Count && pool.Count <= active.Count))
            {
                //���Ӱ� ���� �� ��ȯ
                OBPObject<T> returnObject = OBPObject<T>.InstantiateObject(prefab, parent);
                returnObject.SetTargetPool(this);
                pool.Add(returnObject);
                active.Add(pool.Count - 1);

                returnObject.IsActive = true;
                return returnObject;
            }
            //���� �ִ� ��Ȱ��) ���Ѱ�����ŭ ����Ǿ��� ��� Ȱ��ȭ ���� ��
            else if (limitCount > 0 && limitCount <= pool.Count && pool.Count <= active.Count)
            {
                //���� ������ ���� ��Ȱ��ȭ ��Ų��.
                int old = active[0];
                pool[old].IsActive = false;

                //�ٽ� �߰��ؼ� ��ȯ
                active.Add(old);
                pool[old].IsActive = true;
                return pool[old];
            }
            //Ǯ�� ��Ȱ��ȭ�� ���� ���� ��
            else
            {
                for (int i = 0; i < pool.Count; ++i)
                {
                    //null�� �߰��Ǿ ���� �κ��� �ִٸ�
                    if (pool[i] == null)
                    {
                        //�׳� �����ؼ� �߰��س���!
                        pool[i] = OBPObject<T>.InstantiateObject(prefab, parent);
                    }

                    if (!pool[i].IsActive)
                    {
                        active.Add(i);

                        pool[i].IsActive = true;
                        return pool[i];
                    }
                }
            }

            //��� ���ǿ��� Ż���ϸ� ������ ����
            //����� �ڵ� �߰�

            //���Ӱ� ���� �� ��ȯ
            OBPObject<T> newObject = OBPObject<T>.InstantiateObject(prefab, parent);
            newObject.SetTargetPool(this);
            pool.Add(newObject);
            active.Add(pool.Count - 1);

            newObject.IsActive = true;
            return newObject;
        }

        /// <summary>
        /// ������Ʈ�� ��Ȱ��ȭ�� �θ��� �Լ� : Ȱ��ȭ ����Ʈ���� �����
        /// </summary>
        /// <param name="target"></param>
        public void DisableObject(OBPObject<T> target)
        {
            //������ ���� ã�� �����ֱ�
            for (int i = 0; i < pool.Count; ++i)
            {
                if (active == null) continue;

                if (pool[i].Equals(target))
                {
                    active.Remove(i);
                    return;
                }
            }
        }

        /// <summary>
        /// ��� ��Ȱ��ȭ ��Ű��
        /// </summary>
        public void DisableAll()
        {
            for (int i = active.Count; i > 0; --i)
            {
                if (pool.Count <= active[i]) continue;

                pool[active[i]].IsActive = false;
            }
        }

    }
}