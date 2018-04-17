using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MySql.Data.Common;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataMgr
{
    MySqlConnection sqlConn;

    

    public static DataMgr instance;

    public DataMgr()
    {
        instance = this;
    }

    /// <summary>
    /// 异步连接方法
    /// </summary>
    public async void Connect()
    {
        string conStr = "Database=game;Data Source=127.0.0.1;User Id=root;Password=lichun;Port=3306";

        sqlConn = new MySqlConnection(conStr);

        try
        {
            await sqlConn.OpenAsync();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// 判断传入的字符串是否安全
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public bool IsSafeStr(string str)
    {
        return !Regex.IsMatch(str,@"[-|;|,|\/|\(|\)|\[|\]|\{|\}|\%|\*|\!|\']");
    }

    public async Task<bool> CanRegister(string id)
    {
        //判断防止sql注入
        if (!IsSafeStr(id))
            return false;

        string commandStr = $"select * from user where id='{id}';";
        MySqlCommand sqlCommand = new MySqlCommand(commandStr, sqlConn);

        try
        {
            var reader = await sqlCommand.ExecuteReaderAsync();

            bool hasRows = reader.HasRows;
            reader.Close();
            return !hasRows;
        }
        catch (System.Exception)
        {
            return false;
        }

    }

    /// <summary>
    /// 异步注册方法
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pw"></param>
    /// <returns></returns>
    public async Task<bool> Register(string id,string pw)
    {
        if (!IsSafeStr(id) || !IsSafeStr(pw))
        {
            Debug.LogError("输入了非法字符");
            return false;
        }
        var resultReg = await CanRegister(id);

        if (resultReg == false)
        {
            Debug.LogError("id已被注册");
            return false;
        }

        string cmdStr = $"insert into user set id='{id}',pw='{pw}'";

        MySqlCommand sqlCommand = new MySqlCommand(cmdStr, sqlConn);

        try
        {
            int result=await sqlCommand.ExecuteNonQueryAsync();

            return result == 1;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> CreatePlayer(string id)
    {
        //防止sql注入
        if (!IsSafeStr(id))
            return false;

        //序列化
        PlayerData playerData = new PlayerData();

        IFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        try
        {
            formatter.Serialize(stream, playerData);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        //从流中获取byte
        byte[] byteArr = stream.ToArray();
        stream.Close();

        //写入数据库
        string cmdStr = $"insert into player set id='{id}',data=@data;";
        MySqlCommand sqlCommand = new MySqlCommand(cmdStr, sqlConn);
        //使用参数方式 byte不好使用sql语句
        sqlCommand.Parameters.Add("@data", MySqlDbType.Blob);
        sqlCommand.Parameters["@data"].Value = byteArr;

        try
        {
            var result = await sqlCommand.ExecuteNonQueryAsync();
            return result == 1;
        }
        catch (Exception e)
        {
            Debug.Log($"创建角色失败,{e.Message}");
            return false;
        }
    }
}
