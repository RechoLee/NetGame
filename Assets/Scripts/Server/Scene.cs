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
    /// 根据id返回玩家信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ScenePlayer GetPlayer(string id)
    {
        return null;
    }

    /// <summary>
    /// 加入玩家
    /// </summary>
    /// <param name="id"></param>
    public void AddPlayer(string id)
    {

    }

    /// <summary>
    /// 删除玩家
    /// </summary>
    /// <param name="id"></param>
    public void DelPlayer(string id)
    {

    }

    /// <summary>
    /// 发送玩家列表
    /// </summary>
    /// <param name="player"></param>
    public void SendPlayerList(Player player)
    {

    }

    /// <summary>
    /// 更新场景中玩家的信息
    /// </summary>
    /// <param name="di"></param>
    /// <param name="pos"></param>
    /// <param name="score"></param>
    public void UpdateInfo(string di,Pos pos,int score)
    {

    }
}
