using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//玩家数据对象
[System.Serializable]
public class PlayerData
{

    public int score = 0;

    public PlayerData()
    {
        score = 100;
    }
}