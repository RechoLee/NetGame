using System;
using System.Collections;

/// <summary>
/// 协议基类
/// </summary>
public class ProtocolBase
{
    //解码器
    public virtual ProtocolBase Decode(byte[] readBuff,int start,int length)
    {
        return new ProtocolBase();
    }

    //编码器
    public virtual byte[] Encode()
    {
        return new byte[] { };
    }

    //获取协议名称
    public virtual string GetName()
    {
        return "";
    }

    //获取协议的描叙信息
    public virtual string GetDesc()
    {
        return "";
    }
}
