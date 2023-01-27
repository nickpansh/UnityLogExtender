using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WenQu;

public class TestLogger : MonoBehaviour
{
    void Start()
    {
        LogExtender.Instance.Report(Convert.ToString((int)EventEnum.StartUp));
        Debug.LogFormat("这是一条log,参数是{0}", 0);
        Debug.LogError("这是一条报错");
        Debug.LogWarning("这是一条warning");
        Debug.LogAssertion("这是一条断言");
    }

    void Update()
    {

    }
}
