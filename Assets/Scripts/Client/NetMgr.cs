using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网络管理相关类  管理客户端连接的所有类
/// 这里可以实现接入不同平台的连接
/// </summary>
public class NetMgr
{
    ///这里可以实现一个连接的集合 建立不同服务器的连接，比方或战斗服务器，聊天服务器等
    ///这里简单实现了一个连接

    /// <summary>
    /// 连接server服务器的连接对象
    /// </summary>
    public static Connection serverConn = new Connection();

    //接入不同平台的连接
    //public static Connection platformConn=new Connection();

    public static void Update()
    {
        serverConn.Update();
        //platformConn.Update();
    }

    public static ProtocolBase GetHeartBeatProtocol()
    {
        //组装心跳协议
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("HeartBeat");
        return protocol;
    }
}
