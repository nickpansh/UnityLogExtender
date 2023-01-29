/*** 
 * @Author: NickPansh
 * @Date: 2023-01-27 07:02:49
 * @LastEditors: NickPansh
 * @LastEditTime: 2023-01-29 17:35:32
 * @FilePath: \LogExtender\Assets\Framework\LogExtender\Scripts\Test\TestLogger.cs
 * @Description: 
 * @
 * @Copyright (c) 2023 by NickPansh nickpansh@yeah.net|wenqu.site, All Rights Reserved. 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WenQu;

public class TestLogger : MonoBehaviour
{
    void Start()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("uuid", "123131313");
        LogExtender.Instance.LogTrack(Convert.ToString((int)EventEnum.StartUp), dict);
        Debug.LogFormat("这是一条log,参数是{0}", 0);
        Debug.LogError("这是一条报错");
        Debug.LogWarning("这是一条warning");
        Debug.LogAssertion("这是一条断言");
        TestFunc();
    }

    private void TestFunc()
    {
        Debug.LogFormat("这是第二条log,参数是{0}", "abcd");
    }
    void Update()
    {

    }
}
