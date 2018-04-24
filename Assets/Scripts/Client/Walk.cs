using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour {

    /// <summary>
    /// 预设体
    /// </summary>
    public GameObject prefab;

    /// <summary>
    /// players集合
    /// </summary>
    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    /// <summary>
    /// 用户的id
    /// </summary>
    private string playerId = "";
    /// <summary>
    /// 上次移动的时间
    /// </summary>
    public float lastMoveTime;
    /// <summary>
    /// 单例
    /// </summary>
    public Walk instance;

    private void Awake()
    {
        instance = this;
    }


    /// <summary>
    /// 添加玩家
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_pos"></param>
    /// <param name="_score"></param>
    public void AddPlayer(string _id,Vector3 _pos,int _score)
    {
        GameObject player = Instantiate(prefab, _pos, Quaternion.identity);
        TextMesh textMesh = player.GetComponent<TextMesh>();
        textMesh.text = $"{_id}:{_score}";
        players.Add(_id,player);
    }

    /// <summary>
    /// 移除玩家
    /// </summary>
    /// <param name="id"></param>
    public void RemovePlayer(string id)
    {
        if(players.ContainsKey(id))
        {
            Destroy(players[id]);
            players.Remove(id);
        }
    }

    /// <summary>
    /// 更新玩家分数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="score"></param>
    public void UpdateScore(string id,int score)
    {
        GameObject player = players[id];
        if (player == null)
            return;
        TextMesh textMesh = players[id].GetComponent<TextMesh>();
        textMesh.text = $"{id}:{score}";

    }

    /// <summary>
    /// 更新 字典中的玩家信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    /// <param name="score"></param>
    public void UpdateInfo(string id,Vector3 pos,int score)
    {
        //id为自己的id
        if(playerId.Equals(id))
        {
            UpdateScore(id, score);
            return;
        }

        //其他人
        if(players.ContainsKey(id))
        {
            players[id].transform.position = pos;
            UpdateScore(id, score);
        }
        else
        {
            AddPlayer(id, pos, score);
        }
    }

    /// <summary>
    /// 刚开始进入游戏
    /// </summary>
    /// <param name="id"></param>
    public void StartGame(string id)
    {
        playerId = id;
        //随机产生一个位置
        Vector3 pos = new Vector3();
        pos.x = UnityEngine.Random.Range(-30f, 30f);
        pos.y = 0f;
        pos.z = UnityEngine.Random.Range(-30f,30f);
        AddPlayer(id, pos, 0);

        //同步
        SendPos();

        //获取列表
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("GetList");
        NetMgr.serverConn.Send(proto, GetList);
        NetMgr.serverConn.msgDistribution.AddListener("UpdateInfo", UpdateInfo);
        NetMgr.serverConn.msgDistribution.AddOnceListener("PlayerLeave", PlayerLeave);

    }


    /// <summary>
    /// 玩家离开的事件
    /// </summary>
    /// <param name="obj"></param>
    private void PlayerLeave(ProtocolBase obj)
    {
        ProtocolBytes proto = obj as ProtocolBytes;

        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        RemovePlayer(id);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    private void UpdateInfo(ProtocolBase obj)
    {
        //获取数值
        ProtocolBytes proto = obj as ProtocolBytes;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        Vector3 pos = new Vector3();
        pos.x = proto.GetInt(start, ref start).Value;
        pos.y = proto.GetInt(start, ref start).Value;
        pos.z = proto.GetInt(start, ref start).Value;
        int score = proto.GetInt(start, ref start).Value;

        UpdateInfo(id, pos, score);
    }


    /// <summary>
    /// 获取玩家列表
    /// </summary>
    /// <param name="obj"></param>
    private void GetList(ProtocolBase obj)
    {
        ProtocolBytes protocol = obj as ProtocolBytes;

        //解析协议
        int start = 0;
        string protoName = protocol.GetString(start, ref start);
        int count = protocol.GetInt(start, ref start).Value;
        for (int i = 0; i < count; i++)
        {
            string id = protocol.GetString(start, ref start);

            Vector3 pos = new Vector3();
            pos.x = protocol.GetFloat(start, ref start).Value;
            pos.y = protocol.GetFloat(start, ref start).Value;
            pos.z = protocol.GetFloat(start, ref start).Value;
            int score = protocol.GetInt(start, ref start).Value;

            UpdateInfo(id, pos, score);
        }

    }


    /// <summary>
    /// 同步位置
    /// </summary>
    public void SendPos()
    {
        GameObject player = players[playerId];
        if (player == null)
            return;
        Vector3 pos = player.transform.position;

        //构建UpdateInfo协议 和服务器端的协议有区别
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("UpdateInfo");
        protocol.AddFloat(pos.x);
        protocol.AddFloat(pos.y);
        protocol.AddFloat(pos.z);
        NetMgr.serverConn.Send(protocol);
    }

    /// <summary>
    /// 玩家移动相关逻辑
    /// </summary>
    public void Move()
    {
        if (string.IsNullOrEmpty(playerId))
            return;
        if (players[playerId] == null)
            return;
        if (Time.time - lastMoveTime < 0.1)
            return;
        lastMoveTime = Time.time;

        GameObject player = players[playerId];

        //上下左右
        if(Input.GetKey(KeyCode.UpArrow))
        {
            player.transform.position += new Vector3(0, 0, 1f);
            SendPos();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            player.transform.position += new Vector3(0,0,-1f);
            SendPos();
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            player.transform.position += new Vector3(-1f, 0, 0);
            SendPos();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            player.transform.position += new Vector3(1f,0,0);
            SendPos();
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            ProtocolBytes proto = new ProtocolBytes();
            proto.AddString("AddScore");
            NetMgr.serverConn.Send(proto);
        }

    }

	
	// Update is called once per frame
	void Update () {
        Move();
	}
}
