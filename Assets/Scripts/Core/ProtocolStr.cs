using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 字符串协议，用来解析字符串,整个协议都用字符串表示
/// 形式：[名称，参数1，参数2，参数3] 中间中逗号给开
/// </summary>
public class ProtocolStr :ProtocolBase
{
    /// <summary>
    /// 传输的字符串
    /// </summary>
    public string str;

    /// <summary>
    /// 将byte数组解码成协议
    /// </summary>
    /// <param name="readBuff"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public override ProtocolBase Decode(byte[] readBuff, int start, int length)
    {
        //ProtocolStr protocolStr = new ProtocolStr();
        //protocolStr.str = Encoding.UTF8.GetString(readBuff, start, length);
        //return (protocolStr as ProtocolBase);

        this.str = Encoding.UTF8.GetString(readBuff, start, length);
        return (this as ProtocolBase);
    }

    /// <summary>
    /// 对协议的内容进行编码
    /// </summary>
    /// <returns></returns>
    public override byte[] Encode()
    {
        return Encoding.UTF8.GetBytes(str);
    }

    /// <summary>
    /// 获取协议的名称
    /// </summary>
    /// <returns></returns>
    public override string GetName()
    {
        //返回协议名称，如果str不为空返回第一个参数
        return string.IsNullOrEmpty(str)? "":str.Split(',')[0];
    }

    /// <summary>
    /// 获取消息体
    /// </summary>
    /// <returns></returns>
    public override string GetDesc()
    {
        return str;
    }
}
