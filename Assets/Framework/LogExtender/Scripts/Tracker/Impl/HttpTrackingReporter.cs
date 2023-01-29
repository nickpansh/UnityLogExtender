/*** 
 * @Author: NickPansh
 * @Date: 2023-01-27 17:25:45
 * @LastEditors: NickPansh
 * @LastEditTime: 2023-01-27 20:23:03
 * @FilePath: \LogExtender\Assets\Framework\LogExtender\Scripts\Tracker\Impl\HttpTrackingReporter.cs
 * @Description: http埋点上传工具
 * @
 * @Copyright (c) 2023 by NickPansh nickpansh@yeah.net|wenqu.site, All Rights Reserved. 
 */
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace WenQu
{
    //TODO:这个应该用Helper，不用做单例
    public class HttpTrackingReporter : Singleton<HttpTrackingReporter>, ITrackingReporter
    {
        private void Report(Dictionary<string, string> dict, string url, TrackerConst.HttpMethod method = TrackerConst.HttpMethod.Get)
        {
            if (method == TrackerConst.HttpMethod.Get)
            {
                if (dict != null)
                {
                    url = url + "?";
                    foreach (var item in dict)
                    {
                        url = string.Format("%s&%s=%s", url, item.Key, item.Value);
                    }
                    StartCoroutine(Get(url));
                }
            }
            else
            {
                StartCoroutine(Post(url, dict));
            }
        }

        public void ReportError(Dictionary<string, string> dict, string url, TrackerConst.HttpMethod method = TrackerConst.HttpMethod.Get)
        {
            Report(dict, url, method);
        }

        public void ReportTrackingEvent(string eventId, Dictionary<string, string> dict, string url, TrackerConst.HttpMethod method = TrackerConst.HttpMethod.Get)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                Debug.LogError("eventId字段缺失");
                return;
            }
            if (dict != null)
            {
                dict.Add("eventId", eventId);
            }
            Report(dict, url, method);
        }

        IEnumerator Post(string url, Dictionary<string, string> dict)
        {
            WWWForm form = new WWWForm();
            if (dict != null)
            {
                foreach (var item in dict)
                {
                    form.AddField(item.Key, item.Value);
                }
            }
            UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.ProtocolError)
                Debug.Log(webRequest.error);
            {
                yield return webRequest.downloadHandler.text;
            }
        }
        private IEnumerator Get(string body)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(body);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.ProtocolError)
            {
                yield return webRequest.downloadHandler.text;
            }
        }

    }
}