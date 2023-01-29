/*** 
 * @Author: NickPansh
 * @Date: 2023-01-27 17:25:45
 * @LastEditors: NickPansh
 * @LastEditTime: 2023-01-29 17:40:40
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
using System.Text;

namespace WenQu
{
    public class HttpTrackingReporter : MonoBehaviour, ITrackingReporter
    {

        private void Report(string url, Dictionary<string, string> dict, TrackerConst.HttpMethod method = TrackerConst.HttpMethod.Get)
        {
            if (method == TrackerConst.HttpMethod.Get)
            {

                if (dict != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb = sb.Append(url);
                    sb = sb.Append("?");
                    foreach (var item in dict)
                    {
                        sb = sb.Append(string.Format("{0}={1}", item.Key, item.Value));
                    }
                    StartCoroutine(Get(sb.ToString()));
                }
            }
            else
            {
                StartCoroutine(Post(url, dict));
            }
        }


        public void ReportTrackingEvent(string url, string eventId, Dictionary<string, string> dict, TrackerConst.HttpMethod method = TrackerConst.HttpMethod.Get)
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
            Debug.LogFormat("[TAG]{0}", eventId);
            Report(url, dict, method);
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

            {
                Debug.LogFormat("{0} posted.", url);
                yield return webRequest.downloadHandler.text;

            }
            else
            {
                throw new System.NotImplementedException();
            }

        }
        private IEnumerator Get(string body)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(body);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogFormat("{0} posted.", body);
                yield return webRequest.downloadHandler.text;
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }
        public void ReportError(string url, Dictionary<string, string> dict, TrackerConst.HttpMethod method = TrackerConst.HttpMethod.Get)
        {
            Report(url, dict, method);
        }


    }
}