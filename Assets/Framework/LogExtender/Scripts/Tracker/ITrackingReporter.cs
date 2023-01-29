/*** 
 * @Author: NickPansh
 * @Date: 2023-01-27 17:18:50
 * @LastEditors: NickPansh
 * @LastEditTime: 2023-01-29 17:19:06
 * @FilePath: \LogExtender\Assets\Framework\LogExtender\Scripts\Tracker\ITrackingReporter.cs
 * @Description: 埋点/报错上报接口
 * @这里定义接口的原因是为了后期框架如果嵌入了HTTP模块，可以方便替换实现
 * @Copyright (c) 2023 by NickPansh nickpansh@yeah.net, All Rights Reserved. 
 */
using System.Collections.Generic;

namespace WenQu
{
    public interface ITrackingReporter
    {

        /// <summary>
        /// 上报埋点信息
        /// </summary>
        /// <param name="url">string,url</param>
        /// <param name="eventId">string,事件ID</param>
        /// <param name="dict">字典，要上报的信息</param>

        /// <param name="method"></param>
        public void ReportTrackingEvent(string url, string eventId, Dictionary<string, string> dict, TrackerConst.HttpMethod method = TrackerConst.HttpMethod.Get);

        /// <summary>
        /// 上报错误信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="url">string,url</param>
        /// <param name="dict">字典，要上报的信息</param>

        /// <param name="method"></param>
        public void ReportError(string url, Dictionary<string, string> dict, TrackerConst.HttpMethod method = TrackerConst.HttpMethod.Get);
    }
}