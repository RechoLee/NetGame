using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    DataMgr dataMgr;

    private void Awake()
    {
        dataMgr = new DataMgr();

        dataMgr.Connect();
    }

    // Use this for initialization
    void Start () {

        //Register();

        //CreatePlayer();

        //CheckLogin();

        //GetPlayerData();

        //SavePlayer();

        //DebugTimeStamp();
	}
	
    public async void Register()
    {
        var result =await DataMgr.instance.RegisterAsync("xiaoming","123456");
        if (result)
            Debug.Log("注册成功");
        else
            Debug.Log("注册失败");
    }

    public async void CreatePlayer()
    {
        var result = await DataMgr.instance.CreatePlayerAsync("xiaoming");

        if (result)
            Debug.Log("创建角色成功");
        else
            Debug.Log("创建角色失败");
    }

    /// <summary>
    /// 判断登陆
    /// </summary>
    public async void CheckLogin()
    {
        var result = await DataMgr.instance.CheckPasswordAsync("recho", "123456");
        if (result)
            Debug.Log("登陆成功");
        else
            Debug.Log("请检查用户名和密码");
    }

    public async void GetPlayerData()
    {
        var result = await DataMgr.instance.GetPlayerData("recho");

        if (result != null)
        {
            PlayerData playerData = result as PlayerData;
            Debug.Log($"玩家recho的分数为:{playerData.score}");
        }
        else
        {
            Debug.Log("不能读取玩家数据");
        }
    }

    public async void SavePlayer()
    {
        Player player = new Player
        {
            id = "recho",
            data = new PlayerData {score=3000 }
        };

        var result = await DataMgr.instance.SavePlayerAsync(player);

        if(result)
        {
            Debug.Log("更新成功");
            var result_PD = await DataMgr.instance.GetPlayerData("recho");
            if(result_PD!=null)
            {
                Debug.Log($"玩家分数为：{result_PD.score}");
            }
            else
            {
                Debug.Log("获取玩家数据失败");
            }
        }
        else
        {
            Debug.Log("更新失败");
        }

    }

    /// <summary>
    /// 测试时间戳
    /// </summary>
    public void DebugTimeStamp()
    {
        Debug.Log(Sys.GetTimeStamp());
    }

	// Update is called once per frame
	void Update () {
    }

    private void FixedUpdate()
    {
        //DebugTimeStamp();

    }
}
