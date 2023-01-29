/*** 
 * @Author: NickPansh
 * @Date: 2023-01-27 08:21:16
 * @LastEditors: NickPansh
 * @LastEditTime: 2023-01-29 16:50:55
 * @FilePath: \LogExtender\Assets\Framework\LogExtender\Scripts\Utils\LogExtenderConf.cs
 * @Description: Unity日志扩展配置
 * @
 * @Copyright (c) 2023 by NickPansh nickpansh@yeah.net|wenqu.site, All Rights Reserved. 
 */
using System.Text;
using UnityEngine;
namespace WenQu
{
    [CreateAssetMenu(fileName = "LogExtenderConf", menuName = "LogExtender/LogExtenderConf", order = 0)]
    public class LogExtenderConf : ScriptableObject
    {
        /// <summary>
        /// 是否写入自定义的log文件(只在dev环境中生效)
        /// </summary>
        /// <remarks>
        /// Unity默认的game.log不会输出到手机上
        /// 开启这个开关，可以在手机上也拿到game.log
        /// </remarks>
        public bool logToCustomFileWhenDev = true;

        /// <summary>
        /// 开发环境埋点url，不填则关闭
        /// </summary>
        public string urlDevTrackingEvent = "";


        /// <summary>
        /// 正式环境埋点url，不填则关闭
        /// </summary>
        public string urlReleaseTrackingEvent = "";


        /// <summary>
        /// 测试环境报错上传url（非Editor生效），不填则关闭
        /// </summary>
        public string urlDevTrackingError = "";


        /// <summary>
        /// 正式环境报错上传url，不填则关闭
        /// </summary>
        public string urlReleaseTrackingError = "";

        /// <summary>
        /// 测试环境的log等级
        /// </summary>
        public LogType devLogType = LogType.Log;

        /// <summary>
        /// 正式环境的log等级
        /// </summary>
        // [HideInInspector]
        public LogType releaseLogType = LogType.Error;

        /// <summary>
        /// 输出log的正则表达式
        /// </summary>
        [HideInInspector]
        public string regexPattern = @"[a-zA-Z0-9]*.cs:\d*";
        /// <summary>
        /// 自定义文件名文件名
        /// </summary>
        [HideInInspector]
        public string customFileName = "game.log";

        /// <summary>
        /// 编码方式
        /// </summary>
        [HideInInspector]
        public Encoding encodeFormat = Encoding.UTF8;



    }
}