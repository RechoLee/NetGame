using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 处理玩家角色的协议类，为一个分部类
/// </summary>
public partial class HandlePlayerMsg
{
    /// <summary>
    /// 获取分数
    /// GetScore
    /// 返回协议 GetScore|分数
    /// </summary>
    /// <param name="player"></param>
    /// <param name="protocolBase"></param>
    public void MsgGetScore(Player player,ProtocolBase protocolBase)
    {
        ProtocolBytes protocol = protocolBase as ProtocolBytes;
        protocol.AddString("GetScore");
        protocol.AddInt(player.data.score);
        player.Send(protocol);
    }

    /// <summary>
    /// 处理客户端发来的增加AddScore命令 
    /// 服务器端不返回任何协议
    /// </summary>
    /// <param name="player"></param>
    /// <param name="protocolBase"></param>
    public void AddScore(Player player,ProtocolBase protocolBase)
    {
        //ProtocolBytes protocol = protocolBase as ProtocolBytes;
        //int start = 0;
        //string protoName = protocol.GetString(start, ref start);

        //处理
        player.data.score += 1;

        //保存
        //DataMgr.instance.SavePlayer(player);
    }

    /// <summary>
    /// 获取玩家列表
    /// </summary>
    /// <param name="player"></param>
    /// <param name="protocolBase"></param>
    public void MsgGetList(Player player,ProtocolBase protocolBase)
    {
        Scene.instance.SendPlayerList(player);
    }

    public void MsgUpdateInfo(Player player,ProtocolBase protocolBase)
    {
        //获取数值
        int start = 0;
        ProtocolBytes protocol = protocolBase as ProtocolBytes;
        string protoName = protocol.GetString(start,ref start);
        Pos pos = new Pos();
        pos.X =protocol.GetFloat(start,ref start).Value;
        pos.Y = protocol.GetFloat(start, ref start).Value;
        pos.Z = protocol.GetFloat(start, ref start).Value;
        int score = player.data.score;
        Scene.instance.UpdateInfo(player.id, pos, score);

        //广播
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("UpdateInfo");
        proto.AddString(player.id);
        proto.AddFloat(pos.X);
        proto.AddFloat(pos.Y);
        proto.AddFloat(pos.Z);
        proto.AddInt(score);
        ServNet.instance.Broadcast(proto);

    }
}
