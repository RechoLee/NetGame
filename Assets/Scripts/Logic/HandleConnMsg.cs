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
        Debug.Log($"收到注册协议---用户名：{id},密码：{pw}");

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
}
