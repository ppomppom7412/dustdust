using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YellowGreen.ObjectPool
{
    //복제품 생산 및 관리자
    [System.Serializable]
    public class ObjectPool<T> where T : MonoBehaviour
    {
        bool isInit = false;
        public int limitCount = -1;

        public Transform parent;
        public OBPObject<T> prefab;
        public List<OBPObject<T>> pool;
        public List<int> active; //활성화리스트(우선순위에 따른)

        /// <summary>
        /// 초기화
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
        /// 오브젝트 풀에서 비활성화된 오브젝트 부르기
        /// </summary>
        /// <returns></returns>
        public OBPObject<T> GetPoolObject()
        {
            if (!isInit)
                Initialize();

            //제한 없이 생성) 모두 활성화 됐을 때
            if ((limitCount < 1 && pool.Count <= active.Count)
                //제한 있는 생성) 제한개수보다 적고 모두 활성화 됐을 때
                || (limitCount > 0 && limitCount >= pool.Count && pool.Count <= active.Count))
            {
                //새롭게 생성 후 반환
                OBPObject<T> returnObject = OBPObject<T>.InstantiateObject(prefab, parent);
                returnObject.SetTargetPool(this);
                pool.Add(returnObject);
                active.Add(pool.Count - 1);

                returnObject.IsActive = true;
                return returnObject;
            }
            //제한 있는 재활용) 제한개수만큼 생산되었고 모두 활성화 됐을 때
            else if (limitCount > 0 && limitCount <= pool.Count && pool.Count <= active.Count)
            {
                //가장 오래된 것을 비활성화 시킨다.
                int old = active[0];
                pool[old].IsActive = false;

                //다시 추가해서 반환
                active.Add(old);
                pool[old].IsActive = true;
                return pool[old];
            }
            //풀에 비활성화된 것이 있을 때
            else
            {
                for (int i = 0; i < pool.Count; ++i)
                {
                    //null이 추가되어서 꼬인 부분이 있다면
                    if (pool[i] == null)
                    {
                        //그냥 생성해서 추가해놓자!
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

            //모든 조건에서 탈출하면 문제가 있음
            //제어용 코드 추가

            //새롭게 생성 후 반환
            OBPObject<T> newObject = OBPObject<T>.InstantiateObject(prefab, parent);
            newObject.SetTargetPool(this);
            pool.Add(newObject);
            active.Add(pool.Count - 1);

            newObject.IsActive = true;
            return newObject;
        }

        /// <summary>
        /// 오브젝트가 비활성화시 부르는 함수 : 활성화 리스트에서 지우기
        /// </summary>
        /// <param name="target"></param>
        public void DisableObject(OBPObject<T> target)
        {
            //동일한 것을 찾아 지워주기
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
        /// 모두 비활성화 시키기
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