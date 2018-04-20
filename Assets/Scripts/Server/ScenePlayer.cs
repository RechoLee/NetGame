using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景中玩家的定义
/// </summary>
public class ScenePlayer
{
    public string id;

    public Pos pos = new Pos {
        X = 0f,
        Y=0f,
        Z=0f
    };

    /// <summary>
    /// 玩家分数
    /// </summary>
    public int score = 0;
}

public class Pos
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}
