# 一种Unity日志拓展的方案

[toc]

## 背景

Unity原生的Debug.Log不是那么好用，常见的问题有：

- 未写入本地，尤其是未写入手机端本地，调试比较麻烦
- release模式下无法上报错误信息和关键信息
- 生产/发布环境切换Log输出级别比较麻烦

网上针对性的解决方案也有很多，但或多或少存在一些问题不便于使用。

## 现有解决方案的问题

常见的问题有：

- 字符串的GC没有考虑

  ```c#
  Log.Instance.Log("A="+a);
  ```

  比如上面这段代码，"A="+a的拼接是在传参之前的，这部分开销不合理。

  隐式装箱也很可怕。

- 没有做好宏定义控制，带来if判断的开销。

  部分框架在引入自己的封装Logger时并没有用宏定义来做条件判断，只是简单地用了if判断。

  ```c#
  /// <summary>
  /// info Log
  /// </summary>
  /// <param name="log">Log.</param>
  public void info(object log)
  {
      // 这里有问题，这个if判断的开销依旧可以优化掉。
      if (LogLevel > INFO)
      {
          return;
      }
     	Debug.Log(log);
  }
  ```

  如上面的例子所示，用if并不是个好选择。

- 用宏定义做了过滤级别的判断，但依然不够好。

  ```c#
  public static class GameLog
  {
      public static void LogMessage(string message)
      {
          #if UNITY_EDITOR
          Debug.Log(message);
          #endif
      }
      public static void LogWarning(string message)
      {
          #if UNITY_EDITOR
          Debug.LogWarning(message);
          #endif
      }
      public static void LogError(string message)
      {
          #if UNITY_EDITOR
          Debug.LogWarning(message);
          #endif
      }
  }
  ```

  ![](https://wenqu.space/uploads/2023/01/27/20230127162404.png)

  虽然没有Debug.Log的开销，但是封装的GameLog.Log的开销依旧存在。

- 还有的框架在此基础上做了改进，使用了Optional关键字。如：

  ```c#
  [System.Diagnostics.Conditional("UNITY_EDITOR")]
  public static void LogMessage(string message)
  {
      Debug.Log(message);
  }
  ```

  这个方式性能没有问题，但一来这么做的框架分级别控制都分的非常细（比如常见分七级：log,debug,assert,warning,error,exception,fatal）。

  每个级别都引入一个宏定义。我非常不喜欢这个做法。

  宏定义多了以后，项目维护起来非常的麻烦。

  由于用了封装，还带来一个问题就是老项目如果没有做权限控制，迁移起来比较麻烦。（除非全局替换）

  另外我也不喜欢封装的做法，我的原则是**除非框架对性能有巨大的提升或者明显可以提高开发效率，否则不要去对现有接口进行简单的封装。**

  这一点后面会介绍。简而言之，这个方案还OK，但不符合我的需求。

关于各种方案的性能分析，具体可以参考[How to use Debug Log in Unity (without affecting performance) - Game Dev Beginner](https://gamedevbeginner.com/how-to-use-debug-log-in-unity-without-affecting-performance/)



## 我的方案

我的方案实现的功能有：

- log信息写入本地文件（包括移动端）。
- release模式下裁切掉所有日志。
- 实现埋点系统，release模式下的Error和Exception上报给服务器。
- 埋点系统允许打关键log上报给服务器。
- 便捷的配置生产/发布环境的功能。

主要的亮点是：

- 不对Debug.Log进行封装，这样调用者没有学习成本。
- 沿用Unity的Log,Warning,Error三级，把其他级别的打印交给埋点。
- 什么环境下需要什么级别的日志控制用配置文件控制，不依赖茫茫多的宏定义。

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



- 不同环境下Log的控制

![](https://wenqu.space/uploads/2023/01/27/20230127170723.png)

### 实现原理

主要就是通过监听Application.logMessageReceivedThreaded。

在监听回调里，分级别进行日志写入，埋点上传。



## 设计思路



### 用Extender而不用封装

我有一个原则，就是除非你的封装对性能有巨大的提升，或者明显可以提高开发效率，否则不要对现有接口进行简单的封装。

封装势必带来新的接口，新的接口就带来新的学习成本。

我很不愿意看到项目组新人在调用Debug.Log的时候还要问一下“我们项目组是怎么封装Debug.Log”的。

尤其是大家的文档再怎么写，规范再怎么规定，都不可能有Unity的文档写得好，写的规范。

我希望框架的使用者尽可能接触到的是原汁原味的Unity，

不是XXFramework.Log，不是XXLog.Instance.Log，而是官方文档里写的标准的Debug.Log。



LogExtender的实现原理是监听Application.logMessageReceivedThreaded事件，在接收到事件后去做写入文件等操作。

因此裁切代码，调用方式都和Unity官方保持一致。

LogExtender只是对原有功能的一个补充，你完全不了解也没事——项目组负责打包的那个人了解一下就行了。



这个原则对我来说很重要，所以才大费周章重新造轮子。



### 不引入一堆宏和无用的日志等级

引入宏看起来挺好，但不方便维护。

它非常依赖于文档，远远不如配置文件使用。

这里用了ScriptableObject做LogExtender的配置，比起宏定义，好维护很多。



### 做真正有效的事

Log分八级是很常规的，尤其在服务器开发上。

优先级从高到低依次为：OFF、FATAL、ERROR、WARN、INFO、DEBUG、TRACE、 ALL。

但不能无脑照搬。手游因为有给包行为的存在会不太一样。

最简单的例子，在服务端开发中，debug和info有着非常鲜明的差别——

debug用于调试，只存在于测试环境中。

info用于打点，测试环境中应剔除。



但对于客户端来说，debug和info做区分意义不大。

不管是debug还是info，在交付到玩家手里的包中，都应该剔除！

原因是给玩家的手机中写一个日志文件，不管是什么消息，对玩家有什么用呢？



我的思路是Log保留Unity的三个级别，

额外再用埋点+报错收集来做手游日志的分级别控制，而不是引入无效的tag。

我们真正的关心的事情是：

- 如何搜集到已经交付给玩家的包体里的报错
- 如何搜集到玩家的关键操作日志

这些即使输出到手机上也没什么用，都应该交给埋点来做。



### 关于看帧号，以及分标签打印，颜色输出

这几个事情我都没有做，因为我觉得没有必要。

如果真的有需求，推荐插件Editor Pro。

能用插件解决的问题，就不要去用生产环境的逻辑代码来解决。

风险高+无效的性能开销。



## 参考

[How to use Debug Log in Unity (without affecting performance) - Game Dev Beginner](https://gamedevbeginner.com/how-to-use-debug-log-in-unity-without-affecting-performance/)



## TODO

目前LogExtender.cs的代码已经完成。

写入本地文件目前是同步的，还需要修改。

埋点上传也不完善，目前只是演示。
