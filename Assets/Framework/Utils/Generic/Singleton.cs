/*** 
 * @Author: NickPansh
 * @Date: 2023-01-27 17:35:10
 * @LastEditors: NickPansh
 * @LastEditTime: 2023-01-27 18:06:18
 * @FilePath: \LogExtender\Assets\Framework\Utils\Generic\Singleton.cs
 * @Description: 单例模板
 * @
 * @Copyright (c) 2023 by NickPansh nickpansh@yeah.net|wenqu.site, All Rights Reserved. 
 */
using UnityEngine;
namespace WenQu
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region 局部变量
        private static T _Instance;
        #endregion
        #region 属性
        /// <summary>
        /// 获取单例对象
        /// </summary>
        public static T Instance
        {
            get
            {
                if (null == _Instance)
                {
                    _Instance = FindObjectOfType<T>();
                    if (null == _Instance)
                    {
                        GameObject go = new GameObject();
                        go.name = typeof(T).Name;
                        _Instance = go.AddComponent<T>();
                    }
                }
                return _Instance;
            }
        }
        #endregion
        #region 方法
        protected virtual void Awake()
        {
            if (null == _Instance)
            {
                _Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}