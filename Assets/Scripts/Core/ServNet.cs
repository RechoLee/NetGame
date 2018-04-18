using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary>
/// 网络处理类
/// </summary>
public class ServNet
{
    //监听套接字
    public Socket listenfd;
    //客户端连接池
    public Conn[] conns;
    //最大连接数
    public int maxConn = 50;
    //单例
    public static ServNet instance;

    //构造函数
    public ServNet()
    {
        instance = this;
    }

    /// <summary>
    /// 获取可用连接对象的索引
    /// </summary>
    /// <returns>返回连接对象在池中的索引，返回-1代表获取失败</returns>
    public int NewIndex()
    {
        int index=-1;
        if (conns == null)
            return -2;
        for (int i = 0; i < conns.Length; i++)
        {
            if (!conns[i].isUse)
            {
                index = i;
                if (conns[i] == null)
                    conns[i] = new Conn();
                break;
            }
        }

        return index;
    }

    /// <summary>
    /// 启动服务
    /// </summary>
    /// <param name="host">主机的ip</param>
    /// <param name="port">绑定端口号</param>
    public void Start(string host,int port)
    {
        //初始化conns
        conns = new Conn[maxConn];

        for (int i = 0; i < conns.Length; i++)
        {
            conns[i] = new Conn();
        }
        //初始化socket
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Bind
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
        listenfd.Bind(iPEndPoint);
        //监听
        listenfd.Listen(maxConn);
        //开启接受连接
        listenfd.BeginAccept(AccetpCb, null);
        Debug.Log($"服务器启动成功,绑定ip为:{host},绑定地址为{port}");
    }

    /// <summary>
    /// 异步回调
    /// </summary>
    /// <param name="ar"></param>
    public void AccetpCb(IAsyncResult ar)
    {
        try
        {
            Socket socket = listenfd.EndAccept(ar);
            int index = NewIndex();
            if(index<0)
            {
                socket.Close();
                Debug.Log($"无可用的连接，连接池异常");
            }
            else
            {
                Conn conn = conns[index];
                conn.Init(socket);
                Debug.Log($"{socket.RemoteEndPoint.ToString()}---用户已连接---");
                conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BuffRemain(), SocketFlags.None,ReceiveCb ,conn);
                listenfd.BeginAccept(AccetpCb, null);

            }
        }
        catch ( Exception e)
        {
            Debug.LogError($"异步接收错误：{e.Message}");
        }
    }

    /// <summary>
    /// 异步接收消息
    /// </summary>
    /// <param name="ar"></param>
    private void ReceiveCb(IAsyncResult ar)
    {
        Conn conn = ar.AsyncState as Conn;
        lock(conn)
        {
            try
            {
                int count = conn.socket.EndReceive(ar);
                //关闭信号
                if(count<0)
                {
                    //断开 连接
                    Debug.Log($"{conn.socket.RemoteEndPoint.ToString()}---断开连接---");
                    conn.Close();
                    return;
                }

                //处理消息
                conn.buffCount += count;
                //交给处理类
                //TODO:
                ProcessData(conn);
                //继续接收
                conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BuffRemain(),SocketFlags.None,ReceiveCb, conn);
            }
            catch (Exception e)
            {
                Debug.LogError($"异步接收异常,{e.Message}");
            }
        }
    }

    /// <summary>
    /// 对缓冲区消息处理函数
    /// </summary>
    /// <param name="conn"></param>
    public void ProcessData(Conn conn)
    {
        //如果消息小于一定长度 直接不处理
        if (conn.buffCount < sizeof(Int32))
        {
            Debug.Log("消息太短");
            return;
        }

        //获取一个消息体的总长度
        //Array类的静态方法，从字节数组copy
        Array.Copy(conn.readBuff, conn.lenBytes, sizeof(Int32));
        //字节转换函数
        conn.msgLength = BitConverter.ToInt32(conn.lenBytes, 0);

        //如果消息不完整则不处理
        if (conn.buffCount < conn.msgLength + sizeof(Int32))
        {
            Debug.Log("消息不完整");
            return;
        }

        //将消息转译出来并处理
        //TODO:
        string msg = Encoding.UTF8.GetString(conn.readBuff, sizeof(Int32), conn.msgLength);
        Debug.Log($"收到了{conn.socket.RemoteEndPoint.ToString()}的消息，内容为：{msg}");

        //发送给xxx，待实现
        //TODO:
        Send(conn, msg);

        //清除已经处理的消息
        int remainMsgCount = conn.buffCount - sizeof(Int32) - conn.msgLength;//获取剩余未处理消息长度
        Array.Copy(conn.readBuff, sizeof(Int32) + conn.msgLength, conn.readBuff, 0, remainMsgCount);
        conn.buffCount = remainMsgCount;


        //递归调用处理
        if (conn.buffCount > 0)
        {
            ProcessData(conn);
        }
    }


    /// <summary>
    /// 发送字符串的方法，对字符串进行组装，未进行粘包分包确保发送完成
    /// </summary>
    /// <param name="conn">连接对象</param>
    /// <param name="str">发送的字符串</param>
    public void Send(Conn conn, string str)
    {
        byte[] strBytes = Encoding.UTF8.GetBytes(str);
        byte[] lengthBytes = BitConverter.GetBytes(strBytes.Length);

        //byte拼接函数
        //在Linq命名空间下
        byte[] sendBuff = lengthBytes.Concat(strBytes).ToArray();

        try
        {
            //没有异步回调函数
            //TODO:
            conn.socket.BeginSend(sendBuff, 0, sendBuff.Length, SocketFlags.None, null, null);
        }
        catch (Exception e)
        {
            Debug.Log($"发送字符串异常：{e.Message}");
        }

    }


    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        for (int i = 0; i < conns.Length; i++)
        {
            Conn conn = conns[i];
            if (conn == null)
                continue;
            if (!conn.isUse)
                continue;

            //加锁 避免线程竞争 
            //访问conn的线程有
            //主线程
            //异步回调
            //心跳的定时器线程
            lock (conn)
            {
                conn.Close();
            }
        }
    }
}