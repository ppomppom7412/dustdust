using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ΩÃ±€≈Ê ≈€«√∏¥
//https://m.blog.naver.com/PostView.nhn?isHttpsRedirect=true&blogId=cra2yboy&logNo=222184341227&categoryNo=98&proxyReferer=
namespace Singleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.Log(typeof(T).ToString() + " is NULL");
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this as T;
                Init();
            }
        }

        public virtual void Init()
        {
            //√ ±‚»≠
            instance = (T)GameObject.FindObjectOfType(typeof(T));
            if (!instance)
            {
                GameObject instanceObject = new GameObject();
                instanceObject.name = "MonoSingleton";
                instance = (T)instanceObject.AddComponent(typeof(T));
            }
        }
    }
}