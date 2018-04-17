using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.Common;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class DataMgr
{
    MySqlConnection sqlConn;

    public DataMgr instance;

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
            return hasRows;


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
}
