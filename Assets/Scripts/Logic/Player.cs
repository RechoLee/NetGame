using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 玩家角色类，玩家下线，保存角色数据，向角色发送消息
/// </summary>
public class Player
{
    //角折id
    public string id;
    //连接对象
    public Conn conn;
    //角色数据
    public PlayerData data;
    //玩家的临时数据
    public PlayerTempData tempData;

    public Player()
    {

    }

    public Player(string _id, Conn _conn)
    {
        this.id = _id;
        this.conn = _conn;
        this.tempData = new PlayerTempData();
    }

    //发送
    /// <summary>
    /// 对ServNet中Send的封装
    /// </summary>
    /// <param name="protocol">使用的协议</param>
    public void Send(ProtocolBase protocol)
    {
        if (conn == null)
            return;
        ServNet.instance.Send(conn, protocol);
    }

    /// <summary>
    /// 将玩家踢下线
    /// </summary>
    /// <param name="id"></param>
    /// <param name="protocol"></param>
    /// <returns></returns>
    public static bool KickOff(string id,ProtocolBase protocol)
    {
        Conn[] conns = ServNet.instance.conns;

        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null || conns[i].isUse == false || conns[i].player == null)
                continue;
            if(conns[i].player.id.Equals(id))
            {
                lock (conns[i].player)
                {
                    if (protocol != null)
                        conns[i].player.Send(protocol);
                    return conns[i].player.Logout();
                }
            }

        }
        return true;
    }

    /// <summary>
    /// 下线并保存数据
    /// </summary>
    /// <returns></returns>
    public  bool Logout()
    {
        //事件处理
        //调用ServNet的HandlePlayerEvent
        //TODO:还未实现

        //保存数据
        //异步方法调用
        var result =DataMgr.instance.SavePlayer(this);
        if (result == false)
            return false;

        //下线
        conn.player = null;
        conn.Close();
        return true;
    }

    public async Task<bool> LogoutAsync()
    {
        //事件处理
        //调用ServNet的HandlePlayerEvent
        //TODO:还未实现

        //保存数据
        //异步方法调用
        var result =await DataMgr.instance.SavePlayerAsync(this);
        if (result == false)
            return false;

        //下线
        conn.player = null;
        conn.Close();
        return true;
    }
}
