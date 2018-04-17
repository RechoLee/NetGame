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

        CheckLogin();
	}
	
    public async void Register()
    {
        var result =await DataMgr.instance.Register("xiaoming","123456");
        if (result)
            Debug.Log("注册成功");
        else
            Debug.Log("注册失败");
    }

    public async void CreatePlayer()
    {
        var result = await DataMgr.instance.CreatePlayer("xiaoming");

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
        var result = await DataMgr.instance.CheckPassword("recho", "123456");
        if (result)
            Debug.Log("登陆成功");
        else
            Debug.Log("请检查用户名和密码");
    }

	// Update is called once per frame
	void Update () {
		
	}
}
