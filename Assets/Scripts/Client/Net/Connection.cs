using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// 客户端的网络连接类
/// </summary>
public class Connection
{
    //buffer 大小
    private const int BUFFER_SIZE = 1024;
    /// <summary>
    /// 连接socket
    /// </summary>
    private Socket socket;
    /// <summary>
    /// 缓冲buff
    /// </summary>
    public byte[] readBuff=new byte[BUFFER_SIZE];
    /// <summary>
    /// buff的大小
    /// </summary>
    private int buffCount = 0;

    ///粘包分包相关
    /// <summary>
    /// 消息长度
    /// </summary>
    private Int32 msgLength=0;
    /// <summary>
    /// 消息长度bytes
    /// </summary>
    private byte[] lenBytes = new byte[sizeof(Int32)];
    /// <summary>
    /// 协议
    /// </summary>
    public ProtocolBase protocol=new ProtocolBase();
    /// <summary>
    /// 上一次的心跳时间
    /// </summary>
    public float lastTickTime = 0f;
    /// <summary>
    /// 心跳时间
    /// </summary>
    public float heartBeatTime = 30f;

    ///消息分发
    public MsgDistribution msgDistribution = new MsgDistribution();

    /// <summary>
    /// 连接状态
    /// </summary>
    public ConnStatus status = ConnStatus.None;

    public bool Connect(string host,int port)
    {
        try
        {
            //socket初始化
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //连接
            socket.Connect(host, port);
            //开始接收
            socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE - buffCount, SocketFlags.None, ReceiveCb, readBuff);

            Debug.Log("连接成功");

            //更新状态
            status = ConnStatus.Connected;
            return true;
        }
        catch ( Exception e)
        {
            Debug.LogError($"连接失败：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    /// <returns></returns>
    public bool Close()
    {
        try
        {
            socket.Close();
            status = ConnStatus.None;
            return true;
        }
        catch ( Exception e )
        {
            Debug.LogError($"关闭失败{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 异步接收方法
    /// </summary>
    /// <param name="ar"></param>
    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            buffCount += count;
            ProcessData();
            socket.BeginReceive(readBuff, buffCount, BUFFER_SIZE - buffCount, SocketFlags.None,ReceiveCb,readBuff);
        }
        catch (Exception e)
        {
            Debug.LogError($"接收消息失败:{e.Message}");
            status = ConnStatus.None;
        }
    }

    /// <summary>
    /// 处理粘包分包操作
    /// </summary>
    private void ProcessData()
    {
        if (buffCount < sizeof(Int32))
        {
            Debug.Log("不满足消息长度");
            return;
        }
        Array.Copy(readBuff, lenBytes, sizeof(Int32));
        int msgLength = BitConverter.ToInt32(lenBytes,0);
        if(buffCount<msgLength+sizeof(Int32))
        {
            Debug.Log("不满足完整消息长度");
            return;
        }
        //解码协议方法
        protocol = new ProtocolStr();
        protocol = protocol.Decode(readBuff, sizeof(Int32), msgLength) as ProtocolStr;
        Debug.Log($"收到消息：{protocol.GetDesc()}");

        //将消息加入到分发的队列/集合
        lock (msgDistribution.msgList)
        {
            msgDistribution.msgList.Add(protocol);
        }

        //清除已处理的消息
        int count = buffCount - sizeof(Int32) - msgLength;
        Array.Copy(readBuff, sizeof(Int32) + msgLength, readBuff, 0, count);
        buffCount = count;
        //如果存在消息 递归调用
        if(buffCount>0)
        {
            ProcessData();
        }
    }

    /// <summary>
    /// 向服务器发送协议
    /// </summary>
    /// <param name="protocol"></param>
    /// <returns></returns>
    public bool Send(ProtocolBase protocol)
    {
        if(status!=ConnStatus.Connected)
        {
            Debug.LogError("请先连接服务器");
            return false;
        }

        ///拼接成一个完整的bytes数组
        byte[] msgBytes = protocol.Encode();
        byte[] lenBytes = BitConverter.GetBytes(msgBytes.Length);
        byte[] sendBuff = lenBytes.Concat(msgBytes).ToArray();
        //发送
        socket.Send(sendBuff);
        Debug.Log($"发送{protocol.GetDesc()}消息");
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="protocol"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool Send(ProtocolBase protocol,Action<ProtocolBase> cb)
    {
        string protoName = protocol.GetName();
        return Send(protocol, protoName, cb);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="protocol"></param>
    /// <param name="cbName"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool Send(ProtocolBase protocol, string cbName, Action<ProtocolBase> cb)
    {
        if (status != ConnStatus.Connected)
            return false;
        msgDistribution.AddOnceListener(cbName, cb);
        return Send(protocol);
    }

    /// <summary>
    /// 更新函数
    /// </summary>
    public void Update()
    {
        //消息分发调用
        msgDistribution.Update();

        if(status==ConnStatus.Connected)
        {
            if(Time.time-lastTickTime>heartBeatTime)
            {
                //发送一个心跳包
                ProtocolBase protocol=new ProtocolBase();//NetMgr.GetHeartBeatProtocol()
                Send(protocol);
                lastTickTime = Time.time;
            }
        }

    }
}

public enum ConnStatus
{
    None,
    Connected
};
