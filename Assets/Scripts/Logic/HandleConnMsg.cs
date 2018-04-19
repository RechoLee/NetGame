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

    /// <summary>
    /// 注册处理协议
    /// Register|用户名|密码
    /// 服务器端返回Register协议
    /// Register|-1/0
    /// -1代表注册失败 0代表注册成功
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="protocol"></param>
    public void MsgRegister(Conn conn,ProtocolBase protocolBase)
    {
        //获取数值
        int start = 0;
        ProtocolBytes protocol = protocolBase as ProtocolBytes;
        //ref start值最后会定位到下一条消息的起始位置
        string protoName = protocol.GetString(start,ref start);
        string id = protocol.GetString(start, ref start);
        string pw = protocol.GetString(start, ref start);
        Debug.Log($"收到{protoName}协议---用户名：{id},密码：{pw}");

        //构建返回协议
        protocol = new ProtocolBytes();
        protocol.AddString("Register");

        //判断注册信息
        if(DataMgr.instance.Register(id,pw))
        {
            //注册成功
            protocol.AddInt(0);

            //创建角色
            DataMgr.instance.CreatePlayer(id);
        }
        else
        {
            protocol.AddInt(-1);
        }

        //回发协议
        conn.Send(protocol);
    }

    /// <summary>
    /// 协议： Login|id|pw
    /// 客户端登陆功能协议处理方法
    /// 1.解析协议中的用户名和密码
    /// 2.验证
    /// 3.将已登陆的下线
    /// 4.读取数据
    /// 5.事件触发
    /// 6.放回Login协议 Login|-1/0
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="protocolBase"></param>
    public async void MsgLoginAsync(Conn conn,ProtocolBase protocolBase)
    {
        //获取协议数据
        int start = 0;
        ProtocolBytes protocol = protocolBase as ProtocolBytes;
        string protoName = protocol.GetString(start, ref start);
        string id = protocol.GetString(start,ref start);
        string pw = protocol.GetString(start, ref start);
        Debug.Log($"用户：{id}请求登陆");

        //构建返回的Login协议
        protocol = new ProtocolBytes();
        protocol.AddString("Login");

        //判断登陆信息正确与否
        var resultCheck =await DataMgr.instance.CheckPasswordAsync(id, pw);
        if (!resultCheck)
        {
            //验证失败
            protocol.AddInt(-1);
            conn.Send(protocol);
            return;
        }
        else
        {
            //验证成功
            protocol.AddInt(0);
        }

        //获取玩家数据初始化玩家
        var playerData =await DataMgr.instance.GetPlayerDataAsync(id);

        if(playerData==null)
        {
            protocol.AddInt(-1);
            conn.Send(protocol);
            return;
        }

        conn.player = new Player(id, conn) {data=playerData };

        //事件触发
        ServNet.instance.handlePlayerEvent.OnLogin(conn.player);

        //返回登陆成功的协议
        protocol.AddInt(0);
        conn.Send(protocol);
    }

    /// <summary>
    /// 协议： Login|id|pw
    /// 客户端登陆功能协议处理方法
    /// 1.解析协议中的用户名和密码
    /// 2.验证
    /// 3.将已登陆的下线
    /// 4.读取数据
    /// 5.事件触发
    /// 6.放回Login协议 Login|-1/0
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="protocolBase"></param>
    public void MsgLogin(Conn conn, ProtocolBase protocolBase)
    {
        //获取协议数据
        int start = 0;
        ProtocolBytes protocol = protocolBase as ProtocolBytes;
        string protoName = protocol.GetString(start, ref start);
        string id = protocol.GetString(start, ref start);
        string pw = protocol.GetString(start, ref start);
        Debug.Log($"用户：{id}请求登陆");

        //构建返回的Login协议
        protocol = new ProtocolBytes();
        protocol.AddString("Login");

        //判断登陆信息正确与否
        if (!DataMgr.instance.CheckPassword(id, pw))
        {
            //验证失败
            protocol.AddInt(-1);
            conn.Send(protocol);
            return;
        }
        else
        {
            //验证成功
            protocol.AddInt(0);
        }

        //获取玩家数据初始化玩家
        var playerData =DataMgr.instance.GetPlayerData(id);

        if (playerData == null)
        {
            protocol.AddInt(-1);
            conn.Send(protocol);
            return;
        }

        conn.player = new Player(id, conn) { data = playerData };

        //事件触发
        ServNet.instance.handlePlayerEvent.OnLogin(conn.player);

        //返回登陆成功的协议
        protocol.AddInt(0);
        conn.Send(protocol);
    }

    /// <summary>
    /// 退出的消息
    /// Logout|
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="protocolBase"></param>
    public void MsgLogout(Conn conn,ProtocolBase protocolBase)
    {
        ProtocolBytes protocol = protocolBase as ProtocolBytes;
        protocol.AddString("Logout");
        protocol.AddInt(0);
        conn.Send(protocol);
        if (conn.player==null)
        {
            conn.Close();
        }
        else
        {
            conn.player.Logout();
        }
    }
}
