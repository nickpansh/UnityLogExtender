# 一种Unity日志拓展的方案

[toc]

## 背景

Unity原生的Debug.Log不是那么好用，常见的问题有：

- 未写入本地，尤其是未写入手机端本地，调试比较麻烦
- release模式下无法上报错误信息和关键信息
- 生产/发布环境切换Log输出级别比较麻烦

网上针对性的解决方案也有很多，但或多或少存在一些问题不便于使用。

## 我的方案

我的方案实现的功能有：

- dev模式下log信息写入本地文件（包括移动端）。
- release模式下不输出日志到设备（包括logcat&xcode或其他）
  这节省了字符串拼接带来的垃圾回收与函数调用的开销。
- 可配置埋点系统
  - release模式下的Error和Exception上报给服务器。
  - 允许通过配置上报关键log给服务器。

主要的亮点是：

- 不对Debug.Log进行封装
  - 这样调用者没有学习成本。
  - 导入第三方框架依然可以保证Log一贯性。
- 不引入新的宏定义
  - 对日志的控制通过配置文件定义，易于理解和维护。
- 便捷的配置生产/发布环境的功能。

### Log控制图解
![](https://wenqu.space/uploads/2023/01/29/20230129175649.png)

### 如何使用

- 正常打印维持和Unity的Debug一致，如：

  ```c#
  Debug.LogFormat("这是一条log,参数是{0}", 0);
  Debug.LogError("这是一条报错");
  Debug.LogWarning("这是一条warning");
  Debug.LogAssertion("这是一条断言");
  ```

- 打埋点（上传关键点到服务端）

  ```c#
  LogExtender.Instance.Report("StartUp");
  ```

- 报错上传服务端，无需写代码， 需要配置配置文件。

  ![](https://wenqu.space/uploads/2023/01/27/20230127203555.png)

  ![](https://wenqu.space/uploads/2023/01/27/image-20230127203643542.png)


### 实现原理&设计思路

主要就是通过监听Application.logMessageReceivedThreaded。

在监听回调里，分级别进行日志写入，埋点上传。

设计思路可见[问渠](https://wenqu.site/%E4%B8%80%E7%A7%8DUnity%E6%97%A5%E5%BF%97%E6%89%A9%E5%B1%95%E5%AE%9E%E7%8E%B0%E6%80%9D%E8%B7%AF%E5%8F%8A%E6%96%B9%E6%A1%88.html)

## 参考

[How to use Debug Log in Unity (without affecting performance) - Game Dev Beginner](https://gamedevbeginner.com/how-to-use-debug-log-in-unity-without-affecting-performance/)


## TODO

写入本地文件目前是同步的。


