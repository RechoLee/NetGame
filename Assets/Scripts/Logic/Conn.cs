using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary>
/// 连接对象
/// </summary>
public class Conn
{
    //常量
    public const int BUFFER_SIZE = 1024;
    //socket
    public Socket socket;
    //是否使用
    public bool isUse = false;
    //Buff
    public byte[] readBuff = new byte[BUFFER_SIZE];
    //buff大小
    public int buffCount = 0;

    //粘包分包
    public byte[] lenBytes = new byte[sizeof(UInt32)];
    public Int32 msgLength = 0;

    //心跳时间
    public long lastTickTime = long.MinValue;

    //player对象
    public Player player;

    //无参数构造函数用来初始化工作
    public Conn()
    {
        readBuff = new byte[BUFFER_SIZE];
    }

    /// <summary>
    /// 初始化函数，接收传入的socket初始socket连接等需要的数据
    /// </summary>
    public void Init(Socket _socket)
    {
        this.socket = _socket;
        isUse = true;
        buffCount = 0;

        //心跳时间
        lastTickTime = Sys.GetTimeStamp();
    }

    /// <summary>
    /// 剩余的大小
    /// </summary>
    /// <returns></returns>
    public int BuffRemain()
    {
        return BUFFER_SIZE - buffCount;
    }
    /// <summary>
    /// 获取地址
    /// </summary>
    /// <returns>返回地址字符串</returns>
    public string GetAddress()
    {
        if (!isUse)
            return "无法获取地址";
        return socket.RemoteEndPoint.ToString();
    }
    
    /// <summary>
    /// 关闭连接对象，重置一些参数
    /// </summary>
    public void Close()
    {
        if (!isUse)
            return;
        if(player!=null)
        {
            //玩家退出操作
            //player.Logout();
            return;
        }
        Debug.Log($"{GetAddress()}---断开连接---");
        //断开socket
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        isUse = false;
    }

    /// <summary>
    /// 发送协议，实际是对ServNet中Send的封装
    /// </summary>
    /// <param name="protocol"></param>
    public void Send(ProtocolBase protocol)
    {
        ServNet.instance.Send(this, protocol);
    }

    public void Test()
    {
        
    }
}
