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

        Register();

        CreatePlayer();
	}
	
    public async void Register()
    {
        var result =await DataMgr.instance.Register("recho","123456");
        if (result)
            Debug.Log("注册成功");
        else
            Debug.Log("注册失败");
    }

    public async void CreatePlayer()
    {
        var result = await DataMgr.instance.CreatePlayer("recho");

        if (result)
            Debug.Log("创建角色成功");
        else
            Debug.Log("创建角色失败");
    }

	// Update is called once per frame
	void Update () {
		
	}
}
