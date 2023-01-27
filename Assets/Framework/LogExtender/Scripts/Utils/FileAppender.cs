/*** 
 * @Author: NickPansh
 * @Date: 2023-01-27 09:13:49
 * @LastEditors: NickPansh
 * @LastEditTime: 2023-01-27 14:57:20
 * @FilePath: \LogExtender\Assets\Framework\LogExtender\Scripts\Utils\FileAppender.cs
 * @Description: 文件输出模块
 * @
 * @Copyright (c) 2023 by NickPansh nickpansh@yeah.net|wenqu.site, All Rights Reserved. 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace WenQu
{
    public class FileAppender
    {

        #region  private variables
        private FileStream _fileStream;
        private StreamWriter _streamWritter;
        #endregion

        #region  properties
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath { get; private set; }

        /// <summary>
        /// 编码方式
        /// </summary>
        public Encoding encoding { get; private set; }
        #endregion

        #region  Method
        public FileAppender(string filePath, Encoding encoding)
        {
            this.filePath = filePath;
            this.encoding = encoding;
            // 加载FileStream和StreamWritter
            LoadStream();
        }

        // 加载fileStream和streanWritter
        private void LoadStream()
        {
            try
            {
                _fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                _streamWritter = new StreamWriter(_fileStream, encoding);
                _streamWritter.AutoFlush = false;

            }
            catch (Exception e)
            {
                _streamWritter = null;
                throw e;
            }
        }


        /// <summary>
        /// 同步添加日志
        /// </summary>
        /// <param name="log">字符串，日志内容</param>
        /// <returns></returns>
        public void AppendLog(string log)
        {
            _streamWritter.WriteLine(log);
            _streamWritter.Flush();
        }

        /// <summary>
        /// 同步添加日志
        /// </summary>
        /// <param name="log">字符串，日志内容</param>
        /// <returns></returns>
        public void AsyncAppendLog(string log)
        {
            //TODO:异步log暂未实现

        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dipose()
        {
            _streamWritter?.Close();
            _streamWritter?.Dispose();
            _fileStream?.Close();
            _fileStream?.Dispose();
        }
        #endregion
    }
}