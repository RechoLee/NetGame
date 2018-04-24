using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景类 一个场景中可以其中包含对场景中玩家的操作
/// </summary>
public class Scene
{
    public static Scene instance;

    public Scene()
    {
        instance = this;
    }

    List<ScenePlayer> playerList = new List<ScenePlayer>();

    /// <summary>
    /// 根据id返回玩家信息 没有找到返回null
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ScenePlayer GetPlayer(string id)
    {
        for(int i=0;i<playerList.Count;i++)
        {
            if(id.Equals(playerList[i].id))
            {
                return playerList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 加入玩家
    /// </summary>
    /// <param name="id"></param>
    public void AddPlayer(string _id)
    {
        lock (playerList)
        {
            ScenePlayer player = new ScenePlayer
            {
                id = _id
            };
            playerList.Add(player);
        }
    }

    /// <summary>
    /// 删除玩家
    /// </summary>
    /// <param name="id"></param>
    public void DelPlayer(string _id)
    {
        lock (playerList)
        {
            ScenePlayer p = GetPlayer(_id);
            if (p == null)
                return;

            playerList.Remove(p);

            ///删除玩家成功后 通知服务器广播此玩家离开的消息
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("PlayerLeave");
            protocol.AddString(_id);
            ServNet.instance.Broadcast(protocol);
        }
    }

    /// <summary>
    /// 发送玩家列表 一个协议Get List
    /// </summary>
    /// <param name="player"></param>
    public void SendPlayerList(Player player)
    {
        int count = playerList.Count;
        //构造GetList协议
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetList");
        protocol.AddInt(count);
        for (int i = 0; i < count; i++)
        {
            ScenePlayer p = playerList[i];
            protocol.AddString(p.id);
            protocol.AddFloat(p.pos.X);
            protocol.AddFloat(p.pos.Y);
            protocol.AddFloat(p.pos.Z);
            protocol.AddInt(p.score);
        }

        //向玩家发送协议
        player.Send(protocol);
    }

    /// <summary>
    /// 更新场景中玩家的信息
    /// </summary>
    /// <param name="di"></param>
    /// <param name="pos"></param>
    /// <param name="score"></param>
    public void UpdateInfo(string id, Pos pos,int score)
    {
        int count = playerList.Count;
        ProtocolBytes protocol = new ProtocolBytes();
        ScenePlayer p = GetPlayer(id);
        if (p == null)
            return;

        p.pos.X = pos.X;
        p.pos.Y = pos.Y;
        p.pos.Z = pos.Z;
        p.score = score;
    }
}
