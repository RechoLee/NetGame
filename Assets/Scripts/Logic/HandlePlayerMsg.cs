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
}
