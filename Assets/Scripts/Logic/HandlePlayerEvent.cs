using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 处理玩家事件的类
/// </summary>
public class HandlePlayerEvent
{
    /// <summary>
    /// 玩家上线事件
    /// </summary>
    /// <param name="player"></param>
    public void OnLogin(Player player)
    {
        //玩家上线
        Scene.instance.AddPlayer(player.id);
    }

    /// <summary>
    /// 玩家下线事件
    /// </summary>
    /// <param name="player"></param>
    public void OnLogout(Player player)
    {
        //玩家下线
        Scene.instance.DelPlayer(player.id);
    }
}