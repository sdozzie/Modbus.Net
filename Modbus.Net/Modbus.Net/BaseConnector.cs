﻿using System;
using System.Threading.Tasks;

namespace Modbus.Net
{
    public abstract class BaseConnector
    {
        /// <summary>
        /// 标识Connector的连接关键字
        /// </summary>
        public abstract string ConnectionToken { get; }
        /// <summary>
        /// 是否处于连接状态
        /// </summary>
        public abstract bool IsConnected { get; }
        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <returns>是否连接成功</returns>
        public abstract bool Connect();
        /// <summary>
        /// 连接PLC，异步
        /// </summary>
        /// <returns>是否连接成功</returns>
        public abstract Task<bool> ConnectAsync();
        /// <summary>
        /// 断开PLC
        /// </summary>
        /// <returns>是否断开成功</returns>
        public abstract bool Disconnect();
        /// <summary>
        /// 无返回发送数据
        /// </summary>
        /// <param name="message">需要发送的数据</param>
        /// <returns>是否发送成功</returns>
        public abstract bool SendMsgWithoutReturn(byte[] message);
        /// <summary>
        /// 无返回发送数据
        /// </summary>
        /// <param name="message">需要发送的数据</param>
        /// <returns>是否发送成功</returns>
        public abstract Task<bool> SendMsgWithoutReturnAsync(byte[] message);
        /// <summary>
        /// 带返回发送数据
        /// </summary>
        /// <param name="message">需要发送的数据</param>
        /// <returns>是否发送成功</returns>
        public abstract byte[] SendMsg(byte[] message);
        /// <summary>
        /// 带返回发送数据
        /// </summary>
        /// <param name="message">需要发送的数据</param>
        /// <returns>是否发送成功</returns>
        public abstract Task<byte[]> SendMsgAsync(byte[] message);
    }
}
