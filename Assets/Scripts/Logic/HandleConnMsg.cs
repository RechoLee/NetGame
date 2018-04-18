using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 连接对象的协议处理类、
/// 使用的是分部类 有助于逻辑功能的拆分
/// </summary>
public partial class HandleConnMsg
{
    /// <summary>
    /// 处理心跳协议
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="protocol"></param>
    public void MsgHeartBeat(Conn conn,ProtocolBase protocol)
    {
        conn.lastTickTime = Sys.GetTimeStamp();
        Debug.Log($"更新conn--{conn.socket.RemoteEndPoint.ToString()}--的上一次的心跳时间");
    }
}
