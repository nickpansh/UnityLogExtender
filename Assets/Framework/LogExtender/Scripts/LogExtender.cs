/*** 
 * @Author: NickPansh
 * @Date: 2023-01-27 06:50:53
 * @LastEditors: NickPansh
 * @LastEditTime: 2023-01-29 17:22:14
 * @FilePath: \LogExtender\Assets\Framework\LogExtender\Scripts\LogExtender.cs
 * @Description: Unity日志拓展类
 * @
 * @Copyright (c) 2023 by NickPansh nickpansh@yeah.net|wenqu.site, All Rights Reserved. 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
namespace WenQu
{
    public sealed class LogExtender : MonoBehaviour
    {
        #region private variables
        // 文件记录器，用于写自定义文件
        private static FileAppender _fileAppender;
        private static ITrackingReporter _trackingReporter;
        // 格式化输出样式
        private const string _formatInfo = "[{0}]|{1,-5}|{2}|:{3}";
        private const string _formatErr = "[{0}]|{1,-5}|:{2}\r\n{3}";

        // 时间格式化输出样式
        private const string _timeFormat = "yyyy.MM.dd HH:mm:ss,fff";

        // 埋点上报url
        private string _urlTrackingEvent = null;

        // 报错上报url
        private string _urlTrackingError = null;
        #endregion

        #region  properties
        // 自定义日志文件路径
        public static string pathCustomLog { get; private set; }

        public static LogExtender Instance { get; private set; }
        /// <summary>
        /// 配置文件
        /// </summary>
        public LogExtenderConf conf;

        #endregion

        #region Method
        private void Awake()
        {

            // 检查唯一性
            /*
            问：为什么用Assert，而不是像一些MonoBehaviour的单例那样在这里移除原有的组件并重新生成新的组件？ref:https://wenqu.site/2023/01/07/%E6%B7%B1%E5%85%A5Unity%E5%8D%95%E4%BE%8B%E6%A8%A1%E5%BC%8F%EF%BC%88%E9%99%84%E4%BB%A3%E7%A0%81%EF%BC%89/
            答：首先，日志类不需要懒加载。它在游戏启动的时候就应该在场景里。
                其次，与其说是不需要懒加载，不如说是不应该懒加载。设计上这个类就应该拖到出初始场景里。否则会出现部分日志没走LogExtender的逻辑造成歧义。
                既然设计上不需要懒加载，而是直接把本脚本拖到初始场景，那自然不应该有移除操作。增加了移除代码反而容易造成使用上的困惑。
            */
            Assert.IsTrue(GetComponents(typeof(LogExtender)).Length <= 1, "只允许存在1个LogExtender！请检查场景移除多余的LogExtender。");
            if (null == Instance)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                // 控制日志等级
                Debug.unityLogger.filterLogType = Debug.isDebugBuild ? conf.devLogType : conf.releaseLogType;

                // 开发环境且选项开启才初始化fileAppender
                if (conf.logToCustomFileWhenDev && Debug.isDebugBuild)
                {
                    LoadFileAppender();
                }
                LoadTrackingReporter();
                // 接收log线程
                Application.logMessageReceivedThreaded += HandleLog;
            }

        }

        private void OnDestroy()
        {
            Application.logMessageReceivedThreaded -= HandleLog;
            _fileAppender?.Dipose();
        }

        // 加载FileAppender
        private void LoadFileAppender()
        {
#if UNITY_EDITOR
            string fileDir = Application.dataPath;
#else
           string  fileDir = Application.persistentDataPath;
#endif
            pathCustomLog = Path.Join(fileDir, conf.customFileName);
            _fileAppender = new FileAppender(pathCustomLog, conf.encodeFormat);
        }

        // 加载上报器
        private void LoadTrackingReporter()
        {
            _urlTrackingEvent = Debug.isDebugBuild ? conf.urlDevTrackingEvent : conf.urlReleaseTrackingEvent;
            _urlTrackingError = Debug.isDebugBuild ? conf.urlDevTrackingError : conf.urlReleaseTrackingError;
            _trackingReporter = this.gameObject.AddComponent<HttpTrackingReporter>();

        }

        /// <summary>
        /// 格式化log
        /// </summary>
        /// <param name="logString">string of log的内容</param>
        /// <param name="stackTrace">string of 堆栈信息</param>
        /// <param name="type">LogType of 日志的类型</param>
        /// <returns>格式化后的字符串</returns>
        private string FormatLog(string logString, string stackTrace, LogType type, DateTime dateTime)
        {
            if (type == LogType.Log)
                return string.Format(_formatInfo, dateTime.ToString(_timeFormat), type, GetBriefStackTrace(stackTrace), logString);
            else
                return string.Format(_formatErr, dateTime.ToString(_timeFormat), type, logString, stackTrace);
        }

        /// <summary>
        /// 获得堆栈的简短描述(一般是文件名和行号)
        /// </summary>
        /// <param name="stackTrace"></param>
        /// <returns></returns>
        private string GetBriefStackTrace(string stackTrace)
        {
            var sc = Regex.Match(stackTrace, conf.regexPattern);
            return sc?.Value;
        }

        /// <summary>
        /// 响应额外日志操作
        /// </summary>
        /// <param name="logString">string of log的内容</param>
        /// <param name="stackTrace">string of 堆栈信息</param>
        /// <param name="type">LogType of 日志的类型</param>
        private void HandleLog(string logString, string stackTrace, LogType type)
        {

            // 写入自定义文件
            if (_fileAppender != null)
            {
                string log = FormatLog(logString, stackTrace, type, DateTime.Now);
                _fileAppender.AppendLog(log);
            }

            // Error+上传报错服务器
            if (type == LogType.Error || type == LogType.Exception)
            {
                if (!string.IsNullOrEmpty(_urlTrackingError))
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("log", logString);
                    dict.Add("stackTrace", stackTrace);
                    dict.Add("time", DateTime.Now.ToString());
                    _trackingReporter.ReportError(_urlTrackingError, dict);
                }
            }
        }

        public void LogTrack(string eventId, Dictionary<string, string> dict)
        {
            _trackingReporter.ReportTrackingEvent(_urlTrackingEvent, eventId, dict);
        }
        #endregion

    }
}